using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Stringy.Interpreter;
using Stringy.Lexer;

namespace Stringy.Ast
{
	internal class IdentifierAstNode : AstNode
	{
		private readonly ICollection<IdentifierAstNode> _children = new Collection<IdentifierAstNode>();

		public IdentifierAstNode(Token token, IdentifierAstNode memberIdentifier = null)
		{
			Token = token;
			Name = token.Value;

			if (memberIdentifier != null)
			{
				MemberIdentifier = memberIdentifier;
				_children.Add(MemberIdentifier);
			}
		}

		public string Name { get; }

		public IdentifierAstNode MemberIdentifier
		{
			get => _children.FirstOrDefault();
		    set
			{
				_children.Clear();
				_children.Add(value);
			}
		}

		public override IEnumerable<AstNode> Children => _children;

		public override Token Token { get; }

		public override object Accept(AstNodeVisitor visitor)
		{
			return visitor.Visit(this);
		}
	}
}
