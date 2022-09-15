using Grpc.Core;
using Microsoft.Maui.Automation.RemoteGrpc;

namespace Microsoft.Maui.Automation
{
	public interface IApplication
	{
		public Platform DefaultPlatform { get; }

		public Task<IEnumerable<Element>> GetElements();

		public Task<IEnumerable<Element>> FindElements(Predicate<Element> matcher);

		public Task<string> GetProperty(string elementId, string propertyName);

		public Task<PerformActionResult> PerformAction(string action, string elementId, params string[] arguments);

		public Task<PerformActionResult> PerformAction(string action, params string[] arguments);
	}
}