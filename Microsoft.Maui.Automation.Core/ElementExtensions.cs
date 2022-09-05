using System;
using System.Text.RegularExpressions;

namespace Microsoft.Maui.Automation;

public static class ElementExtensions
{

    public static Task<Element?> FirstBy(this IApplication application, string propertyName, string pattern, bool isRegularExpression)
        => application.FirstBy(application.DefaultPlatform, propertyName, pattern, isRegularExpression);

    public static Task<Element?> FirstById(this IApplication application, string id)
        => application.FirstById(application.DefaultPlatform, id);

    public static Task<Element?> FirstByAutomationId(this IApplication application, string automationId)
        => application.FirstByAutomationId(application.DefaultPlatform, automationId);


    public static async Task<Element?> FirstBy(this IApplication application, Platform platform, string propertyName, string pattern, bool isRegularExpression)
        => (await application.FindElements(platform, e => e.Matches(propertyName, pattern, isRegularExpression))).FirstOrDefault();

    public static async Task<Element?> FirstById(this IApplication application, Platform platform, string id)
        => (await application.FindElements(platform, e => e.Id.Equals(id))).FirstOrDefault();

    public static async Task<Element?> FirstByAutomationId(this IApplication application, Platform platform, string automationId)
        => (await application.FindElements(platform, e => e.AutomationId.Equals(automationId))).FirstOrDefault();





    public static Task<IEnumerable<Element>> By(this IApplication application, string propertyName, string pattern, bool isRegularExpression)
        => application.By(application.DefaultPlatform, propertyName, pattern, isRegularExpression);

    public static Task<IEnumerable<Element>> ById(this IApplication application, string id)
        => application.ById(application.DefaultPlatform, id);

    public static Task<IEnumerable<Element>> ByAutomationId(this IApplication application, string automationId)
        => application.ByAutomationId(application.DefaultPlatform, automationId);


    public static Task<IEnumerable<Element>> By(this IApplication application, Platform platform, string propertyName, string pattern, bool isRegularExpression)
        => application.FindElements(platform, e => e.Matches(propertyName, pattern, isRegularExpression));

    public static Task<IEnumerable<Element>> ById(this IApplication application, Platform platform, string id)
        => application.FindElements(platform, e => e.Id.Equals(id));

    public static Task<IEnumerable<Element>> ByAutomationId(this IApplication application, Platform platform, string automationId)
        => application.FindElements(platform, e => e.AutomationId.Equals(automationId));


    public static IEnumerable<Element> By(this IEnumerable<Element> elements, string propertyName, string pattern, bool isRegularExpression)
        => elements.Traverse(e => e.Matches(propertyName, pattern, isRegularExpression));

    public static IEnumerable<Element> ById(this IEnumerable<Element> elements, string id)
        => elements.Traverse(e => e.Id.Equals(id));

    public static IEnumerable<Element> ByAutomationId(this IEnumerable<Element> elements, string automationId)
        => elements.Traverse(e => e.AutomationId.Equals(automationId));


    public static IEnumerable<Element> Find(this IEnumerable<Element> elements, Func<Element, bool> matcher)
		=> elements.Traverse(matcher);

    static IEnumerable<Element> Traverse(this IEnumerable<Element> elements, Func<Element, bool> matcher)
    {
        var matches = new List<Element>();

        elements.Traverse(matches, matcher);

        return matches;
    }

    static void Traverse(this IEnumerable<Element> source, IList<Element> matches, Func<Element, bool> matcher)
	{
		foreach (var s in source)
		{
			if (matcher(s))
				matches.Add(s);

			Traverse(s.Children, matches, matcher);
		}
	}

    internal static bool Matches(this Element e, string propertyName, string pattern, bool isRegularExpression = false)
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

