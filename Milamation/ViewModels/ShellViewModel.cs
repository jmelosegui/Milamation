using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Text;

namespace Milamation.ViewModels
{
    public class ShellViewModel : Conductor<IScreen>
    {
        protected override void OnInitialize()
        {
            ActivateItem(new LoginViewModel());
        }
    }
}
