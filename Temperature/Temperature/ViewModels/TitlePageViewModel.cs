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
using Xamarin.Forms;
using System;
using System.Threading;

namespace Temperature.ViewModels
{
    public class TitlePageViewModel : ViewModelBase
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

        private DelegateCommand _scanCommand;
        public DelegateCommand ScanCommand =>
            _scanCommand ?? (_scanCommand = new DelegateCommand(async () => await ExecuteScanCommand()));

        static readonly CancellationTokenSource s_cts = new CancellationTokenSource();

        async Task ExecuteScanCommand()
        {
            var permissionResult = await DependencyService.Get<Helpers.IPlatformHelpers>().CheckAndRequestBluetoothPermissions();
            if (permissionResult != PermissionStatus.Granted)
            {
                await UserDialogsService.AlertAsync("Permission denied. Not scanning.");
                AppInfo.ShowSettingsUI();
            }
            else
            {
                UserDialogsService.ShowLoading("Scan Bluetooth Device", MaskType.Gradient);
                DeviceList = new List<IDevice>();
                await _adapterService.StartScanningForDevicesAsync();
                DeviceList = _adapterService.DiscoveredDevices.ToList();
            }
            UserDialogsService.HideLoading();
        }

        private DelegateCommand<IDevice> _itemSelectedCommand;

        public DelegateCommand<IDevice> ItemSelectedCommand =>
            _itemSelectedCommand ?? (_itemSelectedCommand = new DelegateCommand<IDevice>((a) => ConnectCommand(a)));

        private void ConnectCommand(IDevice device)
        {
            _ = UserDialogs.Instance.Confirm(new ConfirmConfig
            {
                Message = "Подключиться? ",
                OkText = "Да",
                CancelText = "Отмена",
                Title = "Подключиться к " + device.Name,
                OnAction = async (confirmed) => { if (!confirmed) return; else await ExecuteItemSelectedCommand(device); }
            });
        }

        private async Task ExecuteItemSelectedCommand(IDevice device)
        {
            try
            {
                UserDialogsService.ShowLoading("Connect To Device", MaskType.Gradient);
                s_cts.CancelAfter(3500);
                await _adapterService.DisconnectDeviceAsync(device);
                await _adapterService.ConnectToDeviceAsync(device);
                var navigationParams = new NavigationParameters
                {
                    { "device", device }
                };
                var result = await NavigationService.NavigateAsync("NavigationPage/TemperatureSensorPage", navigationParams);
                //await ExecuteScanCommand();
            }
            catch (OperationCanceledException e)
            {

                UserDialogsService.Alert(e.Message);
            }
            catch (DeviceConnectionException e)
            {
                UserDialogsService.Alert(e.Message);
            }
            catch (Exception ex)
            {
                UserDialogsService.Alert(ex.Message);
            }
            finally
            {
                s_cts.Dispose();
                UserDialogsService.HideLoading();
            }
        }

        public TitlePageViewModel(INavigationService navigationService, IUserDialogs userDialogsService)
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
    }
}