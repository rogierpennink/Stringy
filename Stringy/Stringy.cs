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

        /// <inheritdoc />
        public string Execute(string template, ErrorMode errMode = ErrorMode.ThrowExceptions)
        {
            return Interpreter.Interpret(template, errMode);
        }

        /// <inheritdoc />
        public TResult EvaluateExpression<TResult>(string expression, ErrorMode errMode = ErrorMode.ThrowExceptions)
        {
            return Interpreter.InterpretExpression<TResult>(expression, errMode);
        }

        /// <inheritdoc />
        public IStringy Set<TClass>(string variableName, TClass value)
        {
            SymbolTable.Set(variableName, value);
            return this;
        }
    }
}
