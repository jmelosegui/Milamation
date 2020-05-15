using Caliburn.Micro;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Milamation.ViewModels
{
    public class ShellViewModel : Conductor<IScreen>
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IAppUpdater appUpdater;
        private readonly ILogger<ShellViewModel> logger;

        public ShellViewModel(IServiceProvider serviceProvider, IAppUpdater appUpdater, ILogger<ShellViewModel> logger)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.appUpdater = appUpdater ?? throw new ArgumentNullException(nameof(appUpdater));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool NewVersionAvailable { get; private set; }

        public async virtual Task NavigateToAsync<T>(CancellationToken cancellationToken) where T : ViewModelBase<T>
        {            
            T viewModel = (T)serviceProvider.GetService(typeof(T));
            
            if (viewModel != null)
            {
                await ActivateItemAsync(viewModel, cancellationToken);
            }
            else
            {
                throw new InvalidOperationException($"Cannot find the view model \"{typeof(T).FullName}\" please ensure it is registerd in the DI container.");
            }
        }

        internal async Task CheckForUpdatesAsync(CancellationToken cancellationToken)
        {
            try
            {
                var result = await appUpdater.GetLatestVersionAsync(cancellationToken);
                if (result.HasValue && result.Value.CurrentVersion != result.Value.FutureVersion)
                {
                    logger.LogInformation($"Updater will fetch version {result.Value.FutureVersion}");
                    await appUpdater.UpdateAsync(CancellationToken.None);
                    NewVersionAvailable = true;

                    MessageBox.Show("There is a new version available. Please restart the application to update.", "Information", MessageBoxButton.OK);

                    logger.LogInformation("Update finished, restart required");
                }
                else
                {
                    logger.LogInformation("Got no response from updater or version is current");
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed checking for updates");
            }
        }

        protected override async Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            _ = CheckForUpdatesAsync(cancellationToken);

            await NavigateToAsync<LoginViewModel>(cancellationToken);
        }
    }
}
