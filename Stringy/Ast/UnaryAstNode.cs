using System.Collections.Generic;
using Stringy.Lexer;

namespace Stringy.Ast
{
	internal abstract class UnaryAstNode : AstNode
	{
		private ICollection<AstNode> _children;

		protected UnaryAstNode(Token token)
		{
			Token = token;
		}

		public AstNode Child { get; set; }

		public override IEnumerable<AstNode> Children => _children ?? (_children = new List<AstNode> {Child});

		public override Token Token { get; }
	}
}
