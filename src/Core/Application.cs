using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Automation.Querying;

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

	public class ElementsNotFoundException : Exception
	{
		public ElementsNotFoundException(Query query)
			: base($"Elements not found matching query")
		{
			Query = query;
		}

		public readonly Query Query;
	}

	public class ElementsStillFoundException : Exception
	{
		public ElementsStillFoundException(Query query)
			: base($"Elements matching query are still found")
		{
			Query = query;
		}

		public readonly Query Query;
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

		public abstract Task<string> GetProperty(string elementId, string propertyName);

		public abstract Task<IEnumerable<IElement>> GetElements();

		public abstract Task<IEnumerable<IElement>> FindElements(Predicate<IElement> matcher);

		public Task<PerformActionResult> PerformAction(string action, params string[] arguments)
			=> PerformAction(action, string.Empty, arguments);

		public abstract Task<PerformActionResult> PerformAction(string action, string elementId, params string[] arguments);

		public Task<string[]> Backdoor(string fullyQualifiedTypeName, string staticMethodName, string[] args)
		{
			var t = Type.GetType(fullyQualifiedTypeName);
			var m = t?.GetMethod(staticMethodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
			var result = m?.Invoke(null, args);
			return Task.FromResult(result as string[] ?? new string[0] );
		}

	}
}
