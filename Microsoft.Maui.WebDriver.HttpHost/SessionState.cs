using OpenQA.Selenium;

namespace Microsoft.Maui.WebDriver.HttpHost
{
    internal class SessionState
    {
		public string Id {  get; set; }

		public IWebDriver Driver {  get; set; }

		public ITimeouts Timeouts { get; set; } = new SessionTimouts();
    }


}
