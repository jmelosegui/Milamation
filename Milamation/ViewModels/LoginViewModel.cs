using HarvestClient;
using HarvestClient.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Milamation.ViewModels
{
    public class LoginViewModel : ViewModelBase<LoginViewModel>
    {
        private readonly IHarvestRestClientFactory factory;
        private IPrincipal principal;

        public LoginViewModel(IHarvestRestClientFactory factory, ILogger<LoginViewModel> logger, IPrincipal principal)
            : base(logger)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.principal = principal;
        }

        private string token;
        public string Token
        {
            get { return token; }
            set
            {
                token = value;
                NotifyOfPropertyChange(() => Token);
            }
        }

        private string accountId;
        public string AccountId
        {
            get { return accountId; }
            set
            {
                accountId = value;
                NotifyOfPropertyChange(() => AccountId);
            }
        }

        public string Version
        {
            get
            {
                FileVersionInfo fv = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
                return fv.FileVersion.ToString();
            }
        }

        public void NavigateToHarvest()
        {
            Process myProcess = new Process();
            myProcess.StartInfo.UseShellExecute = true;
            myProcess.StartInfo.FileName = "https://id.getharvest.com/developers";
            myProcess.Start();
        }

        protected async override Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug("Initializing viewmodel");
            var credentials = ReadFromFile();

            if (credentials != null)
            {
                int indexToken = credentials.IndexOf("#!#");
                AccountId = credentials.Substring(0, indexToken);

                Token = credentials.Substring(indexToken + 3);

                if (AccountId != null && CanSignIn(AccountId, Token))
                {
                    await SignIn(AccountId, Token, cancellationToken);
                }
            }
        }

        public bool CanSignIn(string accountId, string token)
        {
            return !string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(accountId);
        }

        public async Task SignIn(string accountId, string token, CancellationToken cancellationToken)
        {
            IHarvestRestClient harvestClient = factory.CreateHarvestRestClient(token, accountId);

            try
            {
                User user = await harvestClient.Users.GetCurrentAsync();
                SetPrincipal(user);
                WriteToFile($"{accountId}#!#{token}");
                await NavigateToAsync<TimeEntriesReportViewModel>(cancellationToken);
            }
            catch (System.Exception)
            {
                MessageBox.Show("Wrong Account Id or Token");
            }
        }

        private void SetPrincipal(User user)
        {
            IEnumerable<Claim> claims = new List<Claim>()
            {
                new Claim("name", principal.Identity.Name),
                new Claim("harvest:accountId", accountId),
                new Claim("harvest:token", token),
                new Claim("harvest:userId", user.Id.ToString()),
            };

            ClaimsIdentity claimIdentity = new ClaimsIdentity(principal.Identity, claims);

            ((WindowsPrincipal)this.principal).AddIdentity(claimIdentity);
            
        }

        private static string ReadFromFile()
        {
            try
            {
                IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
                using (var stream = new IsolatedStorageFileStream("milamation.txt", FileMode.Open, isoStore))
                {
                    var binaryFormatter = new BinaryFormatter();
                    return binaryFormatter.Deserialize(stream) as string;
                }
            }
            catch
            {
                //If cannot read the file just continue
                return null;
            }
        }

        private static void WriteToFile(string value)
        {
            try
            {
                IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
                using (var stream = new IsolatedStorageFileStream("milamation.txt", FileMode.OpenOrCreate, isoStore))
                {
                    var binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(stream, value);
                }
            }
            catch
            {
                //If cannot write the file just continue
            }
        }
    }
}
