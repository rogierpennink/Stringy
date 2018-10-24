using System.Collections.Generic;
using Stringy.Lexer;

namespace Stringy.Ast
{
	internal abstract class LeafAstNode : AstNode
	{
		protected LeafAstNode(Token token)
		{
			Token = token;
		}

		public sealed override IEnumerable<AstNode> Children { get; } = new AstNode[] {};

		public override Token Token { get; }
	}
}
