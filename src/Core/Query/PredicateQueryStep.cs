using Microsoft.Maui.Automation.Driver;

namespace Microsoft.Maui.Automation.Querying;

public class PredicateQueryStep : QueryStep
{
	public PredicateQueryStep(Predicate<IElement>? predicate = null)
	{
		Predicate = predicate ?? new Predicate<IElement>(e => true);
	}

	public readonly Predicate<IElement> Predicate;

	public override Task<IEnumerable<IElement>> Execute(IDriver driver, IEnumerable<IElement> tree, IEnumerable<IElement> currentSet)
		=> Task.FromResult(currentSet.Where(e => Predicate.Invoke(e)));
}

