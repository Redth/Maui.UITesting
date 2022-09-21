using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Microsoft.Maui.Automation;

public static class QueryExtensions
{
	public static Query ThenBy(this Query query, Predicate<Element> predicate)
		=> query.AndBy(predicate);
}

public class Query
{
	List<Predicate<Element>> predicates = new();

	public static Query By(Predicate<Element> predicate)
		=> new Query().AndBy(predicate);

	internal Query AndBy(Predicate<Element> predicate)
	{
		predicates.Add(predicate);
		return this;
	}

	public IEnumerable<Element> Execute(IEnumerable<Element> source)
	{
		var currentSet = new List<Element>();
		if (source.Any())
			currentSet.AddRange(source);

		foreach (var p in predicates)
		{
			var newSet = currentSet.Traverse(p).ToList();
			currentSet.Clear();

			if (newSet.Any())
				currentSet.AddRange(newSet);
			else
				break; // if there's no new set, break as we're empty
		}

		return currentSet;
	}
}

public static class ElementExtensions
{

	//public static async Task<Element?> FirstBy(this IApplication application, string propertyName, string pattern, bool isRegularExpression)
	//	=> (await application.FindElements(e => e.PropertyMatches(propertyName, pattern, isRegularExpression))).FirstOrDefault();

	//public static async Task<Element?> FirstById(this IApplication application, string id)
	//	=> (await application.FindElements(e => e.Id.Equals(id))).FirstOrDefault();

	//public static async Task<Element?> FirstByAutomationId(this IApplication application, string automationId)
	//	=> (await application.FindElements(e => e.AutomationId.Equals(automationId))).FirstOrDefault();



	//public static Element? FirstBy(this IEnumerable<Element> elements, string propertyName, string pattern, bool isRegularExpression)
	//	=> elements.Traverse(e => e.PropertyMatches(propertyName, pattern, isRegularExpression)).FirstOrDefault();

	//public static Element? FirstById(this IEnumerable<Element> elements, string id)
	//	=> elements.Traverse(e => e.Id.Equals(id)).FirstOrDefault();

	//public static Element? FirstByAutomationId(this IEnumerable<Element> elements, string automationId)
	//	=> elements.Traverse(e => e.AutomationId.Equals(automationId)).FirstOrDefault();



	//public static Task<IEnumerable<Element>> By(this IApplication application, string propertyName, string pattern, bool isRegularExpression)
	//	=> application.FindElements(e => e.PropertyMatches(propertyName, pattern, isRegularExpression));

	//public static Task<IEnumerable<Element>> ById(this IApplication application, string id)
	//	=> application.FindElements(e => e.Id.Equals(id));

	//public static Task<IEnumerable<Element>> ByAutomationId(this IApplication application, string automationId)
	//	=> application.FindElements(e => e.AutomationId.Equals(automationId));


	//public static IEnumerable<Element> By(this IEnumerable<Element> elements, string propertyName, string pattern, bool isRegularExpression)
	//	=> elements.Traverse(e => e.PropertyMatches(propertyName, pattern, isRegularExpression));

	//public static IEnumerable<Element> ById(this IEnumerable<Element> elements, string id)
	//	=> elements.Traverse(e => e.Id.Equals(id));

	//public static IEnumerable<Element> ByAutomationId(this IEnumerable<Element> elements, string automationId)
	//	=> elements.Traverse(e => e.AutomationId.Equals(automationId));


	public static IEnumerable<Element> Find(this IEnumerable<Element> elements, Predicate<Element> predicate)
		=> elements.Traverse(predicate);

	internal static IEnumerable<Element> Traverse(this IEnumerable<Element> elements, Predicate<Element> predicate)
	{
		var matches = new List<Element>();
	
		elements.Traverse(matches, predicate);

		return matches;
	}

	internal static void Traverse(this IEnumerable<Element> source, IList<Element> matches, Predicate<Element> predicate)
	{
		foreach (var s in source)
		{
			if (predicate(s))
				matches.Add(s);

			Traverse(s.Children, matches, predicate);
		}
	}

	public static bool PropertyMatches(this Element e, string propertyName, string pattern, bool isRegularExpression = false)
	{
		var value =
			propertyName.ToLowerInvariant() switch
			{
				"id" => e.Id,
				"automationid" => e.AutomationId,
				"text" => e.Text,
				"type" => e.Type,
				"fulltype" => e.FullType,
				_ => string.Empty
			} ?? string.Empty;

		return isRegularExpression ? Regex.IsMatch(value, pattern) : pattern.Equals(value);
	}
}

