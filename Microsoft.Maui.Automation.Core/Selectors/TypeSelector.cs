namespace Microsoft.Maui.Automation
{
    public class TypeSelector : IViewSelector
    {
        public TypeSelector(string typeName)
        {
            TypeName = typeName;
        }

        public TypeSelector(Type type)
        {
           TypeName = type.Name;
        }

        public string TypeName { get; protected set; }

        public bool Matches(IView view)
            => view.Type.Equals(TypeName);
    }
}
