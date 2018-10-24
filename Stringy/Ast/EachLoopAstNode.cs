using System.Collections.Generic;
using Stringy.Interpreter;
using Stringy.Lexer;

namespace Stringy.Ast
{
	internal class EachLoopAstNode : AstNode
	{
		public EachLoopAstNode(Token token, IdentifierAstNode loopVariableNode, IdentifierAstNode enumerableVariableNode,
			AstNode loopBodyNode)
		{
			Token = token;
			LoopVariableNode = loopVariableNode;
			EnumerableVariableNode = enumerableVariableNode;
			LoopBodyNode = loopBodyNode;
		}

		public IdentifierAstNode LoopVariableNode { get; }

		public IdentifierAstNode EnumerableVariableNode { get; }

		public AstNode LoopBodyNode { get; }

		#region AstNode implementation

		public override Token Token { get; }

		public override IEnumerable<AstNode> Children { get; } = new AstNode[] {};

		public override object Accept(AstNodeVisitor visitor)
		{
			return visitor.Visit(this);
		}

		#endregion
	}
}
