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
		// for the purposes of testing in Maui we're going to make the following mappings
		// Selenium		Here
		// Id			Automation Id
		// TagName		(UI Element).GetType().Name
		// ClassName	(UI Element).GetType().Name
		// Name			Text field of UI element

		public string Url { get => "app://"; set { } }

		public string Title => string.Empty;

		public string PageSource => null;

		public string CurrentWindowHandle => "Root";

		public ReadOnlyCollection<string> WindowHandles { get; }
			= new(new List<string> { "Root" });

		public virtual void Close()
		{
		}

		~PlatformDriverBase ()
        {
			Dispose(false);
        }

		public virtual void Dispose()
		{
			Dispose(true);
		}

		bool disposed;
		void Dispose (bool disposing)
        {
			if (!disposing)
            {
				if (disposing)
					DisposeManagedResources();
				DisposeUnmanagedResources();
				disposed = true;
            }
        }

		protected virtual void DisposeManagedResources()
        {
        }

		protected virtual void DisposeUnmanagedResources()
        {
        }

		public virtual IWebElement FindElement(By by)
			=> by.FindElement(this);

		public virtual IWebElement FindElementByClassName(string className)
			=> FindElementsByClassName(className).FirstOrDefault();

		public virtual IWebElement FindElementById(string id)
			=> Find(v => v.AutomationId.Equals(id)).FirstOrDefault();

		public virtual IWebElement FindElementByName(string name)
		{
			throw new NotImplementedException();
		}

		public virtual IWebElement FindElementByTagName(string tagName)
			=> FindElementsByTagName(tagName).FirstOrDefault();

		public virtual ReadOnlyCollection<IWebElement> FindElements(By by)
			=> by.FindElements(this);

		public virtual ReadOnlyCollection<IWebElement> FindElementsByClassName(string className)
			=> Find(elem => elem.GetType().Name == className).Cast<IWebElement>().ToReadOnlyCollection();

		public virtual ReadOnlyCollection<IWebElement> FindElementsById(string id)
			=> Find(elem => elem.AutomationId == id).Cast<IWebElement>().ToReadOnlyCollection();

		public virtual ReadOnlyCollection<IWebElement> FindElementsByName(string name)
		{
			throw new NotImplementedException();
		}

		public virtual ReadOnlyCollection<IWebElement> FindElementsByTagName(string tagName)
			=> Find(elem => elem.TagName == tagName).Cast<IWebElement>().ToReadOnlyCollection();

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


		public abstract IEnumerable<IPlatformElement> Views
		{
			get;
		}


		public virtual IEnumerable<IPlatformElement> Find(Func<IPlatformElement, bool> selector)
			=> FindDepthFirst(selector, Views);

		protected static IEnumerable<IPlatformElement> FindDepthFirst(Func<IPlatformElement, bool> selector, IEnumerable<IPlatformElement> views)
		{
			var st = new Stack<IPlatformElement>();
			st.PushAllReverse(views);
			while (st.Count > 0)
			{
				var v = st.Pop();
				if (selector(v))
				{
					yield return v;
				}
				st.PushAllReverse(v.Children);
			}
		}

		protected static IEnumerable<IPlatformElement> FindBreadthFirst(Func<IPlatformElement, bool> selector, IEnumerable<IPlatformElement> views)
		{
			var q = new Queue<IPlatformElement>();
			q.EnqueAll(views);
			while (q.Count > 0)
			{
				var v = q.Dequeue();
				if (selector(v))
				{
					yield return v;
				}
				q.EnqueAll(v.Children);
			}
		}
	}
}
