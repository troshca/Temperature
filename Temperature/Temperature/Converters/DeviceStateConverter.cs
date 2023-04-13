using Plugin.BLE.Abstractions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Temperature.Converters
{
    public class DeviceStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string deviceState = "N/A";
            if (value is null)
            {
                return deviceState;
            }
            if (value is DeviceState)
            {
             
                switch (value)
                {
                    case DeviceState.Connected:
                        {
                            deviceState = "Connected";
                            break;
                        }
                    case DeviceState.Disconnected:
                        {
                            deviceState = "Disconnected";
                            break;
                        }
                    case DeviceState.Connecting:
                        {
                            deviceState = "Connecting";
                            break;
                        }
                    case DeviceState.Limited:
                        {
                            deviceState = "Limited";
                            break;
                        }
                    default:
                        deviceState = "N/A";
                        break;
                }
                
            }
            return deviceState;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
