using System.Collections.Generic;
using Stringy.Interpreter;
using Stringy.Lexer;

namespace Stringy.Ast
{
	internal class BranchAstNode : AstNode
	{
		private readonly AstNode[] _children = new AstNode[3];

		public BranchAstNode(AstNode condition, AstNode ifBody)
		{
			_children[0] = condition;
			_children[1] = ifBody;
			_children[2] = new NoOpAstNode(condition.Token);
		}

		public BranchAstNode(AstNode condition, AstNode ifBody, AstNode elseBody)
		{
			_children[0] = condition;
			_children[1] = ifBody;
			_children[2] = elseBody;
		}

		public AstNode Condition
		{
			get { return _children[0]; }
			set { _children[0] = value; }
		}

		public AstNode IfBody
		{
			get { return _children[1]; }
			set { _children[1] = value; }
		}

		public AstNode ElseBody
		{
			get { return _children[2]; }
			set { _children[2] = value; }
		}

		public override IEnumerable<AstNode> Children => _children;

		public override Token Token => Condition.Token;

		public override object Accept(AstNodeVisitor visitor)
		{
			return visitor.Visit(this);
		}
	}
}
