using System;

namespace cod
{
    class Relp
    {
        public void relp_start()
        {
            Console.WriteLine("¡Bienvenido!");
            Console.WriteLine("Ingresa 'exit' para salir.");

            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();

                if (input == "exit")
                {
                    Console.WriteLine("¡Hasta luego!");
                    break;  // Sale del bucle si se ingresa "exit"
                }

                Lexer lexer = new Lexer(input);

                Token token = lexer.GetNextToken();

                foreach (string str in token.SplitString(input))
                {
                    if (token.LookupTokenType(str) != TokenType.IDENT)
                    {
                        Console.WriteLine("Token(" + token.LookupTokenType(str) + "," + str + ")");
                    }
                    else
                    {
                        do
                        {
                            Console.WriteLine(token);
                            token = lexer.GetNextToken();
                        } while (token.Type != TokenType.END);
                    }
                }
            }
        }
    }
}