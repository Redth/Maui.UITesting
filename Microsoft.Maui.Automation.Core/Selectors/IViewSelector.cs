using Newtonsoft.Json;

namespace Microsoft.Maui.Automation
{
    public interface IViewSelector
    {
		public bool Matches(IView element);
    }
}
