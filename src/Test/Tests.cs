using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Automation.Driver;
using Xunit;

namespace Microsoft.Maui.Automation.Test
{
	public class Tests
	{
		readonly IDriver driver;

		public Tests()
		{
			driver = new AppDriverBuilder()
				.AppId("com.companyname.samplemauiapp")
				.AppFilename("C:\\code\\Maui.UITesting\\samples\\SampleMauiApp\\bin\\Debug\\net6.0-android\\com.companyname.samplemauiapp-Signed.apk")
				.DevicePlatform(Platform.Android)
				.AutomationPlatform(Platform.Maui)
				.Device("emulator-5554")
				.Build();
		}

		[Fact]
		public async Task RunApp()
		{
			// Install and launch the app
			await driver.Start();

			// Find the button by its MAUI AutomationId property
			var button = await driver.First(By.AutomationId("buttonOne"));
			Assert.NotNull(button);

			// Tap the button to increment the counter
			await driver.Tap(button);

			// Find the label we expect to have changed
			var label = await driver.First(e =>
				e.Type == "Label"
				&& e.Text.Contains("1"));

			Assert.NotNull(label);
		}
	}
}
