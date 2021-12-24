namespace Microsoft.Maui.Automation
{
    public interface IAction
    {
        public Task<IActionResult> Invoke(IView element);

        //public void Clear();

        //public void SendKeys(string text);

        //public void Return();

        //public void Click();

        //public bool Focus();

        //public string GetProperty(string propertyName);
    }
}
