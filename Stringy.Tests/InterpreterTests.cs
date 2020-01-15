using System;
using Stringy.Interpreter;
using Stringy.Parser;
using Xunit;

namespace Stringy.Tests
{
	public class TestObject
	{
		public string Method(string arg1)
		{
			return arg1;
		}

		public string Method(string arg1, string arg2)
		{
			return arg1 + arg2;
		}

		public string Method(string arg1, string arg2, string arg3 = "arg3")
		{
			return arg1 + arg2 + arg3;
		}

		public string Method(string arg1, string arg2, string arg3 = "arg3", string arg4 = "arg4")
		{
			return arg1 + arg2 + arg3 + arg4;
		}
	}

	public class InterpreterTests
	{
		private IInterpreter GetInterpreter(ISymbolTable symbolTable = null)
		{
			return new Interpreter.Interpreter(new StandardParser(), symbolTable ?? new SymbolTable());
		}

		[Fact]
		public void TestSimpleAddition()
		{
			const string template = "{3 + 4}";

			var interpreter = GetInterpreter();
			var result = interpreter.Interpret(template);

			Assert.Equal("7", result);
		}

		[Fact]
		public void TestSimpleSubtraction()
		{
			const string template = "{3 - 4}";

			var interpreter = GetInterpreter();
			var result = interpreter.Interpret(template);

			Assert.Equal("-1", result);
		}

		[Fact]
		public void TestSimpleMultiplication()
		{
			const string template = "{3 * 4}";

			var interpreter = GetInterpreter();
			var result = interpreter.Interpret(template);

			Assert.Equal("12", result);
		}

		[Fact]
		public void TestSimpleDivision()
		{
			const string template = "{12 / 4}";

			var interpreter = GetInterpreter();
			var result = interpreter.Interpret(template);

			Assert.Equal("3", result);
		}

		[Fact]
		public void TestAdditionAndMultiplication()
		{
			const string template = "{12 + 3 * 4}";

			var interpreter = GetInterpreter();
			var result = interpreter.Interpret(template);

			Assert.Equal("24", result);
		}

		[Fact]
		public void TestAdditionAndParenthesesAndMultiplication()
		{
			const string template = "{(12 + 3) * 4}";

			var interpreter = GetInterpreter();
			var result = interpreter.Interpret(template);

			Assert.Equal("60", result);
		}

		[Fact]
		public void TestStringLiteralAndMathExpression()
		{
			const string template = "The outcome of (12 + 3) * 4 equals {(12 + 3) * 4}";

			var interpreter = GetInterpreter();
			var result = interpreter.Interpret(template);

			Assert.Equal("The outcome of (12 + 3) * 4 equals 60", result);
		}

		[Fact]
		public void TestTernaryExpressionIfBody()
		{
			const string template = "{4 > 3 ? 4 - 3 : 3 - 4}";

			var interpreter = GetInterpreter();
			var result = interpreter.Interpret(template);

			Assert.Equal("1", result);
		}

		[Fact]
		public void TestTernaryExpressionElseBody()
		{
			const string template = "{3 > 4 ? 4 - 3 : 3 - 4}";

			var interpreter = GetInterpreter();
			var result = interpreter.Interpret(template);

			Assert.Equal("-1", result);
		}

		[Fact]
		public void TestSimpleVariables()
		{
			const string template = "{message}";
			
			var symbolTable = new SymbolTable();
			symbolTable.Set("message", "Hello World!");
			var interpreter = GetInterpreter(symbolTable);

			var result = interpreter.Interpret(template);

			Assert.Equal("Hello World!", result);
		}

		[Fact]
		public void TestNullIdentifier()
		{
			const string template = "{object == null}";

			var interpreter = GetInterpreter();

			var result = interpreter.Interpret(template);

			Assert.Equal("True", result);
		}
			
		[Fact]
		public void TestPropertyOfVariable()
		{
			const string variable = "Hello World!";
			const string template = "{variable.Length}";

			var symbolTable = new SymbolTable();
			symbolTable.Set("variable", variable);
			var interpreter = GetInterpreter(symbolTable);

			var result = interpreter.Interpret(template);

			Assert.Equal("12", result);
		}

		[Fact]
		public void TestMethodOfVariable()
		{
			const string variable = "HELLO WORLD!";
			const string template = "{variable.ToLower()}";

			var symbolTable = new SymbolTable();
			symbolTable.Set("variable", variable);
			var interpreter = GetInterpreter(symbolTable);

			var result = interpreter.Interpret(template);

			Assert.Equal("hello world!", result);
		}

		[Fact]
		public void TestMethodWithArgument()
		{
			const int variable = 55;
			const string template = "{variable.Equals((-1 + 7 * 8))}";

			var symbolTable = new SymbolTable();
			symbolTable.Set("variable", variable);
			var interpreter = GetInterpreter(symbolTable);

			var result = interpreter.Interpret(template);

			Assert.Equal("True", result);
		}

		[Fact]
		public void TestFunction()
		{
			const string template = "{HelloWorld()}";

			var symbolTable = new SymbolTable();
			symbolTable.Set<Func<string>>("HelloWorld", () => "Hello World");
			var interpreter = GetInterpreter(symbolTable);

			var result = interpreter.Interpret(template);

			Assert.Equal(HelloWorld(), result);
		}

		[Fact]
		public void TestFunctionWithWrongArguments()
		{
			const string template = "{Add('Hello', 'World')}";

			var symbolTable = new SymbolTable();
			symbolTable.Set<Func<int, int, int>>("Add", Add);
			var interpreter = GetInterpreter(symbolTable);

			Assert.Throws<Exception>(() => interpreter.Interpret(template));
		}

