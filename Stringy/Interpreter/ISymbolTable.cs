namespace Stringy.Interpreter
{
	public interface ISymbolTable
	{
		bool Has(string key);

		object Get(string key);

		void Set<TType>(string variableName, TType value);

		void Set(string variableName, object value);
	}
}
