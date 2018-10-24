using System.Collections.Generic;
using Stringy.Interpreter;
using Stringy.Lexer;

namespace Stringy.Ast
{
	internal class IdentifierProcedureCallAstNode : IdentifierAstNode
	{
		private readonly List<AstNode> _params = new List<AstNode>();

		public IdentifierProcedureCallAstNode(Token token, IdentifierAstNode memberIdentifier = null)
			: base(token, memberIdentifier)
		{
		}

		public IdentifierProcedureCallAstNode(Token token, IEnumerable<AstNode> paramsList, IdentifierAstNode memberIdentifier = null)
			: base(token, memberIdentifier)
		{
			_params.AddRange(paramsList);
		}

		public IEnumerable<AstNode> Params => _params;

		public override object Accept(AstNodeVisitor visitor)
		{
			return visitor.Visit(this);
		}
	}
}
