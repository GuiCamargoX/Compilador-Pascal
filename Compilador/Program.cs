﻿using Compilador.FrontEnd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador
{
    class Program
    {
        static void Main(string[] args)
        {

           List<Token> tokenArrayList =  new Scanner("while.pas").getAnalex() ;

           Parser.SetTokenArrayListIterator(tokenArrayList);

           Parser.parse();

            Console.ReadKey();
        }
    }
}
