namespace Microsoft.Maui.Automation
{

    public class AutomationIdSelector : IViewSelector
    {
        public AutomationIdSelector(string automationId, StringComparison comparison = StringComparison.Ordinal)
        {
            AutomationId = automationId;
            Comparison = comparison;
        }

        public string AutomationId { get; protected set; }
        public StringComparison Comparison { get; protected set; }

        public bool Matches(IView view)
            => view?.AutomationId?.Equals(AutomationId, Comparison) ?? false;
    }
}
