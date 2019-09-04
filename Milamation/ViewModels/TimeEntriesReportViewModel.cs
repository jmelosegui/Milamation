using Caliburn.Micro;
using HarvestClient;
using HarvestClient.Model;
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
using System.Threading.Tasks;
using System.Windows;

namespace Milamation.ViewModels
{
    public class TimeEntriesReportViewModel : Screen
    {
        private readonly HarvestRestClient harvestClient;
        private readonly IDictionary<int, User> userCache;
        private Client selectedClient;
        private DateTime? startDate;
        private DateTime? endDate;
        private List<Rule> rules;
        public TimeEntriesReportViewModel(string token, int accountId)
        {
            harvestClient = new HarvestRestClient(token, accountId);
            userCache = new Dictionary<int, User>();

            Clients = new BindableCollection<Client>();
            Projects = new BindableCollection<ProjectModel>();

            Projects.CollectionChanged += Projects_CollectionChanged;

            this.rules = new List<Rule>
            {
                new IsMeetingRule(),
                new EmptyNotesRule(),
                new NeedPBIRule(),
                new RemoveWordsRule(),
                new NoDescriptionRule(),
                // new NeedProperTaskRule()
            };
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

        public BindableCollection<Client> Clients { get; }

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

        public Client SelectedClient
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
                await foreach (var entry in harvestClient.Timesheets.List(selectedClient.Id, project.Id, StartDate, EndDate))
                {
                    entry.UserRoles = await GetUserRoles(entry.User.Id);
                    entry.CompletePBIColumn();
                    entry.RoundHours();
                    entry.ApplyValidationRules(rules);
                    timeEntries.Add(entry);
                }

                TimeEntriesReport.AddEntries(timeEntries, 2 /*Asuming the header is present*/);
            }

            TimeEntriesReport.FormatExcel(timeEntries.Count());

            TimeEntriesReport.OpenReport();

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

        protected override void OnInitialize()
        {
            base.OnInitialize();
            PropertyChanged += OnPropertyChange; ;
            InternalInitialize();

            // Getting last week dates
            var dt = DateTime.Now;
            StartDate = dt.AddDays(-(int)dt.DayOfWeek - 6);
            EndDate = dt.AddDays(-(int)dt.DayOfWeek - 2);
        }

        private void OnPropertyChange(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SelectedClient):
                    OnClientSelected();
                    break;
            }
        }

        private async void OnClientSelected()
        {
            IsBusy = true;

            try
            {
                List<Project> tempList = new List<Project>();
                await foreach (var item in harvestClient.Projects.List(selectedClient.Id, true))
                {
                    tempList.Add(item);                    
                }

                foreach (var project in tempList.Select(i => new ProjectModel(i)).OrderBy(i => i.Code))
                {
                    Projects.Add(project);
                }                
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void InternalInitialize()
        {
            var result = await harvestClient.Clients.GetById(32315 /*PwC Audit*/);

            Clients.Add(result);
        }

        private async Task<string> GetUserRoles(int userId)
        {
            User result;

            if (!userCache.TryGetValue(userId, out result))
            {
                result = await harvestClient.Users.GetUserById(userId);

                userCache.Add(userId, result);
            }

            return string.Join(',', result.Roles);
        }
    }
}
