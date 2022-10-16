using Microsoft.Maui.Automation.Driver;

namespace Microsoft.Maui.Automation.Querying;

public class PredicateQueryStep : QueryStep
{
	public PredicateQueryStep(Predicate<IElement>? predicate = null, string? predicateDescription = null) : base()
	{
		Predicate = predicate ?? new Predicate<IElement>(e => true);
		PredicateDescription = predicate is null ? string.Empty : predicateDescription ?? "Expression";
	}

	public readonly Predicate<IElement> Predicate;
	public virtual string PredicateDescription { get; private set; }

	public override Task<IEnumerable<IElement>> Execute(IDriver driver, IEnumerable<IElement> tree, IEnumerable<IElement> currentSet)
		=> Task.FromResult(currentSet.Where(e => Predicate.Invoke(e)));

	public override string ToString()
		=> PredicateDescription;
}

