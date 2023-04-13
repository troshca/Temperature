using Temperature.Models;
using Plugin.BLE.Abstractions.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Temperature.Services.BluetoothService
{
    public interface IBluetoothService
    {
        Task<List<IDevice>> GetBluetoothDeviceAsync();
        Task ConnectToBluetoothDeviceAsync(IDevice device);
    }
}