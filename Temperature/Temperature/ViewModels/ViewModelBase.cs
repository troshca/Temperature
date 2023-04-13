using Acr.UserDialogs;
using Plugin.BLE;
using Plugin.BLE.Abstractions.EventArgs;
using Prism.AppModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Temperature.ViewModels
{
    public class ViewModelBase : BindableBase, IInitialize, INavigationAware, IDestructible, IPageLifecycleAware, IDisposable
    {
        protected INavigationService NavigationService { get; private set; }
        protected IUserDialogs UserDialogsService { get; private set; }


        private string _title;

        
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ViewModelBase(INavigationService navigationService, IUserDialogs userDialogsService)
        {
            NavigationService = navigationService;
            var ble = CrossBluetoothLE.Current;
            var adapter = CrossBluetoothLE.Current.Adapter;
            ble.StateChanged += OnStateChangd;
            UserDialogsService = userDialogsService;
        }

        private void OnStateChangd(object sender, BluetoothStateChangedArgs e)
        {
            Debug.WriteLine($"The bluetooth state changed to {e.NewState}");
        }

        public virtual void Initialize(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public virtual void Destroy()
        {

        }

        public virtual void OnAppearing()
        {
        }

        public virtual void OnDisappearing()
        {
        }

        public virtual void Dispose() { }
    }
}
