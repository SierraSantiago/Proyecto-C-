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
                

                while (true)
                {
                    Console.Write(">> ");
                    string source = Console.ReadLine();
                    
                    Lexer lexer = new Lexer(source);
                    Parser parser = new Parser(lexer);
                    Program program = parser.ParseProgram();
                    Environment env = new Environment();
                    Evaluator evaluator = new Evaluator();

                    Console.WriteLine(program);

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

                    if (source == "exit")
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