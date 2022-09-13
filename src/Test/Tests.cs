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
		readonly AppDriver driver;

		public Tests()
		{
			driver = new AppDriver(
				new AutomationConfiguration(
					"com.companyname.samplemauiapp",
					"C:\\code\\Maui.UITesting\\samples\\SampleMauiApp\\bin\\Debug\\net6.0-android\\com.companyname.samplemauiapp-Signed.apk",
					Platform.Android,
					automationPlatform: Platform.Maui,
					device: "emulator-5554"));
		}

		[Fact]
		public async Task RunApp()
		{
			// Install and launch the app
			await driver.InstallApp();
			await driver.LaunchApp();

			// Find the button by its MAUI AutomationId property
			var button = await driver.FirstByAutomationId("buttonOne");
			Assert.NotNull(button);

			// Tap the button to increment the counter
			await driver.Tap(button);

			// Find the label we expect to have changed
			var label = await driver.By(e =>
				e.Type == "Label"
				&& e.Text.Contains("1"));

			Assert.NotEmpty(label);
		}
	}
}
