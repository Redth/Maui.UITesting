using OpenQA.Selenium;
using System.Collections.ObjectModel;

namespace Microsoft.Maui.WebDriver
{
    public class MobileBy : By
    {
		public static new MobileBy Id(string id)
			=> new ("id", id);


		public MobileBy(string strategy, string selector)
			: base()
        {
			Strategy = strategy;
			Selector = selector;
        }

		public readonly string Strategy;
		public readonly string Selector;

		public override IWebElement FindElement(ISearchContext context)
        {
			
            return base.FindElement(context);
        }

        public override ReadOnlyCollection<IWebElement> FindElements(ISearchContext context)
        {
            return base.FindElements(context);
        }
    }


}
