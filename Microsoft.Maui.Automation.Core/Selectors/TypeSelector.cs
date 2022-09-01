namespace Microsoft.Maui.Automation
{
    public class TypeSelector : IElementSelector
    {
        internal TypeSelector()
        { }

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

        public string TypeName { get; protected set; }

        public bool FullName { get; protected set; }

        public bool Matches(Element element)
            => element.Type.Equals(TypeName);
    }
}
