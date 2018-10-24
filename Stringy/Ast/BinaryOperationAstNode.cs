using Stringy.Interpreter;
using Stringy.Lexer;

namespace Stringy.Ast
{
	internal class BinaryOperationAstNode : BinaryAstNode
	{
		public BinaryOperationAstNode(Token token, AstNode left, AstNode right) : base(left, right)
		{
			Operation = token;
		}

		public Token Operation { get; }

		public override Token Token => Operation;

		public override object Accept(AstNodeVisitor visitor)
		{
			return visitor.Visit(this);
		}
	}
}
