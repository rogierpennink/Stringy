using System.Linq;
using Stringy.Ast;
using Stringy.Lexer;
using Stringy.Parser;
using Xunit;

namespace Stringy.Tests
{
	public class ParserTests
	{
		[Fact]
		public void TestLiteralsWithoutProgram()
		{
			var template = "this is a test template!";

			var parser = new StandardParser();

			var rootNode = parser.Parse(new StandardLexer(template));

			Assert.Single(rootNode.Children);
			Assert.True(rootNode.Children.All(c => c is TerminalAstNode<string>));
			Assert.True(((TerminalAstNode<string>)rootNode.Children.Single()).Value == "this is a test template!");
		}

		[Fact]
		public void TestLiteralsWithSingleProgram()
		{
			var template = "left{program}right";

			var parser = new StandardParser();

			var rootNode = parser.Parse(new StandardLexer(template));

			Assert.Equal(3, rootNode.Children.Count());
			Assert.True(rootNode.Children.First() is TerminalAstNode<string>);
			Assert.True(rootNode.Children.Last() is TerminalAstNode<string>);
		}

		[Fact]
		public void TestTernaryOperator()
		{
			var template = "{expression ? 'expression-if' : 'expression-else'}";

			var parser = new StandardParser();

			var rootNode = parser.Parse(new StandardLexer(template));

			var branchNode = rootNode.Children.First() as BranchAstNode;
			Assert.True(branchNode != null);
			Assert.True((branchNode.Condition as IdentifierVariableAccessAstNode).Name == "expression");
			Assert.True((branchNode.IfBody as TerminalAstNode<string>).Value == "expression-if");
			Assert.True((branchNode.ElseBody as TerminalAstNode<string>).Value == "expression-else");
		}

		[Fact]
		public void TestNestedTernaryExpression()
		{
			var template = "{2 > 1 ? (3 > 2 ? 'super true' : 'super false') : 'false'}";

			var parser = new StandardParser();

			var rootNode = parser.Parse(new StandardLexer(template));

			var branchNode = rootNode.Children.First() as BranchAstNode;
			Assert.True(branchNode != null);

			var firstConditionNode = (branchNode.Condition as BinaryOperationAstNode);
			Assert.True(firstConditionNode != null);
			Assert.True((firstConditionNode.Left as TerminalAstNode<int>).Value == 2);
			Assert.True(firstConditionNode.Operation.Type == TokenType.GT);
			Assert.True((firstConditionNode.Right as TerminalAstNode<int>).Value == 1);

			var secondBranchNode = branchNode.IfBody as BranchAstNode;
			var secondConditionNode = secondBranchNode.Condition as BinaryOperationAstNode;
			Assert.True(secondConditionNode != null);
			Assert.True((secondConditionNode.Left as TerminalAstNode<int>).Value == 3);
			Assert.True(secondConditionNode.Operation.Type == TokenType.GT);
			Assert.True((secondConditionNode.Right as TerminalAstNode<int>).Value == 2);

			Assert.True((secondBranchNode.IfBody as TerminalAstNode<string>).Value == "super true");
			Assert.True((secondBranchNode.ElseBody as TerminalAstNode<string>).Value == "super false");

			Assert.True((branchNode.ElseBody as TerminalAstNode<string>).Value == "false");
		}

		[Fact]
		public void TestOperatorPrecedence()
		{
			var template = "{8 * 4 + 3.1415}";

			var parser = new StandardParser();

			var rootNode = parser.Parse(new StandardLexer(template));

			var firstBinaryOp = rootNode.Children.First() as BinaryOperationAstNode;

			Assert.True(firstBinaryOp != null);
			Assert.True(firstBinaryOp.Operation.Type == TokenType.PLUS);
			Assert.True((firstBinaryOp.Right as TerminalAstNode<double>).Value == 3.1415);

			var secondBinaryOp = firstBinaryOp.Left as BinaryOperationAstNode;
			Assert.True((secondBinaryOp.Left as TerminalAstNode<int>).Value == 8);
			Assert.True((secondBinaryOp.Right as TerminalAstNode<int>).Value == 4);
		}

		[Fact]
		public void TestTernaryWithoutElseBody()
		{
			const string template = "{blah > 3 ? 'some text'}";

			var parser = new StandardParser();

			var rootNode = parser.Parse(new StandardLexer(template));

			var branchNode = rootNode.Children.First() as BranchAstNode;

			Assert.True(branchNode != null);
			Assert.True((branchNode.IfBody as TerminalAstNode<string>).Value == "some text");
			Assert.True((branchNode.ElseBody as NoOpAstNode) != null);

			var conditionNode = (branchNode.Condition as BinaryOperationAstNode);
			Assert.True(conditionNode != null);
			Assert.True(conditionNode.Operation.Type == TokenType.GT);
			Assert.True((conditionNode.Left as IdentifierVariableAccessAstNode).Name == "blah");
			Assert.True((conditionNode.Right as TerminalAstNode<int>).Value == 3);
		}

