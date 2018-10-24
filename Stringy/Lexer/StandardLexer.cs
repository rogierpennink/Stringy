using System;
using System.Collections.Generic;
using System.Data;

namespace Stringy.Lexer
{
	public class StandardLexer : ILexer
	{
		private static readonly IDictionary<string, Token> TokenMap = new Dictionary<string, Token>
		{
			{"{", new Token("{", TokenType.LBRACE)},
			{"}", new Token("}", TokenType.RBRACE)},
			{"(", new Token("(", TokenType.LPAREN)},
			{")", new Token(")", TokenType.RPAREN)},
			{"[", new Token("[", TokenType.LSQUARE)},
			{"]", new Token("]", TokenType.RSQUARE)},
			{"+", new Token("+", TokenType.PLUS)},
			{"-", new Token("-", TokenType.MINUS)},
			{"*", new Token("*", TokenType.MUL)},
			{"/", new Token("/", TokenType.DIV)},
			{"!", new Token("!", TokenType.NOT)},
			{"=", new Token("=", TokenType.EQ)},
			{"!=", new Token("!=", TokenType.NEQ)},
			{">", new Token(">", TokenType.GT)},
			{"<", new Token("<", TokenType.LT)},
			{">=", new Token(">=", TokenType.GTE)},
			{"<=", new Token("<=", TokenType.LTE)},
			{".", new Token(".", TokenType.DOT)},
			{",", new Token(",", TokenType.COMMA)},
			{"?", new Token("?", TokenType.QM)},
			{":", new Token(":", TokenType.COLON)},
			{"null", new Token("null", TokenType.NULL)},
			{"false", new Token("false", TokenType.BOOL_FALSE)},
			{"true", new Token("true", TokenType.BOOL_TRUE)}
		};

		private const char None = (char)0;

		private string _input;

		private int _pos;

		private char _currentChar;

		private bool _isInProgram;

		public StandardLexer(string input)
		{
			Reset(input);
		}

		public void Reset(string input)
		{
			_input = input;
			_pos = 0;
			_currentChar = _input[_pos];
			_isInProgram = false;
		}

		public void Reset()
		{
			Reset(_input);
		}

		public Token GetNextToken()
		{
			while (_currentChar != None)
			{
				// Make the current character into a string, for easier use in lookups
				var token = _currentChar + string.Empty;

				if (!_isInProgram && _currentChar != '{')
				{
					return GetOopTextLiteralToken();
				}

				if (_currentChar == '{')
				{
					_isInProgram = true;
				}

				if (_currentChar == '}')
				{
					_isInProgram = false;
				}

				if (char.IsWhiteSpace(_currentChar))
					return GetWhiteSpaceToken();

				if (char.IsDigit(_currentChar))
					return GetNumericToken();

				if (_currentChar == '\'')
					return GetTextLiteralToken();

				if (IsValidStartOfIdentifierChar(_currentChar))
					return GetIdentifierToken();

				if (TokenMap.ContainsKey(token))
				{
					var position = _pos;

					Next();

					while (TokenMap.ContainsKey(token + _currentChar))
					{
						token += _currentChar;
						Next();
					}

					var selectedToken = TokenMap[token];
					selectedToken.Position = position;
					return selectedToken;
				}

				Error();
			}

			return new Token(None.ToString(), TokenType.EOF);
		}

		/// <summary>
		/// Gathers all the whitespace until it encounters a non-white-space character and returns
		/// the gathered whitespace as a single token.
		/// </summary>
		private Token GetWhiteSpaceToken()
		{
			var position = _pos;

			var whiteSpace = string.Empty;
			while (_currentChar != None && char.IsWhiteSpace(_currentChar))
			{
				whiteSpace += _currentChar;
				Next();
			}

			return new Token(whiteSpace, TokenType.WHITESPACE, position);
		}