		[Fact]
		public void TestFunctionWithArguments()
		{
			const string template = "1 + 7 equals {Add(1, 7)}";

			var symbolTable = new SymbolTable();
			symbolTable.Set<Func<int, int, int>>("Add", Add);
			var interpreter = GetInterpreter(symbolTable);

			var result = interpreter.Interpret(template);

			Assert.Equal("1 + 7 equals 8", result);
		}

		[Fact]
		public void TestMethod1()
		{
			const string template = "{obj.Method('arg1')}";

			var symbolTable = new SymbolTable();
			symbolTable.Set("obj", new TestObject());
			var interpreter = GetInterpreter(symbolTable);

			var result = interpreter.Interpret(template);

			Assert.Equal("arg1", result);
		}

		[Fact]
		public void TestMethod2()
		{
			const string template = "{obj.Method('arg1', 'arg2')}";

			var symbolTable = new SymbolTable();
			symbolTable.Set("obj", new TestObject());
			var interpreter = GetInterpreter(symbolTable);

			var result = interpreter.Interpret(template);

			Assert.Equal("arg1arg2", result);
		}

		[Fact]
		public void TestMethod3()
		{
			const string template = "{obj.Method('arg1', 'arg2', 'arg3')}";

			var symbolTable = new SymbolTable();
			symbolTable.Set("obj", new TestObject());
			var interpreter = GetInterpreter(symbolTable);

			var result = interpreter.Interpret(template);

			Assert.Equal("arg1arg2arg3", result);
		}

		[Fact]
		public void TestMethod4()
		{
			const string template = "{obj.Method('arg1', 'arg2', 'arg3', 'arg4')}";

			var symbolTable = new SymbolTable();
			symbolTable.Set("obj", new TestObject());
			var interpreter = GetInterpreter(symbolTable);

			var result = interpreter.Interpret(template);

			Assert.Equal("arg1arg2arg3arg4", result);
		}

		[Fact]
		public void TestEachLoop()
		{
			const string template = "{each number in numbers ? number + ', '}";

			var symbolTable = new SymbolTable();
			symbolTable.Set("numbers", new[] {1, 2, 3});
			var interpreter = GetInterpreter(symbolTable);

			var result = interpreter.Interpret(template);

			Assert.Equal("1, 2, 3, ", result);
		}

		[Fact]
		public void TestEachLoopTernaryBody()
		{
			const string template = "{each number in numbers ? number + (number != 3 ? ', ')}";

			var symbolTable = new SymbolTable();
			symbolTable.Set("numbers", new[] { 1, 2, 3 });
			var interpreter = GetInterpreter(symbolTable);

			var result = interpreter.Interpret(template);

			Assert.Equal("1, 2, 3", result);
		}

	    [Fact]
	    public void TestBooleanExpressionEvaluation()
	    {
	        const string expression = "a == b";

	        var symbolTable = new SymbolTable();
            symbolTable.Set("a", 5);
            symbolTable.Set("b", 5);

	        var interpreter = GetInterpreter(symbolTable);

	        var result = interpreter.InterpretExpression<bool>(expression);

            Assert.True(result);
	    }

        [Fact]
	    public void TestBooleanTernaryExpressionEvaluation()
	    {
	        const string expression = "a == 5 ? true : false";

	        var symbolTable = new SymbolTable();
	        symbolTable.Set("a", 5);

	        var interpreter = GetInterpreter(symbolTable);

	        var result = interpreter.InterpretExpression<bool>(expression);

	        Assert.True(result);
        }

        [Fact]
	    public void TestIntegerExpressionEvaluation()
        {
            const string expression = "25 * 4 + 150";

            var interpreter = GetInterpreter();
            var result = interpreter.InterpretExpression<int>(expression);
                
            Assert.Equal(250, result);
        }

	    [Fact]
	    public void TestMultipleAddOpExpressionEvaluation()
	    {
	        const string expression = "25 + 5 + 100 - 30";

	        var interpreter = GetInterpreter();
	        var result = interpreter.InterpretExpression<int>(expression);

	        Assert.Equal(100, result);
        }

        [Fact]
	    public void TestMultipleMulOpExpressionEvaluation()
	    {
	        const string expression = "3 * 3 / 3 * 3";

	        var interpreter = GetInterpreter();
	        var result = interpreter.InterpretExpression<int>(expression);

	        Assert.Equal(9, result);
        }

        [Fact]
	    public void TestTernaryIntegerExpressionEvaluation()
	    {
	        const string expression = "25 * 4 - 1 >= 100 ? 500 : 1000";

	        var interpreter = GetInterpreter();
	        var result = interpreter.InterpretExpression<int>(expression);

	        Assert.Equal(1000, result);
        }

        [Fact]
	    public void TestDoubleExpressionEvaluation()
        {
            const string expression = "25 * 4 / 1000.0";

	        var interpreter = GetInterpreter();
	        var result = interpreter.InterpretExpression<double>(expression);

	        Assert.Equal(0.1, result);
        }

        [Fact]
	    public void TestBooleanOperatorPrecedence()
	    {
            // If AND takes precedence over OR, this will be true, but if OR takes
            // precedence over AND, this will be false. AND should take precedence
            // over OR.
	        const string expression = "false && false || true";

	        var interpreter = GetInterpreter();
	        var result = interpreter.InterpretExpression<bool>(expression);

	        Assert.True(result);
        }

		private string HelloWorld()
		{
			return "Hello World";
		}

		private int Add(int a, int b)
		{
			return a + b;
		}
	}
}
