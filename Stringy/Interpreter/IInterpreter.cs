namespace Stringy.Interpreter
{
	internal interface IInterpreter
	{
		string Interpret(string template, ErrorMode errMode = ErrorMode.ThrowExceptions);

	    TResult InterpretExpression<TResult>(string expression, ErrorMode errMode = ErrorMode.ThrowExceptions);

	}
}
