namespace Microsoft.Maui.Automation
{
    public class DefaultViewSelector : IElementSelector
    {
        public DefaultViewSelector()
        {
        }

        public bool Matches(IElement view)
            => true;
    }
}
