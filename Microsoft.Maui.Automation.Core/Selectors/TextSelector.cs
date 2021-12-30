using Newtonsoft.Json;

namespace Microsoft.Maui.Automation
{
    public class TextSelector : IElementSelector
    {
        public TextSelector(string text, TextMatchRule rule = TextMatchRule.Contains, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            Text = text;
            Rule = rule;
            Comparison = comparison;
        }

        [JsonProperty]
        public StringComparison Comparison { get; protected set; }

        [JsonProperty]
        public TextMatchRule Rule { get; protected set; }

        [JsonProperty]
        public string Text { get; protected set; }

        public bool Matches(IElement view)
            => Rule switch
            {
                TextMatchRule.Contains => view.Text?.Contains(Text, Comparison) ?? false,
                TextMatchRule.StartsWith => view.Text?.StartsWith(Text, Comparison) ?? false,
                TextMatchRule.EndsWith => view.Text?.EndsWith(Text, Comparison) ?? false,
                TextMatchRule.Exact => view.Text?.Equals(Text, Comparison) ?? false,
                _ => view.Text?.Contains(Text, Comparison) ?? false
            };

        public enum TextMatchRule
        {
            Contains,
            StartsWith,
            EndsWith,
            Exact
        }
    }
}
