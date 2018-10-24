using System;
using System.Collections.Generic;
using System.Linq;
using Stringy.Ast;
using Stringy.Lexer;

namespace Stringy.Parser
{
	internal class StandardParser : IParser
	{
		private readonly TokenType[] MulOp = { TokenType.MUL, TokenType.DIV, TokenType.AND };

		private readonly TokenType[] AddOp = { TokenType.PLUS, TokenType.MINUS, TokenType.OR };

		private readonly TokenType[] RelOp =
		{
			TokenType.EQ, TokenType.NEQ, TokenType.LT, TokenType.GT,
			TokenType.LTE, TokenType.GTE
		};

		private Token CurrentToken;

		private ILexer Lexer;

		public StandardParser()
		{
		}

		public AstNode Parse(ILexer lexer)
		{
			Lexer = lexer;

			// Move to first token
			Next();

			var node = ParseTemplate();

			if (CurrentToken.Type != TokenType.EOF)
				Error();

			return node;
		}

		private void Error(string message = null)
		{
			throw new Exception(message ?? $"Syntax error occurred at token '{CurrentToken.Value}' at position {CurrentToken.Position}");
		}

		private void Next()
		{
			CurrentToken = Lexer.GetNextToken();
		}

		private void Eat(TokenType type)
		{
			if (CurrentToken.Type != type)
				Error();

			Next();
		}

		private void EatWhiteSpace()
		{
			while (CurrentToken.Type == TokenType.WHITESPACE)
			{
				var node = new TerminalAstNode<string>(CurrentToken, CurrentToken.Value);
				Eat(TokenType.WHITESPACE);
			}
		}

		#region Parse methods

		/// <summary>
		/// template		: (program | STRING_CONST)*
		/// </summary>
		private TemplateAstNode ParseTemplate()
		{
			var templateNode = new TemplateAstNode(CurrentToken);

			while (CurrentToken.Type != TokenType.EOF)
			{
				// When we encounter a '{' token that signifies the start of a program,
				// hand off control for the current node to the program parser
				if (CurrentToken.Type == TokenType.LBRACE)
				{
					templateNode.AddNode(ParseProgram());
					continue;
				}

				templateNode.AddNode(new TerminalAstNode<string>(CurrentToken, CurrentToken.Value));
				Next();
			}

			return templateNode;
		}

		/// <summary>
		/// program			: LBRACE statement RBRACE
		/// </summary>
		private AstNode ParseProgram()
		{
			if (CurrentToken.Type != TokenType.LBRACE)
				Error();

			// Skip LBRACE
			Eat(TokenType.LBRACE);

			var statementNode = ParseStatement();

			EatWhiteSpace();

			// Skip RBRACE
			Eat(TokenType.RBRACE);

			return statementNode;
		}

		/// <summary>
		/// statement		: expression (WHITESPACE)* ternary
		///					| each_loop
		///					| expression
		/// </summary>
		private AstNode ParseStatement()
		{
			if (CurrentToken.Type == TokenType.IDENTIFIER && CurrentToken.Value == "each")
				return ParseEachLoop();

			var node = ParseExpression();

			EatWhiteSpace();

			if (CurrentToken.Type == TokenType.QM)
			{
				return ParseTernary(node);
			}


			return node;
		}

		/// <summary>
		/// each_loop		: each variable in variable QM statement
		/// 
		/// TODO: The lexer should return reserved keywords as tokens
		/// </summary>
		/// <returns></returns>
		private AstNode ParseEachLoop()
		{
			if (CurrentToken.Type != TokenType.IDENTIFIER && CurrentToken.Value != "each")
				throw new Exception($"Invalid token, expected 'each' keyword at position {CurrentToken.Position}");

			var eachToken = CurrentToken;

			Eat(TokenType.IDENTIFIER);

			EatWhiteSpace();

			// Extract variable name
			var loopVariable = new IdentifierAstNode(CurrentToken);
			Eat(TokenType.IDENTIFIER);

			EatWhiteSpace();

			// 'in' keyword
			if (CurrentToken.Type != TokenType.IDENTIFIER && CurrentToken.Value == "in")
				throw new Exception($"Invalid token, expect 'in' keyword at position {CurrentToken.Position}");

			Eat(TokenType.IDENTIFIER);
			EatWhiteSpace();

			var enumerable = new IdentifierAstNode(CurrentToken);
			Eat(TokenType.IDENTIFIER);

			EatWhiteSpace();

			Eat(TokenType.QM);

			EatWhiteSpace();

			var loopBody = ParseStatement();
			
			return new EachLoopAstNode(eachToken, loopVariable, enumerable, loopBody);
		}

		/// <summary>
		/// ternary			: QM statement COLON statement
		///					| QM statement
		/// </summary>
		/// <param name="expressionNode"></param>
		private AstNode ParseTernary(AstNode expressionNode)
		{
			Eat(TokenType.QM);

			EatWhiteSpace();

			var ifBody = ParseStatement();

			if (CurrentToken.Type == TokenType.COLON)
			{
				Eat(TokenType.COLON);

				EatWhiteSpace();

				return new BranchAstNode(expressionNode, ifBody, ParseStatement());
			}
			
			return new BranchAstNode(expressionNode, ifBody);
		}

