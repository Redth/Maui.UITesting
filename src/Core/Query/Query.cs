using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Maui.Automation.Driver;

namespace Microsoft.Maui.Automation.Querying;

public class Query
{
	public static class ConfigurationKeys
	{
		public const string PauseBeforeInteractions = "QUERY_PAUSE_BEFORE_INTERACTIONS";
		public const string PauseAfterInteractions = "QUERY_PAUSE_AFTER_INTERACTIONS";
	}

	public static ILogger Logger { get; set; } = NullLogger.Instance;

	public Query()
	{
		Id = Guid.NewGuid().ToString();
	}

	public Query(Platform automationPlatform)
	{
		AutomationPlatform = automationPlatform;
		Id = Guid.NewGuid().ToString();
	}

	public readonly string Id;

	List<IQueryStep> steps = new();

	public static Query On(Platform automationPlatform)
		=> new Query(automationPlatform);

	public static Query By(Predicate<IElement> predicate)
		=> new Query().Append(predicate);

	public static Query ByAutomationId(string automationId)
		=> By(e => e.AutomationId == automationId);

	public static Query ById(string id)
		=> By(e => e.Id == id);

	public static Query OfType(string type)
		=> By(e => e.Type == type);

	public static Query ContainingText(string text, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
		=> By(e => e.Text.Contains(text, comparisonType));

	public static Query Marked(string marked, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
		=> By(e => e.Id.Equals(marked, comparisonType)
			|| e.AutomationId.Equals(marked, comparisonType)
			|| e.Text.Equals(marked, comparisonType));

	public Query Append(IQueryStep step)
	{
		steps.Add(step);
		return this;
	}

	public Query Append(Predicate<IElement> predicate)
	{
		steps.Add(new PredicateQueryStep(predicate));
		return this;
	}

	public Platform? AutomationPlatform { get; private set; }

	public async Task<IEnumerable<IElement>> Execute(IDriver driver, IEnumerable<IElement> source)
	{
		// Keep the original tree
		var tree = source.ToList();

		var previousSet = new List<IElement>();

		var currentSet = new List<IElement>();
		if (source.Any())
			currentSet.AddRange(source);

		foreach (var step in steps)
		{
			previousSet.Clear();
			previousSet.AddRange(currentSet);

			var newSet = (await step.Execute(driver, tree, currentSet)
				.ConfigureAwait(false)).ToList();

			currentSet.Clear();

			if (newSet is not null && newSet.Any())
				currentSet.AddRange(newSet);
			else
				break; // if there's no new set, break as we're empty
		}

		return currentSet;
	}

}