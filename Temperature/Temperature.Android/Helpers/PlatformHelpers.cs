using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Temperature.Helpers;

[assembly: Dependency(typeof(Temperature.Droid.Helpers.PlatformHelpers))]
namespace Temperature.Droid.Helpers
{
    public class PlatformHelpers : IPlatformHelpers
    {
        public async Task<PermissionStatus> CheckAndRequestBluetoothPermissions()
        {
            PermissionStatus statusBLE;
            PermissionStatus statusLocation;

            statusBLE = await Permissions.CheckStatusAsync<BluetoothSPermission>();

            if (statusBLE != PermissionStatus.Granted)
                statusBLE = await Permissions.RequestAsync<BluetoothSPermission>();

            statusLocation = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();


            if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>())
            {
                await Application.Current.MainPage.DisplayAlert("Permission required", "Location permission is required for bluetooth scanning. We do not store or use your location at all.", "OK");
            }

            statusLocation = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            if (statusLocation == PermissionStatus.Granted && statusBLE == PermissionStatus.Granted)
                return PermissionStatus.Granted;
            return PermissionStatus.Denied;

        }
    }

    public class BluetoothSPermission : Xamarin.Essentials.Permissions.BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions => new List<(string androidPermission, bool isRuntime)>
        {
            (Android.Manifest.Permission.BluetoothScan, true),
            (Android.Manifest.Permission.BluetoothConnect, true)
        }.ToArray();
    }
}
