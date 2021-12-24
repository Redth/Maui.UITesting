using System.Collections.Generic;

namespace Microsoft.Maui.Automation
{
    public interface IView : IElement
	{
		public string WindowId { get; }
		public bool Visible { get; }

		public bool Enabled { get; }
		public bool Focused { get; }

		public int X { get; }
		public int Y { get; }
	}
}
