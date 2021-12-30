using Newtonsoft.Json;

namespace Microsoft.Maui.Automation
{
    public class TypeSelector : IElementSelector
    {
        public TypeSelector(string typeName, bool fullName = false)
        {
            TypeName = typeName;
            FullName = fullName;
        }

        public TypeSelector(Type type, bool fullName = false)
        {
            TypeName = type.Name;
            FullName = fullName;
        }

        [JsonProperty]
        public string TypeName { get; protected set; }

        [JsonProperty]
        public bool FullName { get; protected set; }

        public bool Matches(IElement element)
            => element.Type.Equals(TypeName);
    }
}
