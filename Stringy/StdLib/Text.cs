
using System.Collections.Generic;
using System.Linq;

namespace Stringy.stdlib
{
	/// <summary>
	/// The Text class contains a set of utility methods for common text operations, such as
	/// capitalizing, enumerating, etc.
	/// </summary>
	public class Text
	{
		public string Capitalize(string value, bool ucFirst = false)
		{
			return ucFirst ? value[0].ToString().ToUpper() + value.Substring(1).ToLower() : value.ToUpper();
		}

		public string Enumerate(IEnumerable<string> values)
		{
			var strings = values.ToArray();

			if (strings.Length < 2)
				return strings.FirstOrDefault();

			if (strings.Length == 2)
				return strings[0] + " and " + strings[1];

			var part1 = string.Join(", ", strings.Take(strings.Length - 1));
			return part1 + " and " + strings[strings.Length - 1];
		}
	}
}
