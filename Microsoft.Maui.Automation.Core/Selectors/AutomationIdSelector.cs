using Newtonsoft.Json;

namespace Microsoft.Maui.Automation
{

    public class AutomationIdSelector : IElementSelector
    {
        public AutomationIdSelector(string automationId, StringComparison comparison = StringComparison.Ordinal)
        {
            AutomationId = automationId;
            Comparison = comparison;
        }

        [JsonProperty]
        public string AutomationId { get; protected set; }

        [JsonProperty]
        public StringComparison Comparison { get; protected set; }

        public bool Matches(IElement view)
            => view?.AutomationId?.Equals(AutomationId, Comparison) ?? false;
    }
}
