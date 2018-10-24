using System.Collections.Generic;
using Stringy.Interpreter;
using Stringy.Lexer;

namespace Stringy.Ast
{
	internal abstract class AstNode
	{
		public abstract IEnumerable<AstNode> Children { get; }

		public abstract Token Token { get; }

		public abstract object Accept(AstNodeVisitor visitor);
	}
}
