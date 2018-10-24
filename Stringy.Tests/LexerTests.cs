using System;
using Stringy.Lexer;
using Xunit;

namespace Stringy.Tests
{
	public class LexerTests
	{
		[Fact]
		public void TestStringLiteral()
		{
			var str = "stringw1thnumb3rs and no digits";

			var lexer = new StandardLexer(str);

			Assert.True(lexer.GetNextToken() == new Token("stringw1thnumb3rs and no digits", TokenType.TEXT_LITERAL));
			Assert.True(lexer.GetNextToken().Type == TokenType.EOF);
		}
		
		[Fact]
		public void TestMathExpression()
		{
			var str = "{(23+40.503)}";

			var lexer = new StandardLexer(str);

			lexer.GetNextToken(); // {
			Assert.True(lexer.GetNextToken() == new Token("(", TokenType.LPAREN));
			Assert.True(lexer.GetNextToken() == new Token("23", TokenType.INTEGER_CONST));
			Assert.True(lexer.GetNextToken() == new Token("+", TokenType.PLUS));
			Assert.True(lexer.GetNextToken() == new Token("40.503", TokenType.REAL_CONST));
			Assert.True(lexer.GetNextToken() == new Token(")", TokenType.RPAREN));
			lexer.GetNextToken(); // }
			Assert.True(lexer.GetNextToken().Type == TokenType.EOF);
		}

		[Fact]
		public void TestParentheses()
		{
			var str = "{()}";

			var lexer = new StandardLexer(str);

			lexer.GetNextToken(); // {
			Assert.True(lexer.GetNextToken() == new Token("(", TokenType.LPAREN));
			Assert.True(lexer.GetNextToken() == new Token(")", TokenType.RPAREN));
			lexer.GetNextToken(); // }
			Assert.True(lexer.GetNextToken().Type == TokenType.EOF);
		}

		[Fact]
		public void TestBraces()
		{
			var str = "{}";

			var lexer = new StandardLexer(str);

			Assert.True(lexer.GetNextToken() == new Token("{", TokenType.LBRACE));
			Assert.True(lexer.GetNextToken() == new Token("}", TokenType.RBRACE));
			Assert.True(lexer.GetNextToken().Type == TokenType.EOF);
		}

		[Fact]
		public void TestSquareBrackets()
		{
			var str = "{[]}";

			var lexer = new StandardLexer(str);

			lexer.GetNextToken(); // {
			Assert.True(lexer.GetNextToken() == new Token("[", TokenType.LSQUARE));
			Assert.True(lexer.GetNextToken() == new Token("]", TokenType.RSQUARE));
			lexer.GetNextToken(); // }
			Assert.True(lexer.GetNextToken().Type == TokenType.EOF);
		}

		[Fact]
		public void TestMathOperators()
		{
			var str = "{+-*/}";

			var lexer = new StandardLexer(str);

			lexer.GetNextToken(); // {
			Assert.True(lexer.GetNextToken() == new Token("+", TokenType.PLUS));
			Assert.True(lexer.GetNextToken() == new Token("-", TokenType.MINUS));
			Assert.True(lexer.GetNextToken() == new Token("*", TokenType.MUL));
			Assert.True(lexer.GetNextToken() == new Token("/", TokenType.DIV));
			lexer.GetNextToken(); // }
			Assert.True(lexer.GetNextToken().Type == TokenType.EOF);
		}

		[Fact]
		public void TestComparisonOperators()
		{
			var str = "{= != < > <= >=}";

			var lexer = new StandardLexer(str);

			lexer.GetNextToken(); // {
			Assert.True(lexer.GetNextToken() == new Token("=", TokenType.EQ));
			lexer.GetNextToken(); // Whitespace
			Assert.True(lexer.GetNextToken() == new Token("!=", TokenType.NEQ));
			lexer.GetNextToken(); // Whitespace
			Assert.True(lexer.GetNextToken() == new Token("<", TokenType.LT));
			lexer.GetNextToken(); // Whitespace
			Assert.True(lexer.GetNextToken() == new Token(">", TokenType.GT));
			lexer.GetNextToken(); // Whitespace
			Assert.True(lexer.GetNextToken() == new Token("<=", TokenType.LTE));
			lexer.GetNextToken(); // Whitespace
			Assert.True(lexer.GetNextToken() == new Token(">=", TokenType.GTE));
			lexer.GetNextToken(); // }
			Assert.True(lexer.GetNextToken().Type == TokenType.EOF);
		}

		[Fact]
		public void TestBranchOperators()
		{
			var str = "{?:}";

			var lexer = new StandardLexer(str);

			lexer.GetNextToken(); // {
			Assert.True(lexer.GetNextToken() == new Token("?", TokenType.QM));
			Assert.True(lexer.GetNextToken() == new Token(":", TokenType.COLON));
			lexer.GetNextToken(); // }
			Assert.True(lexer.GetNextToken().Type == TokenType.EOF);
		}

		[Fact]
		public void TestStringEscaping()
		{
			var str = "\\{";

			var lexer = new StandardLexer(str);

			Assert.True(lexer.GetNextToken() == new Token("{", TokenType.TEXT_LITERAL));
			Assert.True(lexer.GetNextToken().Type == TokenType.EOF);
		}

		[Fact]
		public void TestQuotedString()
		{
			var str = "{'testing <= {} () 1 > 2 and 2 != 1'}";

			var lexer = new StandardLexer(str);

			lexer.GetNextToken(); // {
			Assert.True(lexer.GetNextToken() == new Token("testing <= {} () 1 > 2 and 2 != 1", TokenType.TEXT_LITERAL));
			lexer.GetNextToken(); // }
			Assert.True(lexer.GetNextToken().Type == TokenType.EOF);
		}

		[Fact]
		public void TestIdentifier()
		{
			var str = "{identifier1 + identifier2}";

			var lexer = new StandardLexer(str);

			lexer.GetNextToken(); // {
			Assert.True(lexer.GetNextToken() == new Token("identifier1", TokenType.IDENTIFIER));
			lexer.GetNextToken(); // whitespace
			Assert.True(lexer.GetNextToken() == new Token("+", TokenType.PLUS));
			lexer.GetNextToken(); // whitespace
			Assert.True(lexer.GetNextToken() == new Token("identifier2", TokenType.IDENTIFIER));
			lexer.GetNextToken(); // }
			Assert.True(lexer.GetNextToken().Type == TokenType.EOF);
		}

		[Fact]
		public void TestNullIdentifier()
		{
			var str = "{identifier1 + null}";

			var lexer = new StandardLexer(str);

			lexer.GetNextToken(); // {
			Assert.True(lexer.GetNextToken() == new Token("identifier1", TokenType.IDENTIFIER));
			lexer.GetNextToken(); // whitespace
			Assert.True(lexer.GetNextToken() == new Token("+", TokenType.PLUS));
			lexer.GetNextToken(); // whitespace
			Assert.True(lexer.GetNextToken() == new Token("null", TokenType.NULL));
			lexer.GetNextToken(); // }
			Assert.True(lexer.GetNextToken().Type == TokenType.EOF);
		}

		[Fact]
		public void NonDelimitedStringThrowsException()
		{
			var str = "{'no ending quote}";

			var lexer = new StandardLexer(str);
			lexer.GetNextToken(); // {
			Assert.Throws<Exception>(() => lexer.GetNextToken());
		}
	}
}
