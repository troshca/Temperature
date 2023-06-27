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

namespace Temperature.ViewModels
{
    public class ServiceListPageViewModel : ViewModelBase
    {
        readonly IAdapter _adapterService;
        readonly IBluetoothLE _bluetoothLE;
        public BLEModel bLEModel { get; set; }

        private IDevice _device;

        private IService _service;
        public IReadOnlyList<IService> Services { get; private set; }

        private IReadOnlyList<ICharacteristic> _characteristics;
        public IReadOnlyList<ICharacteristic> Characteristics
        {
            get => _characteristics;
            private set => SetProperty(ref _characteristics, value);
        }
        public List<IReadOnlyList<ICharacteristic>> ListOfAllCharacteristics { get; private set; }

        private DelegateCommand _DiscoverAllServicesCommand;
        private DelegateCommand<KnownService> _DiscoverServiceByIdCommand;


        public DelegateCommand DiscoverAllServicesCommand =>
            _DiscoverAllServicesCommand ?? (_DiscoverAllServicesCommand = new DelegateCommand(async () => await DiscoverServices()));

        public DelegateCommand<KnownService> DiscoverServiceByIdCommand => _DiscoverServiceByIdCommand;

        public ServiceListPageViewModel(INavigationService navigationService, IUserDialogs userDialogsService) : base(navigationService, userDialogsService)
        {
            _adapterService = CrossBluetoothLE.Current.Adapter;
            _bluetoothLE = CrossBluetoothLE.Current;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            _device = parameters.GetValue<IDevice>("device");
            bLEModel = new BLEModel(_device);
        }
        
        public ObservableCollection<Grouping<IService, ICharacteristic>> AssetsList { get; set; }

        //ObservableCollection<Grouping<IService, ICharacteristic>> AssetsList  = new ObservableCollection<Grouping<IService, ICharacteristic>>();

        private async Task DiscoverServices()
        {
           
            try
            {
                UserDialogsService.ShowLoading("Discovering services...");
                //ListOfAllCharacteristicsInServices<IReadOnlyList<IService>, IReadOnlyList<ICharacteristic>> sdfjb;
                Services = await _device.GetServicesAsync();
                bLEModel.ListServices = new List<IService>();
                bLEModel.ListCharacteristics = new List<ICharacteristic>();
                ListOfAllCharacteristics = new List<IReadOnlyList<ICharacteristic>>();
                bLEModel.ListServices.Clear();
                AssetsList = new ObservableCollection<Grouping<IService, ICharacteristic>>();
                for (int i =0; i < Services.Count(); i++) 
                {
                    var group = new Grouping<IService, ICharacteristic>(Services[i], await Services[i].GetCharacteristicsAsync());
                    AssetsList.Add(group);
                    bLEModel.ListServices.Add(Services[i]);
                    ListOfAllCharacteristics.Add( await Services[i].GetCharacteristicsAsync());

                }


                //bLEModel.ListServices = (List<IService>)Services;
            }
            catch (Exception ex)
            {
                await UserDialogsService.AlertAsync(ex.Message, "Error while discovering services");
            }
            finally
            {
                UserDialogsService.HideLoading();
            }
        }

        private async Task DiscoverService(KnownService knownService)
        {
            try
            {
                UserDialogsService.ShowLoading($"Discovering service {knownService.Id}...");

                var service = await _device.GetServiceAsync(knownService.Id);

                Services = service != null ? new List<IService> { service } : new List<IService>();
                //await RaisePropertyChanged(nameof(Services));

                if (service == null)
                {
                    UserDialogsService.Toast($"Service not found: '{knownService}'", TimeSpan.FromSeconds(3));
                }
            }
            catch (Exception ex)
            {
                await UserDialogsService.AlertAsync(ex.Message, "Error while discovering services");
            }
            finally
            {
                UserDialogsService.HideLoading();
            }
        }

        public IService SelectedService
        {
            get => null;
            set
            {
                if (value != null)
                {
                    //var bundle = new MvxBundle(new Dictionary<string, string>(Bundle.Data) { { ServiceIdKey, value.Id.ToString() } });
                    //_navigation.Navigate<CharacteristicListViewModel, MvxBundle>(bundle);
                }

                //RaisePropertyChanged();

            }
        }


    }
}
