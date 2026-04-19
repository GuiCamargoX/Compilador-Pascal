using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.FrontEnd
{
    public partial class Parser
    {
        public enum TYPE
        {
            I, R, B, C, S, P, L, A     // integer, real, boolean, char, string, procedure, label, array
        }
        private static StreamWriter file=null;
        private static Stream saida=null;

        private static Dictionary<String, TYPE> STRING_TYPE_HASH_MAP;

        private static int dp = 0; // data pointer for vars
        private static Token currentToken;
        private static IEnumerator<Token> it;

        private static readonly int INSTRUCTION_SIZE = 1000;

        private static Byte[] byteArray = new Byte[INSTRUCTION_SIZE];
        private static int qte_label = 1;

        static Parser()
        {
            STRING_TYPE_HASH_MAP = new Dictionary<String, TYPE>
            {
        ["integer"] = TYPE.I,
        ["real"] = TYPE.R,
        ["boolean"] = TYPE.B,
        ["char"] = TYPE.C,
        ["string"] = TYPE.S,
        ["array"] = TYPE.A
            };

        }

        public static Byte[] parse()
        {
            openFileOutput();
        
            getToken(); // Get initial token
        
        
            match("TK_PROGRAM");
            match("TK_IDENTIFIER");
            match("TK_OPEN_PARENTHESIS");
            match("TK_IDENTIFIER");
        
            while ( "TK_COMMA".Equals(currentToken.TokenType)){
                match("TK_COMMA");
                match("TK_IDENTIFIER");
            }
        
            match("TK_CLOSE_PARENTHESIS");
        
            match("TK_SEMI_COLON");
        
            GenerateMepa("","INPP","");
          
            block();
        
            match("TK_DOT");
            match("TK_EOF");
        
           
            file.Close();
            saida.Close();
        
            return byteArray;
        }

        public static Byte[] Parse()
        {
            return parse();
        }

        private static void openFileOutput()
        {
            try
            {
                saida = File.Open("Mepa.txt", FileMode.Create);
                file = new StreamWriter(saida);
            }
            catch (Exception) { }
        
        }

        public static void block()
        {
        
            while ( !"TK_BEGIN".Equals(currentToken.TokenType) ) {
                Console.Write(currentToken.TokenValue);
                switch ( currentToken.TokenType ) {
        
                    case "TK_LABEL":
                        labelDeclarations();
                        break;
        
                    case "TK_VAR":
                        VarDeclarations();
                        break;
        
                    case "TK_PROCEDURE":
                        procDeclaration();
                        break;
                    
                }
                
            }
        
            comando_Begin();
            if (Int32.Parse(SymbolTable.GetVariableCount()) > 0)
                GenerateMepa("", "DMEM", SymbolTable.GetVariableCount());
        
        }

        public static void comando_Begin()
        {
            match("TK_BEGIN");
            statements();
        
            while ("TK_SEMI_COLON".Equals(currentToken.TokenType))
            {
                match("TK_SEMI_COLON");
                statements();
            }
        
            match("TK_END");
        }

        public static void GenerateMepa(string label , string code, string par1) {
            file.WriteLine(label + " "+ code + " " + par1);
        }

        public static string Next_Label() {
            string l = "L" + qte_label;
            qte_label++;
            return l;
        }

        public static void getToken()
        {
            Symbol symb;
            
            if (it.MoveNext())
            {
                currentToken = it.Current;
        
                if ("TK_IDENTIFIER".Equals(currentToken.TokenType))
                {
                    symb = SymbolTable.Lookup(currentToken.TokenValue);
        
                    if (symb != null)
                        currentToken.TokenType = symb.getTokenType();
                }
            }
        }

        public static void SetTokenArrayListIterator(List<Token> tokenArrayList)
        {
            it = tokenArrayList.GetEnumerator();
        }

        public static void SetTokenIterator(List<Token> tokenArrayList)
        {
            SetTokenArrayListIterator(tokenArrayList);
        }

        public static void match(String tokenType)
        {
            if ( !tokenType.Equals(currentToken.TokenType) )
            {
                throw new Exception(String.Format("Token type (%s) does not match current token type (%s)", tokenType, currentToken.TokenType));
            }
            else
            {
                //            System.out.println(String.format("matched: %s", currentToken.getTokenType()));
                getToken();
            }
        }
    }
}
