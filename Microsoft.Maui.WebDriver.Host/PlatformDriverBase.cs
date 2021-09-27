using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

namespace Microsoft.Maui.WebDriver.Host
{
    // All the code in this file is included in all platforms.
    public abstract class PlatformDriverBase : IWebDriver, IFindsById, IFindsByClassName, IFindsByName, IFindsByTagName
	{
		
		public string Url { get => "app://"; set { } }

		public string Title => string.Empty;

		public string PageSource => null;

		public string CurrentWindowHandle => "Root";

		public ReadOnlyCollection<string> WindowHandles { get; }
			= new (new List<string> { "Root" });

		public virtual void Close()
		{
		}

		public virtual void Dispose()
		{
		}

		public virtual IWebElement FindElement(By by)
			=> by.FindElement(this);

		public virtual IWebElement FindElementByClassName(string className)
		{
			throw new NotImplementedException();
		}

		public virtual IWebElement FindElementById(string id)
			=> FindOne(v => v.AutomationId.Equals(id));

		public virtual IWebElement FindElementByName(string name)
		{
			throw new NotImplementedException();
		}

		public virtual IWebElement FindElementByTagName(string tagName)
		{
			throw new NotImplementedException();
		}

		public virtual ReadOnlyCollection<IWebElement> FindElements(By by)
		{
			throw new NotImplementedException();
		}

		public virtual ReadOnlyCollection<IWebElement> FindElementsByClassName(string className)
		{
			throw new NotImplementedException();
		}

		public virtual ReadOnlyCollection<IWebElement> FindElementsById(string id)
		{
			throw new NotImplementedException();
		}

		public virtual ReadOnlyCollection<IWebElement> FindElementsByName(string name)
		{
			throw new NotImplementedException();
		}

		public virtual ReadOnlyCollection<IWebElement> FindElementsByTagName(string tagName)
		{
			throw new NotImplementedException();
		}

		public virtual IOptions Manage()
		{
			throw new NotImplementedException();
		}

		public virtual INavigation Navigate()
		{
			throw new NotImplementedException();
		}

		public virtual void Quit()
		{
			throw new NotImplementedException();
		}

		public virtual ITargetLocator SwitchTo()
		{
			throw new NotImplementedException();
		}


		public abstract IPlatformElement[] GetViews();


		public virtual IEnumerable<IPlatformElement> Find(Func<IPlatformElement, bool> selector)
			=> Find(selector, GetViews(), false);

		public virtual IPlatformElement FindOne(Func<IPlatformElement, bool> selector)
			=> Find(selector, GetViews(), true)?.FirstOrDefault();

		protected virtual IEnumerable<IPlatformElement> Find(Func<IPlatformElement, bool> selector, IPlatformElement[] views, bool onlyFirst)
        {
			foreach (var v in views)
			{
				if (selector(v))
				{
					yield return v;

					if (onlyFirst)
						break;
				}

				if (v.Children?.Any() ?? false)
					Find(selector, v.Children, onlyFirst);
			}
		}
	}
}