namespace Microsoft.Maui.Automation.Driver
{

	public interface IDeviceInfo
	{
		ulong ScreenWidth { get; }
		ulong ScreenHeight { get; }
		double ScreenDensity { get; }

	}

	public class DeviceInfo : IDeviceInfo
	{
		public DeviceInfo()
		{
			ScreenWidth = 0;
			ScreenHeight = 0;
			ScreenDensity = 1;
		}

		public DeviceInfo(ulong width, ulong height, double density)
		{
			ScreenWidth = width;
			ScreenHeight = height;
			ScreenDensity = density;
		}

		public ulong ScreenWidth { get; }

		public ulong ScreenHeight { get; }

		public double ScreenDensity { get; }
	}
}