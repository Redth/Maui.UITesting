using OpenQA.Selenium;

namespace Microsoft.Maui.WebDriver.Host
{
	public interface IPlatformElement : IWebElement
	{
		public string AutomationId { get; }
		public IPlatformElement[] Children { get; }
	}
}