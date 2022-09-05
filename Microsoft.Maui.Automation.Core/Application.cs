using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation
{
	public class ElementNotFoundException : Exception
	{
		public ElementNotFoundException(string elementId)
			: base($"Element with the ID: '{elementId}' was not found.")
		{
			ElementId = elementId;
		}

		public readonly string ElementId;
	}

	public abstract class Application : IApplication
	{
		public virtual void Close()
		{
		}

		~Application()
		{
			Dispose(false);
		}

		public virtual void Dispose()
		{
			Dispose(true);
		}

		bool disposed;
		void Dispose(bool disposing)
		{
			if (!disposed)
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

		public abstract Platform DefaultPlatform { get; }

		public abstract Task<string> GetProperty(Platform platform, string elementId, string propertyName);

		public abstract Task<IEnumerable<Element>> GetElements(Platform platform);

		public abstract Task<IEnumerable<Element>> FindElements(Platform platform, Func<Element, bool> matcher);

		public Task<PerformActionResult> PerformAction(Platform platform, string action, params string[] arguments)
			=> PerformAction(platform, action, string.Empty, arguments);

        public abstract Task<PerformActionResult> PerformAction(Platform platform, string action, string elementId, params string[] arguments);


    }
}
