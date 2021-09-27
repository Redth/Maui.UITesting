using System.Collections.Generic;
using OpenQA.Selenium;

namespace Microsoft.Maui.WebDriver.Host
{
	public interface IPlatformElement : IWebElement
	{
		public string AutomationId { get; }
		public IEnumerable<IPlatformElement> Children { get; }
	}
}