using Microsoft.Maui.Automation.Driver;

namespace Microsoft.Maui.Automation.Querying;

class SiblingsQueryStep : PredicateQueryStep
{
	public SiblingsQueryStep() : base()
	{ }

	public SiblingsQueryStep(Predicate<IElement>? predicate = null) : base(predicate)
	{ }

	public override Task<IEnumerable<IElement>> Execute(IDriver driver, IEnumerable<IElement> tree, IEnumerable<IElement> currentSet)
	{
		var newSet = new List<IElement>();

		foreach (var currentSetElement in currentSet)
		{
			var parent = tree.Traverse(e => e.Id == currentSetElement.ParentId).FirstOrDefault();
			if (parent is null)
				continue;

			var siblings = parent?.Children.Where(c => !c.Id.Equals(currentSetElement.Id));
			if (siblings is not null && siblings.Any())
				newSet.AddRange(siblings);
		}

		return Task.FromResult<IEnumerable<IElement>>(newSet);
	}
}
