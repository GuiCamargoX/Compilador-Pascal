using Compilador.FrontEnd;
using System;
using System.Collections.Generic;

namespace Compilador
{
    class Program
    {
        private const string DefaultSourceFile = "while.pas";

        static void Main(string[] args)
        {
            List<Token> tokens = new Scanner(DefaultSourceFile).GetTokens();
            Parser.SetTokenIterator(tokens);
            Parser.Parse();
            Console.ReadKey();
        }
    }
}
