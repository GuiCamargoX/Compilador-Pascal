using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace Compilador.Tools
{
    class TypePascal
    {
        public enum TYPE
        {
            LETTER, DIGIT, SPACE, OPERATOR, QUOTE
        }

        private static Dictionary<String, String> KEYWORDS_TOKEN; //Classifica o tipodeToken com sua chave 'palavra reservada' do Pascal
        private static Dictionary<String, String> OPERATORS_TOKEN;//Classifica o tipodeToken com sua chave 'operadores' do Pascal

        private static Dictionary<String, TYPE> CHAR_TYPE;/* Cria um alfabeto do Pascal,classificando em :Letter,Digit,Space,Operator,Quote
                                                            obs: Posso verificar facilmente se um caracter pertence ao alfabeto ou não */

        static TypePascal()
        {

            KEYWORDS_TOKEN = new Dictionary<string, string>();
            String word;

            using (Stream arquivo = File.Open("keywords.txt", FileMode.Open))
            using (TextReader leitor = new StreamReader(arquivo))
            {
                word = leitor.ReadLine();
                while (word != null)
                {
                    KEYWORDS_TOKEN[word] = string.Format("TK_{0}", word.ToUpper());
                    word = leitor.ReadLine();
                }
            }

            OPERATORS_TOKEN = new Dictionary<string, string>();
            OPERATORS_TOKEN["("] = "TK_OPEN_PARENTHESIS";
            OPERATORS_TOKEN[")"] = "TK_CLOSE_PARENTHESIS";
            OPERATORS_TOKEN["["] = "TK_OPEN_SQUARE_BRACKET";
            OPERATORS_TOKEN["]"] = "TK_CLOSE_SQUARE_BRACKET";
            OPERATORS_TOKEN["."] = "TK_DOT";
            OPERATORS_TOKEN[".."] = "TK_RANGE";
            OPERATORS_TOKEN[":"] = "TK_COLON";
            OPERATORS_TOKEN[";"] = "TK_SEMI_COLON";
            OPERATORS_TOKEN["+"] = "TK_PLUS";
            OPERATORS_TOKEN["-"] = "TK_MINUS";
            OPERATORS_TOKEN["*"] = "TK_MULTIPLY";
            OPERATORS_TOKEN["/"] = "TK_DIVIDE";
            OPERATORS_TOKEN["<"] = "TK_LESS_THAN";
            OPERATORS_TOKEN["<="] = "TK_LESS_THAN_EQUAL";
            OPERATORS_TOKEN[">"] = "TK_GREATER_THAN";
            OPERATORS_TOKEN[">="] = "TK_GREATER_THAN_EQUAL";
            OPERATORS_TOKEN[":="] = "TK_ASSIGNMENT";
            OPERATORS_TOKEN[","] = "TK_COMMA";
            OPERATORS_TOKEN["="] = "TK_EQUAL";
            OPERATORS_TOKEN["<>"] = "TK_NOT_EQUAL";


            CHAR_TYPE = new Dictionary<string, TYPE>();
            for (int i = 65; i < 91; i++) //converte 65-90 Unicode para letras A-Z e classifica como LETTER 
            {
                // Add letters
                char ch = Convert.ToChar(i);
                string currentChar = ch.ToString();
                CHAR_TYPE[currentChar] = TYPE.LETTER;
                CHAR_TYPE[currentChar.ToLower()] = TYPE.LETTER;
            }

            for (int i = 48; i < 58; i++)
            {
                // Add digits
                char ch = Convert.ToChar(i);
                string currentChar = ch.ToString();
                CHAR_TYPE[currentChar] = TYPE.DIGIT;
            }

            for (int i = 1; i < 33; i++)
            {
                // Add spaces
                char ch = Convert.ToChar(i);
                string currentChar = ch.ToString();
                CHAR_TYPE[currentChar] = TYPE.SPACE;
            }

            foreach (KeyValuePair<string, string> par in OPERATORS_TOKEN)
            {
                // podemos acessar a chave atual do dicionário
                string chave = par.Key;
                CHAR_TYPE[chave] = TYPE.OPERATOR;
            }

            // Add single quote (=Apostrophe = ')
            char c = Convert.ToChar(39);
            string currenChar = c.ToString();
            CHAR_TYPE[currenChar] = TYPE.QUOTE;

        }

        public static TYPE Get(char element)
        {
            return CHAR_TYPE[element.ToString()];
        }

        public static string GetTokenOp(string element) {
            return OPERATORS_TOKEN[element];
        }

        public static string GetTokenKey(string tokeName) {
            bool contain = KEYWORDS_TOKEN.ContainsKey(tokeName);

            if (contain)
                return KEYWORDS_TOKEN[tokeName];
            else return null;
        }

        public static bool OpContainsKey(char element ) {
            return OPERATORS_TOKEN.ContainsKey( element.ToString() );
        } 

        //elemento string, char
    }
}
