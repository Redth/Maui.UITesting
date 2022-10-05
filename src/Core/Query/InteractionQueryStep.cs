using Microsoft.Maui.Automation.Driver;

namespace Microsoft.Maui.Automation.Querying;

public class InteractionQueryStep : IQueryStep
{
	public InteractionQueryStep(Func<IDriver, IElement, Task> interaction)
	{
		Interaction = interaction;
	}

	public readonly Func<IDriver, IElement, Task> Interaction;
	
	public async Task<IEnumerable<IElement>> Execute(IDriver driver, IEnumerable<IElement> tree, IEnumerable<IElement> currentSet)
	{
		foreach (var e in currentSet)
			await Interaction(driver, e);

		return currentSet;
	}
}


