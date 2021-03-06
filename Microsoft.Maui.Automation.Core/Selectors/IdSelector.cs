namespace Microsoft.Maui.Automation
{
    public class IdSelector : IElementSelector
    {
        public IdSelector(string id, StringComparison comparison = StringComparison.Ordinal)
        {
            Id = id;
            Comparison = comparison;
        }

        public string Id { get; protected set; }

        public StringComparison Comparison { get; protected set; }

        public bool Matches(IElement view)
            => view?.AutomationId?.Equals(Id, Comparison) ?? false;
    }
}
