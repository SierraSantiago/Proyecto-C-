using System;

namespace cod
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("¡Bienvenido al SimpleREPL!");
            Console.WriteLine("Ingresa comandos o 'exit' para salir.");

            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();

                Lexer lexer = new Lexer(input);

                Token token;

                do
                {
                    token = lexer.GetNextToken();
                    Console.WriteLine(token);
                } while (token.Type != TokenType.EOF);

                if (input == "exit")
                {
                    Console.WriteLine("¡Hasta luego!");
                    break;
                }

            }
        }

        static object Evaluate(string input)
        {
            // Aquí puedes implementar la lógica de evaluación de comandos.
            // Puedes usar un motor de scripting, evaluar expresiones matemáticas, etc.
            // Por simplicidad, este ejemplo simplemente devuelve el input como cadena.
            return input;
        }
    }
}