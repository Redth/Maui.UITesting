using System.Text.RegularExpressions;

namespace Microsoft.Maui.Automation
{
    public class RegularExpressionSelector : IViewSelector
    {
        public RegularExpressionSelector(string pattern, RegexOptions options = RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace)
        {
            Pattern = pattern;
            Options = options;
            Rx = new Regex(Pattern, Options);
        }

        public string Pattern { get; protected set; }
        public RegexOptions Options { get; protected set; }
        public Regex Rx { get; protected set; }

        public bool Matches(IView view)
            => view.Text != null && Rx.IsMatch(view.Text);
    }
}
