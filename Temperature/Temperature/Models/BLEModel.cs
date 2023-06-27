using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Temperature.Models
{
    [AddINotifyPropertyChangedInterface]
    public class BLEModel
    {
        public IDevice Device { get; private set; }
        public Guid Id => Device.Id;
        public bool IsConnected => Device.State == DeviceState.Connected;
        public int Rssi => Device.Rssi;
        public string Name => Device.Name;

        private List<IService> _ListServices;
        public List<IService> ListServices
        {
            get
            {
                return _ListServices;
            }
            set
            {
                _ListServices = value;
            }
        }

        private List<ICharacteristic> _ListCharacteristics;
        public List<ICharacteristic> ListCharacteristics
        {
            get
            {
                return _ListCharacteristics;
            }
            set
            {
                _ListCharacteristics = value;
            }
        }

        public BLEModel(IDevice device)
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

    public class Grouping<K, T> : ObservableCollection<T>

    {
        public K Key { get; private set; }
        public Grouping(K key, IEnumerable<T> items) 
        {
            this.Key = key;
            foreach (var item in items) 
            {
                this.Add(item);
            }
        }

    }
}
