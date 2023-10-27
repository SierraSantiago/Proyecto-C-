using System;

namespace cod
{
    class relp2
    {
        public void relp_start2()
        {
            Console.WriteLine("¡Bienvenido!");
            Console.WriteLine("Ingresa 'exit' para salir.");

            while (true)
            {
                List<string> scanned = new List<string>();

                while (true)
                {
                    Console.Write(">> ");
                    string source = Console.ReadLine();
                    scanned.Add(source);
                    Lexer lexer = new Lexer(string.Join(" ", scanned));
                    Parser parser = new Parser(lexer);
                    Program program = parser.ParseProgram();
                    Environment env = new Environment();
                    Evaluator evaluator = new Evaluator();

                    if (parser.Errors.Count > 0)
                    {
                        PrintParseErrors(parser.Errors);
                        continue;
                    }

                    Object evaluated = evaluator.Evaluate(program, env);

                    if (evaluated != null)
                    {
                        Console.WriteLine(evaluated.Inspect());
                    }

                    if (source == "salir()")
                    {
                        break;
                    }
                }
            }
        }
        public void PrintParseErrors(List<string> errors)
        {
            foreach (string error in errors)
            {
                Console.WriteLine(error);
            }
        }


    }
}