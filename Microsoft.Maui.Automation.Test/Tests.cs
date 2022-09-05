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
		public Tests()
		{
			driver = new Driver.AndroidDriver(new AutomationConfiguration());
		}

		readonly Microsoft.Maui.Automation.Driver.AndroidDriver driver;

		[Fact]
		public async Task RunApp()
		{
			var appId = "com.companyname.samplemauiapp";

			//await driver.InstallApp(@"C:\code\Maui.UITesting\samples\SampleMauiApp\bin\Debug\net6.0-android\com.companyname.samplemauiapp-Signed.apk", appId);
			await driver.LaunchApp(appId);

			var elements = await driver.FindElements(Platform.Maui, "AutomationId", "buttonOne");

			var e = elements.FirstOrDefault();

		}
	}
}
