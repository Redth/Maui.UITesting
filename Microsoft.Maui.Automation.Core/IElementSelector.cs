using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Microsoft.Maui.Automation
{
    public interface IElementSelector
    {
		public bool Matches(IView element);
    }

    public class MultiSelector : IElementSelector
    {
        public MultiSelector(params IElementSelector[] elementSelectors)
        {
            Selectors = elementSelectors;
            Any = false;
        }

        public MultiSelector(bool any, params IElementSelector[] elementSelectors)
        {
            Selectors = elementSelectors;
            Any = any;
        }

        public readonly IElementSelector[] Selectors;
        public readonly bool Any;

        public bool Matches(IView element)
        {
            foreach (var s in Selectors)
            {
                var isMatch = s.Matches(element);

                // If looking for any to match, and we found a match, return true
                if (Any && isMatch)
                    return true;

                // If we want ALL to match and we found a non-match, return false
                if (!Any && !isMatch)
                    return false;
            }

            // If we get here, we went through all the selectors and all did or all did not match
            // Otherwise we would have short circuited out the loop earlier
            // If we were looking for ANY, we had no matches, return false
            // If we are looking for ALL, we had all matches, return true
            return !Any;
        }
    }

    public class TypeSelector : IElementSelector
    {
        public TypeSelector(string typeName)
        {
            TypeName = typeName;
        }

        public TypeSelector(Type type)
        {
           TypeName = type.Name;
        }

        public readonly string TypeName;

        public bool Matches(IView element)
            => element.Type.Equals(TypeName);
    }

    public class RegularExpressionSelector : IElementSelector
    {
        public RegularExpressionSelector(string pattern, RegexOptions options = RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace)
        {
            Pattern = pattern;
            Options = options;
            Rx = new Regex(Pattern, Options);
        }

        public readonly string Pattern;
        public readonly RegexOptions Options;
        public readonly Regex Rx;

        public bool Matches(IView element)
            => Rx.IsMatch(element.Text);
    }

    public class TextSelector : IElementSelector
    {
        public TextSelector(string text, TextMatchRule rule = TextMatchRule.Contains, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            Text = text;
            Rule = rule;
            Comparison = comparison;
        }

        public readonly StringComparison Comparison;
        public readonly TextMatchRule Rule;
        public readonly string Text;

        public bool Matches(IView element)
        => Rule switch
        {
            TextMatchRule.Contains => element.Text?.Contains(Text, Comparison) ?? false,
            TextMatchRule.StartsWith => element.Text?.StartsWith(Text, Comparison) ?? false,
            TextMatchRule.EndsWith => element.Text?.EndsWith(Text, Comparison) ?? false,
            TextMatchRule.Exact => element.Text?.Equals(Text, Comparison) ?? false,
            _ => element.Text?.Contains(Text, Comparison) ?? false
        };

        public enum TextMatchRule
        {
            Contains,
            StartsWith,
            EndsWith,
            Exact
        }
    }

    public class AutomationIdSelector : IElementSelector
    {
        public AutomationIdSelector(string automationId, StringComparison comparison = StringComparison.Ordinal)
        {
            AutomationId = automationId;
            Comparison = comparison;
        }

        public readonly string AutomationId;
        public readonly StringComparison Comparison;

        public bool Matches(IView element)
            => element?.AutomationId?.Equals(AutomationId, Comparison) ?? false;
    }

    public class IdSelector : IElementSelector
    {
        public IdSelector(string id, StringComparison comparison = StringComparison.Ordinal)
        {
            Id = id;
            Comparison = comparison;
        }

        public readonly string Id;
        public readonly StringComparison Comparison;

        public bool Matches(IView element)
            => element?.AutomationId?.Equals(Id, Comparison) ?? false;
    }
}
