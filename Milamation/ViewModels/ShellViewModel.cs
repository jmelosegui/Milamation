using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Milamation.ViewModels
{
    public class ShellViewModel : Conductor<IScreen>
    {
        private readonly IServiceProvider serviceProvider;

        public ShellViewModel(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

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

        protected override async Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            await NavigateToAsync<LoginViewModel>(cancellationToken);
        }
    }
}
