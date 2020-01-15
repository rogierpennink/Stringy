using Stringy.Ast;
using Stringy.Lexer;

namespace Stringy.Parser
{
	internal interface IParser
	{
		AstNode Parse(ILexer lexer);

	    AstNode ParseStatement(ILexer lexer);

	}
}
