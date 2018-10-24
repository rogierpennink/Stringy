using System.Collections.Generic;
using System.Linq;
using Stringy.Interpreter;
using Stringy.Lexer;

namespace Stringy.Ast
{
	internal class TemplateAstNode : AstNode
	{
		public TemplateAstNode(Token token)
		{
			Token = token;
		}

		private readonly ICollection<AstNode> _children = new List<AstNode>();

		public override IEnumerable<AstNode> Children => _children;

		public override Token Token { get; }

		public void AddNode(AstNode node)
		{
			_children.Add(node);
		}

		public override object Accept(AstNodeVisitor visitor)
		{
			return visitor.Visit(this);
		}
	}
}
