using Stringy.Interpreter;
using Stringy.Parser;

using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Stringy.Tests")]

namespace Stringy
{
    public class Stringy : IStringy
    {
        private IInterpreter Interpreter { get; }

        private ISymbolTable SymbolTable { get; }

        public Stringy() : this(new SymbolTable())
        {
        }

        public Stringy(ISymbolTable symbolTable)
        {
            // Init
            SymbolTable = symbolTable;
            Interpreter = new Interpreter.Interpreter(new StandardParser(), SymbolTable);
        }

        public string Execute(string template, ErrorMode errMode = ErrorMode.ThrowExceptions)
        {
            return Interpreter.Interpret(template, errMode);
        }

        public void Set<TClass>(string variableName, TClass value)
        {
            SymbolTable.Set(variableName, value);
        }
    }
}
