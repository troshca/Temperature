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
using Plugin.BLE.Abstractions;
using Prism.Common;
using Temperature.Models;
using Plugin.BLE.Abstractions.Extensions;
using Plugin.BLE.Abstractions.EventArgs;
using System.Collections;

namespace Temperature.ViewModels
{
    public class TemperatureSensorPageViewModel : ViewModelBase
    {
        private bool _updatesTemperatureStarted;
        //public double Temperature;
        private double _temperature;
        private int _humidity;
        public double Temperature
        {
            get => _temperature;
            set => SetProperty(ref _temperature, value);
        }
        public int Humidity
        {
            get => _humidity;
            set => SetProperty(ref _humidity, value);
        }

        readonly IAdapter _adapterService;
        readonly IBluetoothLE _bluetoothLE;
        readonly IBluetoothService _bluetoothService;
        private IDevice _device;
        private IService _service;
        private IReadOnlyList<ICharacteristic> _characteristics;
        public IReadOnlyList<ICharacteristic> Characteristics
        {
            get => _characteristics;
            private set => SetProperty(ref _characteristics, value);
        }

        public ObservableCollection<string> Messages { get; } = new ObservableCollection<string>();
        public ICharacteristic Characteristic { get; private set; }
        public string CharacteristicValue => Characteristic?.Value.ToHexString().Replace("-", " ");
        private bool _updatesStarted;
        public bool UpdatesStarted
        {
            get { return _updatesStarted; }
            set
            {
                SetProperty(ref _updatesStarted, value);
            }
        }
        private string _updateButtonText = "Включить автообновление";
        public string UpdateButtonText
        {
            get { return _updateButtonText; }
            set
            {
                if (_updatesStarted)
                { _updateButtonText = "Выключить автообновление"; }
                else { _updateButtonText = "Включить автообновление"; }
            }
        }
           
        KnownService temperatureCUS = new KnownService("Unknown Service", new Guid("0000abcd-1212-efde-1523-785fef13d123"));

        private DelegateCommand _UpdateCommand;

        public DelegateCommand UpdateCommand =>
            _UpdateCommand ?? (_UpdateCommand = new DelegateCommand(async () => await Update()));
        //public MvxAsyncCommand ToggleUpdatesCommand => new MvxAsyncCommand((() => _updatesStarted ? StopUpdates() : StartUpdates()));
        public DelegateCommand ToggleUpdatesCommand => new DelegateCommand(async () => await Updates());
            


        public TemperatureSensorPageViewModel(INavigationService navigationService, IUserDialogs userDialogsService) : base(navigationService, userDialogsService)
        {
            _adapterService = CrossBluetoothLE.Current.Adapter;
            _bluetoothLE = CrossBluetoothLE.Current;


        }

        private async Task Update()
        {
            await UpdateTemperature();
            await UpdateHumidity();
        }
        private async Task UpdateHumidity()
        {
            await ReadValueAsync(Characteristics[0]);
            Humidity = Characteristics[0].Value[0];//ConvertHumidity(Characteristics[0].Value);
        }
        private async Task UpdateTemperature()
        {
            await ReadValueAsync(Characteristics[1]);
            Temperature = ConvertTemperature(Characteristics[1].Value);
        }

        private double ConvertTemperature(byte[] array)
        {
            if (array == null) { return 0; }
            double _temperature = 0;
            _temperature += ((array[1] << 8) | array[0]);

            return ((((_temperature / 65536.0) * 165.0) - 40.0));
        }
        private int ConvertHumidity(byte[] ar)
        {
            return (int)((ar[0] / 65536.0) * 100.0);
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            _device = parameters.GetValue<IDevice>("device");
            await DiscoverService(temperatureCUS);
            await LoadCharacteristics();
            await Update();
        }