		/// <summary>
		/// Gets all the characters that are outside of a 'program' (a program is
		/// surrounded by {} characters.)
		/// </summary>
		private Token GetOopTextLiteralToken()
		{
			var stringLiteral = string.Empty;
			var position = _pos;

			while (_currentChar != None && _currentChar != '{')
			{
				if (_currentChar == '\\')
				{
					// If the backslash is indeed escaping something, move past the escape character
					var peekChar = Peek();
					if (peekChar == '{' || peekChar == '\\')
					{
						Next(); // Move past escape char
					}

					stringLiteral += _currentChar; // Add escaped character to string

					Next();
					continue;
				}

				stringLiteral += _currentChar;
				Next();
			}

			return new Token(stringLiteral, TokenType.TEXT_LITERAL, position);
		}

		/// <summary>
		/// Gathers all the characters that are not reserved as special tokens
		/// </summary>
		private Token GetTextLiteralToken(string stringLiteral = "")
		{
			var position = _pos;

			if (_currentChar != '\'')
				Error();

			Next();

			while (_currentChar != None && _currentChar != '\'')
			{
				// Handle an escaped quote by ignoring the escape char and moving past the
				// escaped quote itself. This can be part of a text literal whether it's
				// between quotes or not.
				if (_currentChar == '\\')
				{
					// If the backslash is indeed escaping something, move past the escape character
					if (Peek() == '\'')
					{
						Next(); // Move past escape char
					}

					stringLiteral += _currentChar; // Add quote to string

					Next(); 
					continue;
				}

				stringLiteral += _currentChar;
				Next();
			}

			// We've reached the end of the "program", if we're still between quotes,
			// raise an error. Otherwise return the text literal.
			if (_currentChar == None)
				Error($"Unterminated text literal. Did you forget a closing quote character (')?");

			// Skip the end quote
			Next();

			return new Token(stringLiteral, TokenType.TEXT_LITERAL, position);
		}

		private Token GetIdentifierToken()
		{
			var position = _pos;
			var identifier = string.Empty;

			while (IsValidStartOfIdentifierChar(_currentChar) || char.IsDigit(_currentChar))
			{
				identifier += _currentChar;
				Next();
			}

			// Test if the "identifier" exists in the token list. If so, it is a reserved
			// keyword instead of an identifier.
			if (TokenMap.ContainsKey(identifier))
			{
				var token = new Token(identifier, TokenMap[identifier].Type, position);
				return token;
			}

			return new Token(identifier, TokenType.IDENTIFIER, position);
		}

		private bool IsValidStartOfIdentifierChar(char character)
		{
			return character != None && !char.IsWhiteSpace(character) && !TokenMap.ContainsKey(character + string.Empty)
				&& (char.IsLetter(character) || character == '_');
		}

		private Token GetNumericToken()
		{
			var position = _pos;
			string number = string.Empty;
			var type = TokenType.INTEGER_CONST;

			while (_currentChar != None && char.IsDigit(_currentChar))
			{
				number += _currentChar;
				Next();
			}

			if (_currentChar == '.')
			{
				type = TokenType.REAL_CONST;
				number += _currentChar;
				Next();

				while (_currentChar != None && char.IsDigit(_currentChar))
				{
					number += _currentChar;
					Next();
				}
			}

			if (!char.IsWhiteSpace(_currentChar) && !TokenMap.ContainsKey(_currentChar + string.Empty))
			{
				return GetTextLiteralToken(number);
			}

			return new Token(number, type, position);
		}

		private void Error(string message = null)
		{
			throw new Exception(message ?? $"Invalid character '{_currentChar}' at position {_pos}");
		}

		private void Next()
		{
			_pos++;
			_currentChar = _pos >= _input.Length ? None : _input[_pos];
		}

		private char Peek()
		{
			var peekPos = _pos + 1;
			return peekPos >= _input.Length ? None : _input[peekPos];
		}

		private bool IsEof()
		{
			return _pos >= _input.Length;
		}
	}
}
