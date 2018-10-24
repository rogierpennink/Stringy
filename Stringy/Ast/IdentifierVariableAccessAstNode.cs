using Stringy.Interpreter;
using Stringy.Lexer;

namespace Stringy.Ast
{
	internal class IdentifierVariableAccessAstNode : IdentifierAstNode
	{
		public IdentifierVariableAccessAstNode(Token token, IdentifierAstNode memberIdentifier = null)
			: base(token, memberIdentifier)
		{
		}

		public override object Accept(AstNodeVisitor visitor)
		{
			return visitor.Visit(this);
		}
	}
}
