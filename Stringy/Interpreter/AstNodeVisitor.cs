using Stringy.Ast;

namespace Stringy.Interpreter
{
	internal abstract class AstNodeVisitor
	{
		public virtual object Visit(TemplateAstNode node)
		{
			return null;
		}

		public virtual object Visit(BinaryAstNode node)
		{
			return null;
		}

		public virtual object Visit(BinaryOperationAstNode node)
		{
			return null;
		}

		public virtual object Visit(BranchAstNode node)
		{
			return null;
		}

		public virtual object Visit(EachLoopAstNode node)
		{
			return null;
		}

		public virtual object Visit(IdentifierAstNode node)
		{
			return null;
		}

		public virtual object Visit<TType>(TerminalAstNode<TType> node)
		{
			return null;
		}

		public virtual object Visit(IdentifierVariableAccessAstNode node)
		{
			return null;
		}

		public virtual object Visit(IdentifierProcedureCallAstNode node)
		{
			return null;
		}

		public virtual object Visit(NoOpAstNode node)
		{
			return null;
		}

		public virtual object Visit(TerminalAstNode<string> node)
		{
			return null;
		}

		public virtual object Visit(TerminalAstNode<int> node)
		{
			return null;
		}

		public virtual object Visit(TerminalAstNode<double> node)
		{
			return null;
		}

		public virtual object Visit(TerminalAstNode<bool> node)
		{
			return null;
		}

		public virtual object Visit(NullAstNode node)
		{
			return null;
		}

		public virtual object Visit(UnaryOperationAstNode node)
		{
			return null;
		}
	}
}
