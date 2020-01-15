namespace Stringy
{
    public interface IStringy
    {
        /// <summary>
        /// Treats the input template as a template string with substitution sections, and
        /// returns the template with its substitution sections replaced by the
        /// interpretation results.
        /// </summary>
        string Execute(string template, ErrorMode errMode = ErrorMode.ThrowExceptions);

        TResult EvaluateExpression<TResult>(string expression, ErrorMode errMode = ErrorMode.ThrowExceptions);

        IStringy Set<TClass>(string variableName, TClass value);
    }
}