		[Fact]
		public void TestNullIdentifier()
		{
			const string template = "{null = null}";

			var parser = new StandardParser();
			var rootNode = parser.Parse(new StandardLexer(template));

			var binOpNode = rootNode.Children.First() as BinaryOperationAstNode;
			Assert.NotNull(binOpNode);
			Assert.True(binOpNode.Operation.Type == TokenType.EQ);
			Assert.NotNull(binOpNode.Left as NullAstNode);
			Assert.NotNull(binOpNode.Right as NullAstNode);
		}

		[Fact]
		public void TestProcedureCall()
		{
			const string template = "{variable.Method()}";

			var parser = new StandardParser();

			var rootNode = parser.Parse(new StandardLexer(template));
			
			var node = rootNode.Children.First() as IdentifierVariableAccessAstNode;
			
			Assert.True(node != null);
			Assert.Equal("variable", node.Name);
			Assert.Equal("Method", node.MemberIdentifier.Name);
			Assert.Empty((node.MemberIdentifier as IdentifierProcedureCallAstNode).Params);
		}

		[Fact]
		public void TestSimpleParamsList()
		{
			const string template = "{Method(variable)}";

			var parser = new StandardParser();

			var rootNode = parser.Parse(new StandardLexer(template));

			var procNode = rootNode.Children.First() as IdentifierProcedureCallAstNode;

			Assert.True(procNode != null);
			Assert.Equal("Method", procNode.Name);
			Assert.Single(procNode.Params);

			var paramNode = procNode.Params.First() as IdentifierVariableAccessAstNode;
			Assert.True(paramNode != null);
			Assert.Equal("variable", paramNode.Name);
		}

		[Fact]
		public void TestMultiSimpleParamsList()
		{
			const string template = "{Method(variable, 'text')}";

			var parser = new StandardParser();

			var rootNode = parser.Parse(new StandardLexer(template));

			var procNode = rootNode.Children.First() as IdentifierProcedureCallAstNode;

			Assert.True(procNode != null);
			Assert.Equal("Method", procNode.Name);
			Assert.Equal(2, procNode.Params.Count());

			var paramNode = procNode.Params.First() as IdentifierVariableAccessAstNode;
			Assert.True(paramNode != null);
			Assert.Equal("variable", paramNode.Name);

			var paramNode2 = procNode.Params.Last() as TerminalAstNode<string>;
			Assert.True(paramNode2 != null);
			Assert.Equal("text", paramNode2.Value);
		}

		[Fact]
		public void TestExpressionParameterList()
		{
			const string template = "{Method(3 + 4 * 5)}";

			var parser = new StandardParser();

			var rootNode = parser.Parse(new StandardLexer(template));

			var procNode = rootNode.Children.First() as IdentifierProcedureCallAstNode;

			Assert.True(procNode != null);
			Assert.Equal("Method", procNode.Name);
			Assert.Single(procNode.Params);

			var paramNode = procNode.Params.First() as BinaryOperationAstNode;
			Assert.True(paramNode != null);
			Assert.Equal(TokenType.PLUS, paramNode.Operation.Type);
			Assert.Equal(3, (paramNode.Left as TerminalAstNode<int>).Value);

			var secondOp = paramNode.Right as BinaryOperationAstNode;
			Assert.Equal(TokenType.MUL, secondOp.Operation.Type);
			Assert.Equal(4, (secondOp.Left as TerminalAstNode<int>).Value);
			Assert.Equal(5, (secondOp.Right as TerminalAstNode<int>).Value);
		}

		[Fact]
		public void TestProcedureCallInParameterList()
		{
			const string template = "{Method(SecondMethod())}";

			var parser = new StandardParser();

			var rootNode = parser.Parse(new StandardLexer(template));

			var procNode = rootNode.Children.First() as IdentifierProcedureCallAstNode;

			Assert.True(procNode != null);
			Assert.Equal("Method", procNode.Name);
			Assert.Single(procNode.Params);

			var paramNode = procNode.Params.First() as IdentifierProcedureCallAstNode;
			Assert.True(paramNode != null);
			Assert.Equal("SecondMethod", paramNode.Name);
			Assert.Empty(paramNode.Params);
		}

		[Fact]
		public void TestEachLoop()
		{
			const string template = "{each number in numbers ? number + ', '}";

			var parser = new StandardParser();

			var rootNode = parser.Parse(new StandardLexer(template));

			var eachNode = rootNode.Children.First() as EachLoopAstNode;

			Assert.True(eachNode != null);
			Assert.True(eachNode.LoopVariableNode.Name == "number");
			Assert.True(eachNode.EnumerableVariableNode.Name == "numbers");
		}
	}
}
