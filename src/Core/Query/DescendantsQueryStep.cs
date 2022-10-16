using Microsoft.Maui.Automation.Driver;

namespace Microsoft.Maui.Automation.Querying;

public class DescendantsQueryStep : PredicateQueryStep
{
	public DescendantsQueryStep(Predicate<IElement>? predicate, string? predicateDescription = null)
		: base(predicate, predicateDescription)
	{
	}

	public DescendantsQueryStep() : base()
	{
	}

	public override Task<IEnumerable<IElement>> Execute(IDriver driver, IEnumerable<IElement> tree, IEnumerable<IElement> currentSet)
		=> Task.FromResult(currentSet.Traverse(Predicate));

	public override string ToString()
		=> $"Descendants({base.ToString()})";
}

