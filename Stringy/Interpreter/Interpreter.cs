using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using Stringy.Ast;
using Stringy.Lexer;
using Stringy.Parser;
using Stringy.stdlib;

namespace Stringy.Interpreter
{
	internal class Interpreter : AstNodeVisitor, IInterpreter
	{
		private IParser Parser { get; }

		private ISymbolTable SymbolTable { get; }

		private ErrorMode CurrentErrorMode { get; set; }

		public Interpreter(IParser parser, ISymbolTable symbolTable)
		{
			Parser = parser;
			SymbolTable = symbolTable;
			CurrentErrorMode = ErrorMode.ThrowExceptions;

			IncludeStandardLibraries();
		}

		private void IncludeStandardLibraries()
		{
			SymbolTable.Set("Text", new Text());
		}

		public override object Visit(TemplateAstNode node)
		{
			var stringbuilder = new StringBuilder();

			// Concatenate the results of the child nodes
			foreach (var childNode in node.Children)
			{
				object fragment = null;

				if (CurrentErrorMode == ErrorMode.ThrowExceptions)
				{
					fragment = childNode.Accept(this);
				}
				else
				{
					try
					{
						fragment = childNode.Accept(this);
					}
					catch(Exception e)
					{
						if (CurrentErrorMode == ErrorMode.SubstituteExceptions)
							fragment = e.Message;
					}
				}

				stringbuilder.Append(fragment);
			}

			return stringbuilder.ToString();
		}

		public override object Visit(UnaryOperationAstNode node)
		{
			dynamic value = node.Child.Accept(this);

			switch (node.Operation.Type)
			{
				case TokenType.MINUS:
					return value * -1;
				case TokenType.PLUS:
					return value * 1;
			}

			throw Error(node);
		}

		public override object Visit(BinaryOperationAstNode node)
		{
			var leftVal = (dynamic)node.Left.Accept(this);
			var rightVal = (dynamic)node.Right.Accept(this);

			switch (node.Operation.Type)
			{
				// Relop
				case TokenType.EQ:
					return leftVal == rightVal;
				case TokenType.NEQ:
					return leftVal != rightVal;
				case TokenType.GT:
					return leftVal > rightVal;
				case TokenType.LT:
					return leftVal < rightVal;
				case TokenType.GTE:
					return leftVal >= rightVal;
				case TokenType.LTE:
					return leftVal <= rightVal;

				// Addop
				case TokenType.OR:
					return (bool)leftVal || (bool)rightVal;
				case TokenType.PLUS:
					return leftVal + rightVal;
				case TokenType.MINUS:
					return leftVal - rightVal;

				// Mulop
				case TokenType.AND:
					return (bool)leftVal && (bool)rightVal;
				case TokenType.MUL:
					return leftVal * rightVal;
				case TokenType.DIV:
					return leftVal / rightVal;
			}

			throw Error(node);
		}

		public override object Visit(BranchAstNode node)
		{
			var booleanResult = (bool)node.Condition.Accept(this);
			if (booleanResult)
			{
				return node.IfBody.Accept(this);
			}

			// TODO: Hack!
			if (node.ElseBody is NoOpAstNode)
			{
				return "";
			}

			return node.ElseBody.Accept(this);
		}

		public override object Visit(EachLoopAstNode node)
		{
			// First check that the loop variable doesn't already exist
			if (SymbolTable.Has(node.LoopVariableNode.Name))
				throw new Exception($"Loop variable {node.LoopVariableNode.Name} at position {node.LoopVariableNode.Token.Position} is already defined!");

			// Verify that the enumerable variable exists and is an enumerable
			var enumerable = SymbolTable.Get(node.EnumerableVariableNode.Name);
			var type = enumerable.GetType();
			if (!typeof(IEnumerable).IsAssignableFrom(type))
				throw new Exception($"{node.EnumerableVariableNode.Name} is not an Enumerable object at position {node.EnumerableVariableNode.Token.Position}");

			// Concatenate output from the loop body
			var stringBuilder = new StringBuilder();

			foreach (var item in (IEnumerable)enumerable)
			{
				SymbolTable.Set(node.LoopVariableNode.Name, item);
				stringBuilder.Append(node.LoopBodyNode.Accept(this));
			}

			return stringBuilder.ToString();
		}

		public override object Visit<TType>(TerminalAstNode<TType> node)
		{
			return node.Value;
		}

		public override object Visit(NullAstNode node)
		{
			return null;
		}

		public override object Visit(IdentifierVariableAccessAstNode idNode)
		{
			var node = (IdentifierAstNode)idNode;
			object value = SymbolTable.Get(node.Name);

			while (node.MemberIdentifier != null)
			{
				node = node.MemberIdentifier;
				var procNode = node as IdentifierProcedureCallAstNode;

				// If procNode is null we assume it's a property
				if (procNode == null)
				{
					var property = value.GetType().GetProperty(node.Name);
					value = property.GetValue(value);
				}
				else
				{
					var paramList = procNode.Params.Select(p => p.Accept(this)).ToList();
					//var paramTypes = paramList.Select(p => p.GetType()).ToList();

					// First get all methods because we might be calling one with optional parameters
					// which means we need to take those into account with the param types.
					MethodInfo method = null;
					var methods = value.GetType().GetMethods().Where(m => m.Name == node.Name)
						.OrderBy(m => m.GetParameters().Length - m.GetParameters().Count(x => x.IsOptional))
						.ToArray();

					for (var i = 0; i < methods.Length; i++)
					{
						var parameters = methods[i].GetParameters();
						if (parameters.Length >= paramList.Count)
						{
							method = methods[i];
							break;
						}
					}

					if (method.GetParameters().Length > paramList.Count)
					{ 
						paramList.AddRange(Enumerable.Range(1, method.GetParameters().Length - paramList.Count).Select(x => Type.Missing));
					}
					
					//var method = value.GetType().GetMethod(node.Name, paramTypes);
					value = method.Invoke(value, paramList.ToArray());
				}
			}

			return value;
		}

		public override object Visit(IdentifierProcedureCallAstNode procNode)
		{
			object value = SymbolTable.Get(procNode.Name);
			var type = value.GetType();

			var paramList = procNode.Params.Select(p => p.Accept(this)).ToArray();
			var paramTypes = paramList.Select(p => p.GetType()).ToArray();

			var anyMethod = type.GetMethod("Invoke");
			if (anyMethod == null)
				throw Error(procNode, $"'{procNode.Name}' is not an Invokable procedure.");

			var method = type.GetMethod("Invoke", paramTypes);
			if (method == null)
				throw Error(procNode, $"'{procNode.Name}' called with invalid arguments.");

			return method.Invoke(value, paramList);
		}

		public string Interpret(string template, ErrorMode errMode = ErrorMode.ThrowExceptions)
		{
			CurrentErrorMode = errMode;

			var tree = Parser.Parse(new StandardLexer(template));
			return tree.Accept(this).ToString();
		}
		
		private Exception Error(AstNode node, string message = null)
		{
			message = $"{{Runtime Exception: {node.GetType().Name}, token {node.Token.Value} at position {node.Token.Position}" +
					  (message != null ? "\n" + message : string.Empty) + "}}";
			return new Exception(message);
		}
	}
}
