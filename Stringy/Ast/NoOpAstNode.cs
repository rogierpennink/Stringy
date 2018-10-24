using Stringy.Interpreter;
using Stringy.Lexer;

namespace Stringy.Ast
{
	internal class NoOpAstNode : LeafAstNode
	{
		public NoOpAstNode(Token token) : base(token)
		{
		}

		public override object Accept(AstNodeVisitor visitor)
		{
			return visitor.Visit(this);
		}
	}
}
