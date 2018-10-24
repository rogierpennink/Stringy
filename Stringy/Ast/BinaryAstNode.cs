using System.Collections.Generic;
using System.Linq;
using Stringy.Interpreter;

namespace Stringy.Ast
{
	internal abstract class BinaryAstNode : AstNode
	{
		private ICollection<AstNode> _children;

		protected BinaryAstNode(AstNode left, AstNode right)
		{
			Left = left;
			Right = right;
		}

		public AstNode Left { get; set; }

		public AstNode Right { get; set; }

		public override IEnumerable<AstNode> Children => _children ?? (_children = new List<AstNode> {Left, Right});

		public override object Accept(AstNodeVisitor visitor)
		{
			return visitor.Visit(this);
		}
	}
}
