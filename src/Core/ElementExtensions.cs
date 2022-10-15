using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Microsoft.Maui.Automation;

public static class ElementExtensions
{
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

