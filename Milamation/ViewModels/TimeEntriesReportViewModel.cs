﻿using Caliburn.Micro;
using HarvestClient;
using HarvestClient.Model;
using Microsoft.Extensions.Logging;
using Milamation.Extensions;
using Milamation.Models;
using Milamation.ValidationRules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Milamation.ViewModels
{
    public class TimeEntriesReportViewModel : ViewModelBase<TimeEntriesReportViewModel>
    {
        private readonly IHarvestRestClient harvestClient;
        private readonly IDictionary<int, User> userCache;
        private readonly IEnumerable<Rule> rules;
        private readonly IPrincipal principal;
        private ClientModel selectedClient;
        private DateTime? startDate;
        private DateTime? endDate;

        public TimeEntriesReportViewModel(
                IHarvestRestClientFactory harvestClientFactory,
                IEnumerable<Rule> rules,
                IPrincipal principal,
                ILogger<TimeEntriesReportViewModel> logger)
            : base(logger)
        {
            //harvestClient = new HarvestRestClient(token, accountId);
            userCache = new Dictionary<int, User>();

            Clients = new BindableCollection<ClientModel>();
            Projects = new BindableCollection<ProjectModel>();

            Projects.CollectionChanged += Projects_CollectionChanged;
            string bearerToken = ((ClaimsPrincipal)principal).FindFirst("harvest:token").Value;
            string accountId = ((ClaimsPrincipal)principal).FindFirst("harvest:accountId").Value;
            harvestClient = harvestClientFactory.CreateHarvestRestClient(bearerToken, accountId);
            this.rules = rules ?? throw new ArgumentNullException(nameof(rules));
            this.principal = principal;
            PropertyChanged += OnPropertyChange;
        }

        private void Projects_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (ProjectModel project in e.NewItems)
                    project.PropertyChanged += Project_PropertyChanged;

            if (e.OldItems != null)
                foreach (ProjectModel project in e.OldItems)
                    project.PropertyChanged -= Project_PropertyChanged;
        }

        private void Project_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                NotifyOfPropertyChange(() => CanExportTimeEntries);
            }
        }

        public BindableCollection<ClientModel> Clients { get; }

        public BindableCollection<ProjectModel> Projects { get; }

        public string Version
        {
            get
            {
                FileVersionInfo fv = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
                return fv.FileVersion.ToString();
            }
        }

        public DateTime? StartDate
        {
            get
            {
                return startDate;
            }

            set
            {
                startDate = value;
                NotifyOfPropertyChange(() => StartDate);
            }
        }

        public DateTime? EndDate
        {
            get
            {
                return endDate;
            }

            set
            {
                endDate = value;
                NotifyOfPropertyChange(() => EndDate);
            }
        }

        private bool onlyMyEntries;

        public bool OnlyMyEntries
        {
            get
            {
                return onlyMyEntries;
            }

            set
            {
                onlyMyEntries = value;
                NotifyOfPropertyChange(() => OnlyMyEntries);
            }
        }
        public ClientModel SelectedClient
        {
            get { return selectedClient; }
            set
            {
                selectedClient = value;
                NotifyOfPropertyChange(() => SelectedClient);
                NotifyOfPropertyChange(() => CanExportTimeEntries);
            }
        }

        private bool isBusy;

        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                NotifyOfPropertyChange(() => IsBusy);
            }
        }

        public bool CanExportTimeEntries
        {
            get
            {
                return selectedClient != null
                        && Projects.Where(mo => mo.IsSelected).Any();
            }
        }

        private string clientFilterText;

        public string ClientFilterText
        {
            get { return clientFilterText; }
            set
            {
                clientFilterText = value;
                NotifyOfPropertyChange(() => ClientFilterText);
                this.ClientFilter = (client) =>
                {

                    if (string.IsNullOrEmpty(this.clientFilterText))
                    {
                        return true;
                    }

                    var name = ((IFilterable)client).DisplayName;
                    if (string.IsNullOrEmpty(name))
                    {
                        return false;
                    }

                    return name.IndexOf(this.ClientFilterText, StringComparison.OrdinalIgnoreCase) >= 0;
                };
            }
        }

        private Predicate<object> clientFilter;
        public Predicate<object> ClientFilter
        {
            get { return this.clientFilter; }
            private set
            {
                this.clientFilter = value;
                this.NotifyOfPropertyChange(() => ClientFilter);
            }
        }

        public async void ExportTimeEntries()
        {
            IsBusy = true;
            try
            {
                TimeEntriesReport.CreateExcelHeader();
            }
            catch (IOException ex)
            {
                IsBusy = false;

                if (ex.Message.Contains("The process cannot access the file"))
                {
                    MessageBox.Show("The process cannot access the file. Verify the report is not open.", "Error", MessageBoxButton.OKCancel);

                    return;
                }
                throw;
            }

            var timeEntries = new List<TimesheetEntry>();

            foreach (var project in Projects.Where(mo => mo.IsSelected))
            {
                int? userId = null;
                if (OnlyMyEntries && int.TryParse(((ClaimsPrincipal)principal).FindFirst("harvest:userId").Value, out int tempUserId))
                {
                    userId = tempUserId;
                }

                var timesheet = harvestClient.Timesheets
                             .ListAsync(selectedClient.Id, project.Id, StartDate, EndDate, userId);

                await foreach (var entry in timesheet)
                {
                    entry.UserRoles = await GetUserRolesAsync(entry.User.Id);
                    entry.CompletePBIColumn();
                    entry.RoundHours();
                    entry.ApplyValidationRules(rules);
                    timeEntries.Add(entry);
                }
            }

            TimeEntriesReport.AddEntries(timeEntries.OrderByDescending(i => i.SpentDate), 2 /*Asuming the header is present*/);

            var timeentryCount = timeEntries.Count();

            if (timeentryCount > 0)
            {
                TimeEntriesReport.FormatExcel(timeentryCount);

                TimeEntriesReport.OpenReport();
            }
            else
            {
                MessageBox.Show("Cannot find any time entry for the provided parameters.", "Information", MessageBoxButton.OK);
            }

            IsBusy = false;
        }

        public void Logout()
        {
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            isoStore.DeleteFile("milamation.txt");

            var mainwindows = ((ShellViewModel)Parent).GetView() as Window;
            if (mainwindows != null)
            {
                mainwindows.Close();
            }
        }

        protected async override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            if (this.IsInitialized)
            {
                return;
            }

            await base.OnActivateAsync(cancellationToken);

            await InternalInitialize();

            // Getting last week dates
            var dt = DateTime.Now;
            StartDate = dt.AddDays(-(int)dt.DayOfWeek - 6);
            EndDate = dt.AddDays(-(int)dt.DayOfWeek - 2);
        }

        private async void OnPropertyChange(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SelectedClient):
                    await OnClientSelected();
                    break;
                case nameof(ClientFilter):
                    OnClientFiltered();
                    break;
                // TODO: Remove this hack once I figure out how to always run the OnActivateAsync
                case nameof(Parent):
                    await OnActivateAsync(CancellationToken.None);
                    break;
            }
        }

        private async Task OnClientSelected()
        {
            IsBusy = true;

            try
            {

                Projects.Clear();
                if (selectedClient != null)
                {
                    IOrderedAsyncEnumerable<ProjectModel> list = harvestClient.Projects
                                                                  .ListAsync(selectedClient.Id, true)
                                                                  .Select(i => new ProjectModel(i))
                                                                  .OrderBy(i => i.Code);
                    await foreach (var project in list)
                    {
                        Projects.Add(project);
                    }
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void OnClientFiltered()
        {

        }

        private async Task InternalInitialize()
        {
            IsBusy = true;

            try
            {
                //// TODO: I should have this through some sort of configuration.
                //var clients = new Client[]
                //{
                //    new Client
                //    {
                //        Id = 32315,
                //        Name = "PricewaterhouseCoopers"
                //    } , 
                //    //8090413, /* PwC Lux */
                //    //25558, /* Arrow Digital, LLC */
                //};

                var clients = harvestClient.Clients
                                           .ListAsync(isActive: true)
                                           .OrderBy(c => c.Id)
                                           .Select(c => new ClientModel(c));
                await foreach (var client in clients)
                {
                    if (client != null)
                    {
                        Clients.Add(client);
                    }
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task<string> GetUserRolesAsync(int userId)
        {
            User result;

            if (!userCache.TryGetValue(userId, out result))
            {
                result = await harvestClient.Users.GetUserByIdAsync(userId);

                userCache.Add(userId, result);
            }

            return string.Join(',', result.Roles);
        }
    }
}
