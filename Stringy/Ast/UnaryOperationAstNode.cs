using Stringy.Interpreter;
using Stringy.Lexer;

namespace Stringy.Ast
{
	internal class UnaryOperationAstNode : UnaryAstNode
	{
		public UnaryOperationAstNode(Token token, AstNode childNode) : base(token)
		{
			Operation = token;
			Child = childNode;
		}

		public Token Operation { get; }

		public override object Accept(AstNodeVisitor visitor)
		{
			return visitor.Visit(this);
		}
	}
}
