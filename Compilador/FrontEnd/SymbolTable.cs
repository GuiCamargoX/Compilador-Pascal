using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador.FrontEnd
{
    public class SymbolTable
    {
        public class Scope
        {
            public Dictionary<string, Symbol> symbolTable = new Dictionary<string, Symbol>(); // symbol table for the current scope
            public Scope next { get; set; } = null; // pointer to the next outer scope
        }

        private static Scope headerScope = new Scope();
        public static int nivel_corrente{ get; private set; } = 0;
        private static string[] QteVarScope = new string[100] ;

        public static void Insere(Symbol symbol)
        {
            symbol.nivel_corrente = nivel_corrente;

            headerScope.symbolTable[symbol.getName()] = symbol;
   
        }

        public static Symbol Busca(String symbolName)
        {
            Scope scopeCursor = headerScope;

            while (scopeCursor != null)
            {
               
                if ( scopeCursor.symbolTable.ContainsKey(symbolName) )
                {
                   return scopeCursor.symbolTable[symbolName];
                }

                scopeCursor = scopeCursor.next;
            }

            // Symbol does not exist
            return null;
        }

        public static void openScope()
        {
            Scope innerScope = new Scope();

            nivel_corrente++;
            QteVarScope[nivel_corrente] = "0";

            // Add new scope to the headerScope
            innerScope.next = headerScope;

            // Move headerScope to the front of the Scope linked list
            headerScope = innerScope;
        }

        public static void closeScope()
        {
            headerScope = headerScope.next;
            nivel_corrente--;
        }

        public static Scope getHeaderScope()
        {
            return headerScope;
        }

        public static void setQteVariaveis(string n) {
            QteVarScope[nivel_corrente] = n; 
        }

        public static string getQteVariaveis() {
            return QteVarScope[nivel_corrente];
        }

    }
}
