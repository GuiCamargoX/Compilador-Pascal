using Compiler.FrontEnd;
using System;
using System.Collections.Generic;

namespace Compiler
{
    class Program
    {
        private const string DefaultSourceFile = "while.pas";

        static void Main(string[] args)
        {
            string sourceFile = ResolveSourceFile(args);
            List<Token> tokens = new Scanner(sourceFile).GetTokens();
            Parser.SetTokenIterator(tokens);
            Parser.Parse();
            Console.ReadKey();
        }

        private static string ResolveSourceFile(string[] args)
        {
            if (args != null && args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
            {
                return args[0];
            }

            return DefaultSourceFile;
        }
    }
}
