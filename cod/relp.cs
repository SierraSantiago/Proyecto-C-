
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
                Parser parser = new Parser(lexer);
                Evaluator eva = new Evaluator();
                Environment env = new Environment();
                var programa = parser.ParseProgram();
                Console.WriteLine(programa);
                
                
                

                
                 
                Object evaluated = eva.Evaluate(programa, env);
                Console.WriteLine(input+":input");
                
                
                
               

                 if (evaluated != null)
                {
                    Console.WriteLine(evaluated.Inspect());
                }
                 

            }
        }
    }
}