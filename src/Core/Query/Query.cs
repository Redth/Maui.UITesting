using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Maui.Automation.Driver;
using static System.Net.Mime.MediaTypeNames;

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
		QueryId = Guid.NewGuid().ToString();
	}

	public Query(Platform automationPlatform)
	{
		AutomationPlatform = automationPlatform;
		QueryId = Guid.NewGuid().ToString();
	}

	public readonly string QueryId;

	List<IQueryStep> steps = new();

	public static Query On(Platform automationPlatform)
		=> new Query(automationPlatform);

	static Query by(Predicate<IElement> predicate, string? predicateDescription = null)
		=> new Query().append(predicate, predicateDescription);

	public static Query By(Predicate<IElement> predicate)
		=> by(predicate, null);

    public static Query AutomationId(string automationId)
		=> by(e => e.AutomationId == automationId, $"AutomationId='{automationId}'");

	public static Query ById(string id)
		=> by(e => e.Id == id, $"Id='{id}'");

	public static Query Type(string type)
		=> by(e => e.Type == type, $"Type='{type}'");

	public static Query Type(string[] types)
		=> by(e => types.Any(t => t == e.Type), string.Join(" OR ", types.Select(t => $"Type='{t}'")));

	public static Query ContainingText(string text, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
		=> by(e => e.Text.Contains(text, comparisonType), $"$Text.Contains('{text}')");

	public static Query Marked(string marked, StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
		=> by(e => e.Id.Equals(marked, comparisonType)
			|| e.AutomationId.Equals(marked, comparisonType)
			|| e.Text.Equals(marked, comparisonType),
			$"Id='{marked}' OR AutomationId='{marked}' OR Text='{marked}'");

	public Query Append(IQueryStep step)
	{
		steps.Add(step);
		return this;
	}

	public Query Append(Predicate<IElement> predicate)
		=> append(predicate, null);

	internal Query append(Predicate<IElement> predicate, string? predicateDescription = null)
	{
		steps.Add(new PredicateQueryStep(predicate, predicateDescription));
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

	public override string ToString()
	{
		var s = new StringBuilder();
		s.Append($"Query(Id='{QueryId}'");

		if (AutomationPlatform is not null)
			s.Append($", AutomationPlatform={AutomationPlatform}");
		s.AppendLine(")");

		for (int i = 0; i < steps.Count; i++)
		{
			var step = steps[i];
			s.Append($"\t .{step.ToString()}");
			if (i < steps.Count - 1)
				s.AppendLine();
		}

		return s.ToString();
	}
}