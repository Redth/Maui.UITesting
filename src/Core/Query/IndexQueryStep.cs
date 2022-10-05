using Microsoft.Maui.Automation.Driver;

namespace Microsoft.Maui.Automation.Querying;

public class IndexQueryStep : PredicateQueryStep
{
	public IndexQueryStep(int index)
		: base()
	{
		Index = index;
	}

	public IndexQueryStep(int index, Predicate<IElement> predicate)
		: base(predicate)
	{
		Index = index;
	}

	public readonly int Index;

	public override Task<IEnumerable<IElement>> Execute(IDriver driver, IEnumerable<IElement> tree, IEnumerable<IElement> currentSet)
	{
		var newSet = new List<IElement>();

		try
		{
			newSet.Add(currentSet.ElementAt(Index));
		}
		catch { }

		return Task.FromResult<IEnumerable<IElement>>(newSet);
	}
}

