using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Microsoft.Maui.Automation
{
    public class RegularExpressionSelector : IElementSelector
    {
        public RegularExpressionSelector(string pattern, RegexOptions options = RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace)
        {
            Pattern = pattern;
            Options = options;
            Rx = new Regex(Pattern, Options);
        }

        [JsonProperty]
        public string Pattern { get; protected set; }
        
        [JsonProperty]
        public RegexOptions Options { get; protected set; }
        
        [JsonIgnore]
        public Regex Rx { get; protected set; }

        public bool Matches(IElement view)
            => view.Text != null && Rx.IsMatch(view.Text);
    }
}
