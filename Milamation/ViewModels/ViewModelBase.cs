using Caliburn.Micro;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Milamation.ViewModels
{
    public abstract class ViewModelBase<T> : Screen where T : ViewModelBase<T>
    {
        protected readonly ILogger<T> logger;

        protected ViewModelBase(ILogger<T> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected async virtual Task NavigateToAsync<TViewModel>(CancellationToken cancellationToken) where TViewModel : ViewModelBase<TViewModel>
        {   
            var shell = Parent as ShellViewModel;
            if (shell != null)
            {
                await shell.NavigateToAsync<TViewModel>(cancellationToken);
            }
        }
    }
}