        private async Task DiscoverService(KnownService knownService)
        {
            try
            {
                UserDialogsService.ShowLoading($"Discovering service {knownService.Id}...");
                var service = await _device.GetServiceAsync(knownService.Id);
                if (service == null)
                {
                    UserDialogsService.Toast($"Service not found: '{knownService}'", TimeSpan.FromSeconds(3));
                }
                else
                {
                    _service = service;
                }
            }
            catch (Exception ex)
            {
                await UserDialogsService.AlertAsync(ex.Message, "Error while discovering services");
                Trace.Message(ex.Message);
            }
            finally
            {
                UserDialogsService.HideLoading();
            }
        }

        private async Task LoadCharacteristics()
        {
            UserDialogsService.ShowLoading("Loading characteristics...");
            try
            {
                Characteristics = await _service.GetCharacteristicsAsync();
                UserDialogsService.HideLoading();
            }
            catch (Exception ex)
            {
                UserDialogsService.HideLoading();
                await UserDialogsService.AlertAsync(ex.Message);
            }
        }

        private async Task ReadValueAsync(ICharacteristic characteristic)
        {
            if (characteristic == null)
                return;

            try
            {
                UserDialogsService.ShowLoading("Reading characteristic value...");

                await characteristic.ReadAsync();

                Messages.Insert(0, $"Read value {CharacteristicValue}");
            }
            catch (Exception ex)
            {
                UserDialogsService.HideLoading();
                await UserDialogsService.AlertAsync(ex.Message);

                Messages.Insert(0, $"Error {ex.Message}");

            }
            finally
            {
                UserDialogsService.HideLoading();
            }

        }
        private async Task Updates()
        {
            if(!_updatesStarted)
            {
                await StartUpdates();
            }
            else
            {
                await StopUpdates();
            }
        }

        private async Task StartUpdates()
        {
            try
            {
                _updatesStarted = true;
                UpdateButtonText = "Выключить автообновление";

                Characteristics[1].ValueUpdated -= TemperatureOnValueUpdated;
                Characteristics[1].ValueUpdated += TemperatureOnValueUpdated;

                Characteristics[0].ValueUpdated -= HumidityOnValueUpdated;
                Characteristics[0].ValueUpdated += HumidityOnValueUpdated;

                await Characteristics[1].StartUpdatesAsync();
                await Characteristics[0].StartUpdatesAsync();

                Messages.Insert(0, $"Start updates");

            }
            catch (Exception ex)
            {
                await UserDialogsService.AlertAsync(ex.Message);
            }
        }

        private async Task StopUpdates()
        {
            try
            {
                if (!_updatesStarted)
                {
                    return;
                }

                _updatesStarted = false;
                UpdateButtonText = "Включить автообновление";

                await Characteristics[1].StopUpdatesAsync();
                Characteristics[1].ValueUpdated -= TemperatureOnValueUpdated;

                await Characteristics[0].StopUpdatesAsync();
                Characteristics[0].ValueUpdated -= HumidityOnValueUpdated;

                Messages.Insert(0, $"Temperature Stop updates");
                Messages.Insert(0, $"Humidity Stop updates");

            }
            catch (Exception ex)
            {
                await UserDialogsService.AlertAsync(ex.Message);
            }
        }

        private void TemperatureOnValueUpdated(object sender, CharacteristicUpdatedEventArgs characteristicUpdatedEventArgs)
        {
            Temperature = ConvertTemperature(characteristicUpdatedEventArgs.Characteristic.Value); 

            Device.InvokeOnMainThreadAsync(() =>
            {
                Messages.Insert(0, $"Temperature [{DateTime.Now.TimeOfDay}] - Updated: {Temperature}");
            });
        }
        private void HumidityOnValueUpdated(object sender, CharacteristicUpdatedEventArgs characteristicUpdatedEventArgs)
        {
            Humidity = characteristicUpdatedEventArgs.Characteristic.Value[0];
            Device.InvokeOnMainThreadAsync(() =>
            {
                Messages.Insert(0, $"Humidity [{DateTime.Now.TimeOfDay}] - Updated: {Humidity}");
            });
        }


    }
}
