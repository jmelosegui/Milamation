using Caliburn.Micro;
using HarvestClient;
using HarvestClient.Model;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;

namespace Milamation.ViewModels
{
    public class LoginViewModel : Screen
    {
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

        private int? accountId;
        public int? AccountId
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

        protected override void OnInitialize()
        {
            var credentials = ReadFromFile();

            if (credentials != null)
            {
                int indexToken = credentials.IndexOf("#!#");
                if (int.TryParse(credentials.Substring(0, indexToken), out int aId))
                {
                    AccountId = aId;
                }

                Token = credentials.Substring(indexToken + 3);

                if (AccountId.HasValue && CanSignIn(AccountId.Value, Token))
                {
                    SignIn(AccountId.Value, Token);
                }
            }
        }

        public bool CanSignIn(int accountId, string token)
        {
            return !string.IsNullOrEmpty(token) && accountId > 0;
        }

        public async void SignIn(int accountId, string token)
        {
            HarvestRestClient harvestClient = new HarvestRestClient(token, accountId);

            User user = null;

            try
            {
                user = await harvestClient.Users.GetCurrent();
            }
            catch (System.Exception)
            {
                MessageBox.Show("Wrong Account Id or Token");
            }

            if (user != null)
            {
                WriteToFile($"{accountId}#!#{token}");
                var shell = Parent as ShellViewModel;
                if (shell != null)
                {   
                    shell.ActivateItem(new TimeEntriesReportViewModel(token, accountId));
                }
            }
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
                //Si no puedo leer del fichero sigo adelante
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
            }
        }
    }
}
