using Stringy.Interpreter;
using Stringy.Parser;
using Xunit;

namespace Stringy.Tests
{
	public class StdLibTests
	{
		private IInterpreter GetInterpreter(ISymbolTable symbolTable = null)
		{
			return new Interpreter.Interpreter(new StandardParser(), symbolTable ?? new SymbolTable());
		}

		[Fact]
		public void TestCapitalizeDefault()
		{
			const string template = "{Text.Capitalize('hello world')}";
			
			var interpreter = GetInterpreter();
			var result = interpreter.Interpret(template);

			Assert.Equal("HELLO WORLD", result);
		}

		[Fact]
		public void TestCapitalizeUcFirst()
		{
			const string template = "{Text.Capitalize('hello world', true)}";

			var interpreter = GetInterpreter();
			var result = interpreter.Interpret(template);

			Assert.Equal("Hello world", result);
		}

		[Fact]
		public void TestEnumerate()
		{
			const string template = "{Text.Enumerate(names)}";

			var symbolTable = new SymbolTable();
			symbolTable.Set("names", new[] {"Bugs Bunny", "Tom", "Jerry"});
			var interpreter = GetInterpreter(symbolTable);

			var result = interpreter.Interpret(template);

			Assert.Equal("Bugs Bunny, Tom and Jerry", result);

		}
	}
}
