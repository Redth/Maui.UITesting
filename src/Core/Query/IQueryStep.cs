using Microsoft.Maui.Automation.Driver;

namespace Microsoft.Maui.Automation.Querying;

public interface IQueryStep
{
	Task<IEnumerable<IElement>> Execute(IDriver driver, IEnumerable<IElement> tree, IEnumerable<IElement> currentSet);
}

