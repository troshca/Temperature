using Acr.UserDialogs;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Temperature.ViewModels
{
    public class HistoryPageViewModel : ViewModelBase
    {
        public HistoryPageViewModel(INavigationService navigationService, IUserDialogs userDialogsService) : base(navigationService, userDialogsService)
        {
        }
    }
}
