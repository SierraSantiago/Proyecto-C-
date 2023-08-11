 public class Program
{
 public static void Main()
    {
        string input = "2 + 5 .,;{) h ";
        Lexer lexer = new Lexer(input);

        Token token;

        do
        {
            token = lexer.GetNextToken();
            Console.WriteLine(token);
        } while (token.Type != TokenType.EOF);
    }
}