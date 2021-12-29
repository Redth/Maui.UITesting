using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Maui.Automation
{
    public static class ApiExtensions
    {

        public static Task<IElement?> Expect(this IApplication app, Platform platform)
        {
            return null;
        }

        public static IAsyncEnumerable<IElement> By(this IElement element, params IElementSelector[] selectors)
            => All(element.Application!, element.Platform, selectors);

        public static async Task<IElement?> FirstBy(this IElement element, params IElementSelector[] selectors)
        {
            await foreach (var e in By(element, selectors))
                return e;

            return default;
        }

        public static IAsyncEnumerable<IElement> ByAutomationId(this IElement element, string automationId, StringComparison comparison = StringComparison.Ordinal)
            => By(element, new AutomationIdSelector(automationId, comparison));

        public static IAsyncEnumerable<IElement> ById(this IElement element, string id, StringComparison comparison = StringComparison.Ordinal)
            => By(element, new IdSelector(id, comparison));




        public static IAsyncEnumerable<IElement> By(this IApplication app, Platform platform, params IElementSelector[] selectors)
            => All(app, platform, selectors);

        public static async Task<IElement?> FirstBy(this IApplication app, Platform platform, params IElementSelector[] selectors)
        {
            await foreach (var e in By(app, platform, selectors))
                return e;

            return default;
        }

        public static IAsyncEnumerable<IElement> ByAutomationId(this IApplication app, Platform platform, string automationId, StringComparison comparison = StringComparison.Ordinal)
            => By(app, platform, new AutomationIdSelector(automationId, comparison));

        public static Task<IElement?> ById(this IApplication app, Platform platform, string id, StringComparison comparison = StringComparison.Ordinal)
            => FirstBy(app, platform, new IdSelector(id, comparison));



        public static async IAsyncEnumerable<IElement> All(this IApplication app, Platform platform, params IElementSelector[] selectors)
        {
            await foreach(var element in app.Descendants(platform, selector: new CompoundSelector(any: false, selectors)))
                yield return element;
        }

        public static async IAsyncEnumerable<IElement> Any(this IApplication app, Platform platform, params IElementSelector[] selectors)
        {
            await foreach (var element in app.Descendants(platform, selector: new CompoundSelector(any: true, selectors)))
                yield return element;
        }


        public static IAsyncEnumerable<IElement> All(this IElement element, params IElementSelector[] selectors)
            => All(element, element.Platform, selectors);

        public static async IAsyncEnumerable<IElement> All(this IElement element, Platform platform, params IElementSelector[] selectors)
        {
            if (element.Application is null)
                throw new NullReferenceException("Element's Application is null");
            await foreach (var d in element.Application!.Descendants(platform, element.Id, new CompoundSelector(any: false, selectors)))
                yield return element;
        }

        public static IAsyncEnumerable<IElement> Any(this IElement element, params IElementSelector[] selectors)
            => Any(element, element.Platform, selectors);

        public static async IAsyncEnumerable<IElement> Any(this IElement element, Platform platform, params IElementSelector[] selectors)
        {
            if (element.Application is null)
                throw new NullReferenceException("Element's Application is null");
            await foreach (var d in element.Application!.Descendants(platform, element.Id, new CompoundSelector(any: true, selectors)))
                yield return d;
        }


    }
}
