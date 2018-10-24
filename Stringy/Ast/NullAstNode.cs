using Stringy.Interpreter;
using Stringy.Lexer;

namespace Stringy.Ast
{
	internal class NullAstNode : TerminalAstNode
	{
		public NullAstNode(Token token) : base(token)
		{
		}

		public override object Accept(AstNodeVisitor visitor)
		{
			return visitor.Visit(this);
		}
	}
}
