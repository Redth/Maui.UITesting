using Microsoft.Maui.Automation.Driver;

namespace Microsoft.Maui.Automation.Querying;

class FirstQueryStep : PredicateQueryStep
{
	public FirstQueryStep() : base()
	{ }

	public FirstQueryStep(Predicate<IElement>? predicate = null) : base(predicate)
	{ }

	public override Task<IEnumerable<IElement>> Execute(IDriver driver, IEnumerable<IElement> tree, IEnumerable<IElement> currentSet)
	{
		var first = currentSet.FirstOrDefault(e => Predicate(e));

		if (first is not null)
			return Task.FromResult<IEnumerable<IElement>>(new[] { first });
		else
			return Task.FromResult(Enumerable.Empty<IElement>());
	}
}

