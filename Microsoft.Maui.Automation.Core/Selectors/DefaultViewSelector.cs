namespace Microsoft.Maui.Automation
{
    public class DefaultViewSelector : IViewSelector
    {
        public DefaultViewSelector()
        {
        }

        public bool Matches(IView view)
            => true;
    }
}
