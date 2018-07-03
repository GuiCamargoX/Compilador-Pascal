using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador.FrontEnd
{
    public class Parser
    {
        public enum TYPE
        {
            I, R, B, C, S, P, L, A     // integer, real, boolean, char, string, procedure, label, array
        }
        private static int dp = 0; // data pointer for vars

        private static Dictionary<String, TYPE> STRING_TYPE_HASH_MAP;
        private static readonly int ADDRESS_SIZE = 4;

        private static Token currentToken;
        private static IEnumerator<Token> it;

        private static readonly int INSTRUCTION_SIZE = 1000;

        private static Byte[] byteArray = new Byte[INSTRUCTION_SIZE];
        private static int s = -1;

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

            block();

            return byteArray;
        }


        public static void block()
        {
            if( "TK_LABEL".Equals(currentToken.TokenType) )
                labelDeclarations();
                
            if("TK_VAR".Equals( currentToken.TokenType ) )
                varDeclarations();
                   

            while ("TK_PROCEDURE".Equals(currentToken.TokenType) || "TK_FUNCTION".Equals(currentToken.TokenType)) {
                switch (currentToken.TokenType) {

                    case "TK_PROCEDURE":
                        procDeclaration();
                        break;

                    case "TK_FUNCTION":
                        funcDeclaration();
                        break;
                }
            }

            comando_composto();
        }




        public static void getToken()
        {
            if (it.MoveNext())
            {
                currentToken = it.Current;
            }
        }

        public static void SetTokenArrayListIterator(List<Token> tokenArrayList)
        {
            it = tokenArrayList.GetEnumerator();
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
