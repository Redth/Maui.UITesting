using Grpc.Core;
using Microsoft.Maui.Automation.RemoteGrpc;

namespace Microsoft.Maui.Automation
{
	public interface IApplication
	{
		public Platform DefaultPlatform { get; }

		public Task<IEnumerable<IElement>> GetElements();

		public Task<IEnumerable<IElement>> FindElements(Predicate<IElement> matcher);

		public Task<string> GetProperty(string elementId, string propertyName);

		public Task<PerformActionResult> PerformAction(string action, string elementId, params string[] arguments);

		public Task<PerformActionResult> PerformAction(string action, params string[] arguments);

		public Task<string[]> Backdoor(string fullyQualifiedTypeName, string staticMethodName, string[] args);
	}
}