namespace Stringy.Lexer
{
	public interface ILexer
	{
		Token GetNextToken();

		void Reset(string input, bool treatInputAsString = false);

		void Reset();
	}
}
