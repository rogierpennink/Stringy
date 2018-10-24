namespace Stringy
{
    public interface IStringy
    {
        string Execute(string template, ErrorMode errMode = ErrorMode.ThrowExceptions);

        void Set<TClass>(string variableName, TClass value);
    }
}
