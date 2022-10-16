using Microsoft.Maui.Automation.Driver;

namespace Microsoft.Maui.Automation.Querying;

public abstract class QueryStep : IQueryStep
{
	public abstract Task<IEnumerable<IElement>> Execute(IDriver driver, IEnumerable<IElement> tree, IEnumerable<IElement> currentSet);
}
