using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Temperature.Helpers;

[assembly: Dependency(typeof(Temperature.UWP.Helpers.PlatformHelpers))]
namespace Temperature.UWP.Helpers
{
    public class PlatformHelpers : IPlatformHelpers
    {
        public Task<PermissionStatus> CheckAndRequestBluetoothPermissions()
        {
            return Task.FromResult(PermissionStatus.Granted);
        }
    }
}
