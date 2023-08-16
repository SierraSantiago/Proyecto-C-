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

                Lexer lexer = new Lexer(input);
                
                Token token = lexer.GetNextToken();

               if (token.LookupTokenType(input) != TokenType.IDENT)
                {
                 Console.WriteLine("Token(" + token.LookupTokenType(input) + "," + input + ")");
                 }
                else if (input == "exit")
             {
        Console.WriteLine("¡Hasta luego!");
        break;
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