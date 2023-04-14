using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Temperature.Helpers
{
    public interface IPlatformHelpers
    {
        Task<PermissionStatus> CheckAndRequestBluetoothPermissions();
    }
}
