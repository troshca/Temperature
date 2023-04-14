using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Temperature;
using Temperature.Helpers;

[assembly: Dependency(typeof(Temperature.iOS.Helpers.PlatformHelpers))]
namespace Temperature.iOS.Helpers
{
    public class PlatformHelpers : IPlatformHelpers
    {
        public Task<PermissionStatus> CheckAndRequestBluetoothPermissions()
        {
            return Task.FromResult(PermissionStatus.Granted);
        }
    }
}
