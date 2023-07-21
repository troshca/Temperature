using Acr.UserDialogs;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Temperature.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        public SettingsPageViewModel(INavigationService navigationService, IUserDialogs userDialogsService) : base(navigationService, userDialogsService)
        {

        }
    }
}
