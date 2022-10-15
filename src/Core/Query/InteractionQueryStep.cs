using Microsoft.Maui.Automation.Driver;

namespace Microsoft.Maui.Automation.Querying;

public class InteractionQueryStep : IQueryStep
{
	public InteractionQueryStep(Func<IDriver, IElement, Task> interaction)
	{
		Interaction = interaction;
	}

	public virtual TimeSpan DefaultPauseBeforeInteraction => TimeSpan.FromMilliseconds(300);
	public virtual TimeSpan DefaultPauseAfterInteraction => TimeSpan.FromMilliseconds(300);

	public readonly Func<IDriver, IElement, Task> Interaction;
	
	public async Task<IEnumerable<IElement>> Execute(IDriver driver, IEnumerable<IElement> tree, IEnumerable<IElement> currentSet)
	{
		var pauseBefore = driver.Configuration.Get(Query.ConfigurationKeys.PauseBeforeInteractions, DefaultPauseBeforeInteraction);
		var pauseAfter = driver.Configuration.Get(Query.ConfigurationKeys.PauseAfterInteractions, DefaultPauseAfterInteraction);

		foreach (var e in currentSet)
		{
			await Task.Delay(pauseBefore);
			await Interaction(driver, e);
			await Task.Delay(pauseAfter);
		}

		return currentSet;
	}
}


