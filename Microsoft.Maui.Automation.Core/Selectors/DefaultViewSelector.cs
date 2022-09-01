namespace Microsoft.Maui.Automation
{
    public class DefaultViewSelector : IElementSelector
    {
        public DefaultViewSelector()
        {
        }

        public bool Matches(Element view)
            => true;
    }
}
