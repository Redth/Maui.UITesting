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

			configuration = new AutomationConfiguration(
				Platform.Maccatalyst, automationPlatform: Platform.Maui);

			driver = new Driver.AppDriver(configuration);
		}

		readonly IAutomationConfiguration configuration;
		readonly AppDriver driver;

		[Fact]
		public async Task RunApp()
		{
			var appId = "com.companyname.samplemauiapp";
			var file = "/Users/redth/code/Maui.VirtualListView/Sample/VirtualListViewSample/bin/Debug/net6.0-maccatalyst/maccatalyst-x64/VirtualListViewSample.app";


            await driver.InstallApp(file, appId);

			//await driver.InstallApp(@"C:\code\Maui.UITesting\samples\SampleMauiApp\bin\Debug\net6.0-android\com.companyname.samplemauiapp-Signed.apk", appId);
			await driver.LaunchApp(appId);

			var elements = await driver.FindElements(Platform.Maui, "AutomationId", "buttonOne");

			var e = elements.FirstOrDefault();

		}
	}
}
