using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Microsoft.Maui.Automation;

public static class ElementExtensions
{

	//public static async Task<IElement?> FirstBy(this IApplication application, string propertyName, string pattern, bool isRegularExpression)
	//	=> (await application.FindIElements(e => e.PropertyMatches(propertyName, pattern, isRegularExpression))).FirstOrDefault();

	//public static async Task<IElement?> FirstById(this IApplication application, string id)
	//	=> (await application.FindIElements(e => e.Id.Equals(id))).FirstOrDefault();

	//public static async Task<IElement?> FirstByAutomationId(this IApplication application, string automationId)
	//	=> (await application.FindIElements(e => e.AutomationId.Equals(automationId))).FirstOrDefault();



	//public static IElement? FirstBy(this IEnumerable<IElement> elements, string propertyName, string pattern, bool isRegularExpression)
	//	=> IElements.Traverse(e => e.PropertyMatches(propertyName, pattern, isRegularExpression)).FirstOrDefault();

	//public static IElement? FirstById(this IEnumerable<IElement> elements, string id)
	//	=> elements.Traverse(e => e.Id.Equals(id)).FirstOrDefault();

	//public static IElement? FirstByAutomationId(this IEnumerable<IElement> elements, string automationId)
	//	=> elements.Traverse(e => e.AutomationId.Equals(automationId)).FirstOrDefault();



	//public static Task<IEnumerable<IElement>> By(this IApplication application, string propertyName, string pattern, bool isRegularExpression)
	//	=> application.FindElements(e => e.PropertyMatches(propertyName, pattern, isRegularExpression));

	//public static Task<IEnumerable<IElement>> ById(this IApplication application, string id)
	//	=> application.FindElements(e => e.Id.Equals(id));

	//public static Task<IEnumerable<IElement>> ByAutomationId(this IApplication application, string automationId)
	//	=> application.FindElements(e => e.AutomationId.Equals(automationId));


	//public static IEnumerable<IElement> By(this IEnumerable<IElement> elements, string propertyName, string pattern, bool isRegularExpression)
	//	=> elements.Traverse(e => e.PropertyMatches(propertyName, pattern, isRegularExpression));

	//public static IEnumerable<IElement> ById(this IEnumerable<IElement> elements, string id)
	//	=> elements.Traverse(e => e.Id.Equals(id));

	//public static IEnumerable<IElement> ByAutomationId(this IEnumerable<IElement> elements, string automationId)
	//	=> elements.Traverse(e => e.AutomationId.Equals(automationId));


	public static IEnumerable<IElement> Find(this IEnumerable<IElement> elements, Predicate<IElement> predicate)
		=> elements.Traverse(predicate);

	internal static IEnumerable<IElement> Traverse(this IEnumerable<IElement> elements, Predicate<IElement> predicate)
	{
		var matches = new List<IElement>();
	
		elements.Traverse(matches, predicate);

		return matches;
	}

	internal static void Traverse(this IEnumerable<IElement> source, IList<IElement> matches, Predicate<IElement> predicate)
	{
		foreach (var s in source)
		{
			if (predicate(s))
				matches.Add(s);

			Traverse(s.Children, matches, predicate);
		}
	}

	public static bool PropertyMatches(this IElement e, string propertyName, string pattern, bool isRegularExpression = false)
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

