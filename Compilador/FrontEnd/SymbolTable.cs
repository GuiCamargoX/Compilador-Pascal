using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador.FrontEnd
{
    public class SymbolTable
    {
        private static readonly int HASH_TABLE_SIZE = 211;

        public class Scope
        {
            public Symbol[] symbolTable = new Symbol[HASH_TABLE_SIZE]; // symbol table for the current scope
            public Scope next { get; set; } = null; // pointer to the next outer scope
        }

        private static Scope headerScope = new Scope();
        public static int nivel_corrente{ get; private set; } = 0;

        public static void Insere(Symbol symbol)
        {
            int hashValue = hash(symbol.getName());
            symbol.nivel_corrente = nivel_corrente;

            Symbol bucketCursor = headerScope.symbolTable[hashValue];
            if (bucketCursor == null)
            {
                // Empty bucket
                headerScope.symbolTable[hashValue] = symbol;
            }
            else
            {
                // Existing Symbols in bucket
                while (bucketCursor.next != null)
                {
                    bucketCursor = bucketCursor.next;
                }

                // Append symbol at the end of the bucket
                bucketCursor.next = symbol;
            }
        }

        public static Symbol Busca(String symbolName)
        {
            int hashValue = hash(symbolName);
            Symbol bucketCursor = headerScope.symbolTable[hashValue];
            Scope scopeCursor = headerScope;

            while (scopeCursor != null)
            {
                while (bucketCursor != null)
                {
                    if (bucketCursor.getName().Equals(symbolName))
                    {
                        return bucketCursor;
                    }
                    bucketCursor = bucketCursor.next;
                }
                scopeCursor = scopeCursor.next;
            }

            // Symbol does not exist
            return null;
        }

        public static int hash(String symbolName)
        {
            int h = 0;
            for (int i = 0; i < symbolName.Length; i++)
            {
                h = h + h + symbolName[i];
            }

            h = h % HASH_TABLE_SIZE;

            return h;
        }

        public static void openScope()
        {
            Scope innerScope = new Scope();

            nivel_corrente++;

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
    }
}
