using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Temperature.Helpers
{
	public static class PermissionHelpers
	{
		public static async Task<bool> IsGranted<TPermission>() where TPermission : Xamarin.Essentials.Permissions.BasePermission, new()
		{
			var result = await Xamarin.Essentials.Permissions.CheckStatusAsync<TPermission>();
			return result == Xamarin.Essentials.PermissionStatus.Granted;
		}

		public static async Task<Xamarin.Essentials.PermissionStatus> RequestIfNeeded<TPermission>() where TPermission : Xamarin.Essentials.Permissions.BasePermission, new()
		{
			if (await IsGranted<TPermission>())
			{
				return Xamarin.Essentials.PermissionStatus.Granted;
			}

			var status = Xamarin.Essentials.PermissionStatus.Unknown;
			try
			{
				status = await Xamarin.Essentials.Permissions.RequestAsync<TPermission>();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString());
			}

			return status;
		}
	}
}