		/// <summary>
		/// expression			: simple_expression
		///						| simple_expression relop simple_expression
		/// </summary>
		private AstNode ParseExpression()
		{
			var node = ParseSimpleExpression();

			EatWhiteSpace();

			while (RelOp.Contains(CurrentToken.Type))
			{
				var token = CurrentToken;

				Eat(CurrentToken.Type);

				EatWhiteSpace();

				node = new BinaryOperationAstNode(token, node, ParseSimpleExpression());
			}

			return node;
		}

		/// <summary>
		/// simple_expression	: term
		///						| term addop term
		/// </summary>
		private AstNode ParseSimpleExpression()
		{
			var node = ParseTerm();

			EatWhiteSpace();

			while (AddOp.Contains(CurrentToken.Type))
			{
				var token = CurrentToken;

				Eat(CurrentToken.Type);

				EatWhiteSpace();

				node = new BinaryOperationAstNode(token, node, ParseTerm());
			}

			return node;
		}

		/// <summary>
		/// term			: factor (mulop factor)*
		/// </summary>
		private AstNode ParseTerm()
		{
			// First parse the initial factor
			var node = ParseFactor();

			EatWhiteSpace();

			while (MulOp.Contains(CurrentToken.Type))
			{
				var token = CurrentToken;

				Eat(token.Type);

				// Eat further whitespace
				EatWhiteSpace();

				node = new BinaryOperationAstNode(token, node, ParseFactor());
			}

			return node;
		}

		/// <summary>
		/// factor			: PLUS factor
		///					| MINUS factor
		///					| INTEGER_CONST
		///					| REAL_CONST
		///					| STRING_CONST
		///					| LPAREN statement RPAREN
		///					| NOT factor
		///					| variable_access
		///					| null
		///					| true
		///					| false
		/// </summary>
		private AstNode ParseFactor()
		{
			AstNode node = null;
			
			EatWhiteSpace();

			var token = CurrentToken;

			switch (token.Type)
			{
				case TokenType.PLUS:
					Eat(TokenType.PLUS);
					node = new UnaryOperationAstNode(token, ParseFactor());
					break;

				case TokenType.MINUS:
					Eat(TokenType.MINUS);
					node = new UnaryOperationAstNode(token, ParseFactor());
					break;

				case TokenType.INTEGER_CONST:
					Eat(TokenType.INTEGER_CONST);
					node = new TerminalAstNode<int>(token, int.Parse(token.Value));
					break;

				case TokenType.REAL_CONST:
					Eat(TokenType.REAL_CONST);
					node = new TerminalAstNode<double>(token, double.Parse(token.Value));
					break;

				case TokenType.TEXT_LITERAL:
					Eat(TokenType.TEXT_LITERAL);
					node = new TerminalAstNode<string>(token, token.Value);
					break;

				case TokenType.IDENTIFIER:
					node = ParseVariableAccess();
					break;

				case TokenType.NULL:
					Eat(TokenType.NULL);
					node = new NullAstNode(token);
					break;

				case TokenType.BOOL_FALSE:
					Eat(TokenType.BOOL_FALSE);
					node = new TerminalAstNode<bool>(token, false);
					break;

				case TokenType.BOOL_TRUE:
					Eat(TokenType.BOOL_TRUE);
					node = new TerminalAstNode<bool>(token, true);
					break;

				case TokenType.LPAREN:
					Eat(TokenType.LPAREN);
					node = ParseStatement();
					Eat(TokenType.RPAREN);
					break;

				case TokenType.NOT:
					Eat(TokenType.NOT);
					node = ParseFactor();
					break;
			}

			return node;
		}

		/// <summary>
		/// variable_access		: variable
		///						| procedure_call
		///						| member_access
		///
		/// member_access		: variable_access DOT variable_access
		///
		/// procedure_call		: identifier params
		///
		/// params				: LPAREN actual_param_list RPAREN
		///
		///	actual_param_list	: actual_param_list COMMA param
		///						| empty
		/// 
		/// param				: statement
		/// 
		/// variable			: identifier
		/// </summary>
		private IdentifierAstNode ParseVariableAccess(IdentifierAstNode parentNode = null)
		{
			IdentifierAstNode node;
			var token = CurrentToken;

			Eat(TokenType.IDENTIFIER);

			EatWhiteSpace();

			if (CurrentToken.Type != TokenType.LPAREN)
			{
				node = new IdentifierVariableAccessAstNode(token);
			}
			else
			{
				node = ParseProcedureCall(token);
				EatWhiteSpace();
			}

			if (CurrentToken.Type == TokenType.DOT)
			{
				Eat(TokenType.DOT);
				EatWhiteSpace();
				node = ParseVariableAccess(node);
			}

			if (parentNode != null)
			{
				parentNode.MemberIdentifier = node;
			}

			return parentNode ?? node;
		}

		private IdentifierAstNode ParseProcedureCall(Token identifierToken)
		{
			Eat(TokenType.LPAREN);
			EatWhiteSpace();

			var parameters = new List<AstNode>();

			while (CurrentToken.Type != TokenType.RPAREN)
			{
				parameters.Add(ParseStatement());

				EatWhiteSpace();

				if (CurrentToken.Type == TokenType.COMMA)
				{
					Eat(TokenType.COMMA);
					EatWhiteSpace();
				}
			}

			Eat(TokenType.RPAREN);

			return new IdentifierProcedureCallAstNode(identifierToken, parameters);
		}

		#endregion
	}
}
