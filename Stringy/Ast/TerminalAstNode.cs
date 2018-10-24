using System.Collections.Generic;
using Stringy.Interpreter;
using Stringy.Lexer;

namespace Stringy.Ast
{
	/// <summary>
	/// Represents a Terminal; this is a leaf, it has no children.
	/// </summary>
	internal abstract class TerminalAstNode : LeafAstNode
	{
		protected TerminalAstNode(Token token) : base(token)
		{
		}
	}

	
	internal class TerminalAstNode<TType> : TerminalAstNode
	{
		public TType Value { get; }

		public TerminalAstNode(Token token, TType value) : base(token)
		{
			Value = value;
		}

		public override object Accept(AstNodeVisitor visitor)
		{
			return visitor.Visit(this);
		}
	}
}
