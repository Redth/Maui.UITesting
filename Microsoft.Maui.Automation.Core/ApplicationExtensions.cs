using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation
{
    public static class ApiExtensions
    {
        //public static Task<IEnumerable<Element>> By(this Element element, params IElementSelector[] selectors)
        //    => All(element.Application, element.Platform, selectors);

        //public static async Task<Element?> FirstBy(this Element element, params IElementSelector[] selectors)
        //    => (await By(element.Application, element.Platform, selectors)).FirstOrDefault();

        //public static Task<IEnumerable<Element>> By(this IApplication app, Platform platform, params IElementSelector[] selectors)
        //    => All(app, platform, selectors);

        //public static async Task<Element?> FirstBy(this IApplication app, Platform platform, params IElementSelector[] selectors)
        //    => (await By(app, platform, selectors))?.FirstOrDefault();

        //public static Task<IEnumerable<Element>> ByAutomationId(this IApplication app, Platform platform, string automationId, StringComparison comparison = StringComparison.Ordinal)
        //    => By(app, platform, new AutomationIdSelector(automationId, comparison));

        //public static Task<Element?> ById(this IApplication app, Platform platform, string id, StringComparison comparison = StringComparison.Ordinal)
        //    => FirstBy(app, platform, new IdSelector(id, comparison));



        //public static Task<IEnumerable<Element>> All(this IApplication app, Platform platform, params IElementSelector[] selectors)
        //    => app.GetElements(platform, selector: new CompoundSelector(any: false, selectors));

        //public static Task<IEnumerable<Element>> Any(this IApplication app, Platform platform, params IElementSelector[] selectors)
        //    => app.GetElements(platform, selector: new CompoundSelector(any: true, selectors));

        //public static Task<IEnumerable<Element>> All(this Element element, Platform platform, params IElementSelector[] selectors)
        //    => element.Application.GetElements(platform, element.Id, new CompoundSelector(any: false, selectors));

        //public static Task<IEnumerable<Element>> Any(this Element element, Platform platform, params IElementSelector[] selectors)
        //    => element.Application.GetElements(platform, element.Id, new CompoundSelector(any: true, selectors));


    }
}
