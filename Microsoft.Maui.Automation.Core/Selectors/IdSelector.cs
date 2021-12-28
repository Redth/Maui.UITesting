namespace Microsoft.Maui.Automation
{
    public class IdSelector : IViewSelector
    {
        public IdSelector(string id, StringComparison comparison = StringComparison.Ordinal)
        {
            Id = id;
            Comparison = comparison;
        }

        public string Id { get; protected set; }
        public StringComparison Comparison { get; protected set; }

        public bool Matches(IView view)
            => view?.AutomationId?.Equals(Id, Comparison) ?? false;
    }
}
