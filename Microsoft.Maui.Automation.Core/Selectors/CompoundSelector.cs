using Newtonsoft.Json;

namespace Microsoft.Maui.Automation
{
    public class CompoundSelector : IElementSelector
    {
        public CompoundSelector(bool any, params IElementSelector[] elementSelectors)
        {
            Selectors = elementSelectors;
            Any = any;
        }

        [JsonProperty]
        public IElementSelector[] Selectors { get; protected set; }

        [JsonProperty]
        public bool Any { get; protected set; }

        public bool Matches(IElement element)
        {
            foreach (var s in Selectors)
            {
                var isMatch = s.Matches(element);

                // If looking for any to match, and we found a match, return true
                if (Any && isMatch)
                    return true;

                // If we want ALL to match and we found a non-match, return false
                if (!Any && !isMatch)
                    return false;
            }

            // If we get here, we went through all the selectors and all did or all did not match
            // Otherwise we would have short circuited out the loop earlier
            // If we were looking for ANY, we had no matches, return false
            // If we are looking for ALL, we had all matches, return true
            return !Any;
        }
    }
}
