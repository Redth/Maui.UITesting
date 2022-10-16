using Microsoft.Maui.Automation.Driver;

namespace Microsoft.Maui.Automation.Querying;

public class IndexQueryStep : QueryStep
{
	public IndexQueryStep(int index)
		: base()
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

	public override string ToString()
		=> $"Index({Index})";
}

