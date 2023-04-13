using Temperature.Models;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Temperature.Services.BluetoothService
{
    public class BluetoothService : IBluetoothService
    {
        public  IAdapter _adapterService;
        public BluetoothService()
        {
            _adapterService =  CrossBluetoothLE.Current.Adapter;
        }

        public async Task ConnectToBluetoothDeviceAsync(IDevice device)
        {
            
            
        }

        public async Task<List<IDevice>> GetBluetoothDeviceAsync()
        {
            _adapterService= CrossBluetoothLE.Current.Adapter;
            List<IDevice> DeviceList = new List<IDevice>();
            await _adapterService.StartScanningForDevicesAsync();
            DeviceList = _adapterService.DiscoveredDevices.ToList();
            return DeviceList;
        }
    }
}