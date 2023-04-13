using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Text;

namespace Temperature.Models
{
    [AddINotifyPropertyChangedInterface]
    public class BluetoothDeviceDetail
    {
        public IDevice Device { get; private set; }
        public Guid Id => Device.Id;
        public bool IsConnected => Device.State == DeviceState.Connected;
        public int Rssi => Device.Rssi;
        public string Name => Device.Name;
        public BluetoothDeviceDetail(IDevice device)
        {
            Device = device;
        }
        public void Update(IDevice newDevice = null)
        {
            if (newDevice != null)
            {
                Device = newDevice;
            }
        }

    }
}
