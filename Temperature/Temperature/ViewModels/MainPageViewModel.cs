using Acr.UserDialogs;
using Temperature.Helpers;
using Temperature.Services.BluetoothService;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using Prism.Commands;
using Prism.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Temperature.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        readonly IAdapter _adapterService;
        readonly IBluetoothLE _bluetoothLE;


        private List<IDevice> _deviceList;

        public List<IDevice> DeviceList
        {
            get { return _deviceList; }
            set { SetProperty(ref _deviceList, value); }
        }

        private IDevice _selectedItem;

        public IDevice SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }

        //private bool isRefreshing = false;

        //public bool IsRefreshing
        //{
        //    get { return isRefreshing; }
        //    set { SetProperty(ref isRefreshing, value); }
        //}

        private DelegateCommand _scanCommand;
        public DelegateCommand ScanCommand =>
            _scanCommand ?? (_scanCommand = new DelegateCommand(async () => await ExecuteScanCommand()));

        async Task ExecuteScanCommand()
        {
            var LocationPermission = await PermissionHelpers.RequestIfNeeded<Permissions.LocationAlways>();
            
            UserDialogsService.ShowLoading("Scan Bluetooth Device", MaskType.Gradient);
            if (LocationPermission == PermissionStatus.Granted)
            {
                DeviceList = new List<IDevice>();
                await _adapterService.StartScanningForDevicesAsync();
                DeviceList = _adapterService.DiscoveredDevices.ToList();
            }
            else
            {
                await UserDialogsService.AlertAsync("Location Permission is required");
            }
            UserDialogsService.HideLoading();

        }


        private DelegateCommand<IDevice> _itemSelectedCommand;

        public DelegateCommand<IDevice> ItemSelectedCommand =>
            _itemSelectedCommand ?? (_itemSelectedCommand = new DelegateCommand<IDevice>(async (a) => await ExecuteItemSelectedCommand(a)));

        //private DelegateCommand _refreshCommand;

        //public DelegateCommand RefreshCommand =>
        //    _refreshCommand ?? (_refreshCommand = new DelegateCommand(async () => await ExecuteRefreshCommand()));

        //private async Task ExecuteRefreshCommand()
        //{
        //    IsRefreshing = true;
        //    await GetBlutoothDevices();
        //    IsRefreshing = false;
        //}

        private async Task ExecuteItemSelectedCommand(IDevice device)
        {
            try
            {
                UserDialogsService.ShowLoading("Connect To Device", MaskType.Gradient);
                await _adapterService.ConnectToDeviceAsync(device);
                UserDialogsService.HideLoading();
                await ExecuteScanCommand();
            }
            catch (DeviceConnectionException e)
            {

            }
        }

        public MainPageViewModel(INavigationService navigationService, IUserDialogs userDialogsService)
            : base(navigationService, userDialogsService)
        {
            _adapterService = CrossBluetoothLE.Current.Adapter;
            _bluetoothLE = CrossBluetoothLE.Current;
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
        }

        public async override void OnAppearing()
        {
            base.OnAppearing();

        }

        private async Task GetBlutoothDevices()
        {
            var LocationPermission = await PermissionHelpers.RequestIfNeeded<Permissions.LocationAlways>();
            if (LocationPermission == PermissionStatus.Granted)
            {
                DeviceList = new List<IDevice>();

                await _adapterService.StartScanningForDevicesAsync();
                _adapterService.DeviceDiscovered += (s, a) => DeviceList.Add(a.Device);

            }
            else
            {
                await UserDialogsService.AlertAsync("Location Permission is required");
            }

        }
    }
}