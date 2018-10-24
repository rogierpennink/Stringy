using System;
using System.Collections.Generic;

namespace Stringy.Interpreter
{
	internal class VariableContainer
	{
		public Type Type;

		public object Value;
	}

	public class SymbolTable : ISymbolTable
	{
		private readonly IDictionary<string, VariableContainer> _variableRegistry = new Dictionary<string, VariableContainer>();

		public object Get(string key)
		{
			if (_variableRegistry.ContainsKey(key))
			{
				var container = _variableRegistry[key];
				return container.Value;
			}

			return null;
		}

		public void Set<TType>(string variableName, TType value)
		{
			Set(variableName, (object)value);
		}

		public void Set(string variableName, dynamic value)
		{
			// Register class if it hasn't previously been registered
			var type = value.GetType();

			VariableContainer container;

			// Check if a variable container already exists
			if (_variableRegistry.ContainsKey(variableName))
			{
				container = _variableRegistry[variableName];
				if (container.Type != type)
				{
					throw new InvalidOperationException($"Cannot store value of type {type} in variable of type {container.Type}");
				}
			}
			else
			{
				container = new VariableContainer
				{
					Type = type
				};
				_variableRegistry[variableName] = container;
			}

			container.Value = value;
		}

		public bool Has(string key)
		{
			return _variableRegistry.ContainsKey(key);
		}
	}
}
