using System.Collections.Generic;

namespace Stringy.Lexer
{
	public enum TokenType
	{
		// Special constants
		EOF				= 0,
		WHITESPACE		= 1,

		// Numeric Constants
		REAL_CONST		= 20,
		INTEGER_CONST,

		// Other constants
		TEXT_LITERAL	= 30,

		IDENTIFIER		= 50,

		COMMA			= 80,

		// Grouping, order-of-precedence
		LPAREN			= 100,
		RPAREN,
		LBRACE,
		RBRACE,
		LSQUARE,
		RSQUARE,
	
		// Escaping (identifying strings etc) 
		SQUOT			= 120,

		// Mathematical operators
		PLUS			= 150,
		MINUS,
		MUL,
		DIV,

		// Relational operators
		NOT				= 180,
		EQ,
		NEQ,
		GT,
		LT,
		GTE,
		LTE,

		//Logical operators
		AND				= 200,
		OR,

		// Bitwise operators?

		// Accessor operators
		DOT				= 220,

		// Control tokens
		QM = 300,
		COLON,

		// Special identifiers
		NULL			= 400,

		BOOL_FALSE		= 410,
		BOOL_TRUE		= 411
	}

	public class Token
	{
		public Token()
		{
		}

		public Token(string value, TokenType type, int position = 0)
		{
			Value = value;
			Type = type;
			Position = position;
		}

		public Token(char value, TokenType type, int position = 0) : this(string.Empty + value, type, position)
		{
		}

		public string Value;

		public TokenType Type;

		public int Position;

		public static bool operator ==(Token token1, Token token2)
		{
			if ((object)token1 == null || (object)token2 == null)
				return false;

			// Position doesn't count
			return token1.Value == token2.Value && token1.Type == token2.Type;
		}

		public static bool operator !=(Token token1, Token token2)
		{
			return !(token1 == token2);
		}
	}
}
