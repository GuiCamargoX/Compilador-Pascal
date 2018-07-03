using System;
using System.Collections.Generic;
using System.IO;
using Compilador.Tools;

namespace Compilador.FrontEnd
{
    class Scanner
    {
        public static List<Token> tokenArrayList = new List<Token>();

        public Scanner(String namePath) {
      
            using (StreamReader MyStreamReader = new StreamReader(namePath))
            {
                char[] ch = MyStreamReader.ReadToEnd().ToLower().ToCharArray();

                for (int i = 0; i < ch.Length; i++)
                {
                    CheckCharacter(ch[i]);
                }
            }

        }

        public List<Token> getAnalex() {

            Word.clearStates();
            Word.TokenName = "EOF";
            generateToken("TK_EOF");

            return tokenArrayList;
        }

        public static void CheckCharacter(char element) {

            switch ( TypePascal.Get( element )) { //get retorna um letter,ou digit, 

                case TypePascal.TYPE.LETTER:

                    Word.add(element);

                    if (element == 'E' && Word.ReadingNumber) // se o caracter lido for E , e minha word for numero
                    {
                        Word.SciNotation = true; // diz pra minha word que ela tem notação cientifica
                    }

                    break;

                case TypePascal.TYPE.DIGIT:

                    if (Word.isEmpty()){
                        Word.ReadingNumber = true; // diz pra minha word que estou lidando com numero 
                    }
                    Word.add(element);

                    break;

                case TypePascal.TYPE.SPACE:// espaço é considerado um simbolo terminal

                    if (Word.ReadingString) //se estou lendo uma string em pascal, eu acrescento o espaço na word
                    {
                        Word.add(element);
                    }
                    else if (Word.ReadingColon) //se a word é : , então posso classifica-la e gerar um token com o seu tipo
                    {
                        string tokenType = TypePascal.GetTokenOp(Word.TokenName);
                        generateToken( tokenType );

                    }
                    else if (Word.ReadingBool)// se a word é >= , <>, ..., então posso classifica-la e gerar um token
                    {
                        string tokenType = TypePascal.GetTokenOp(Word.TokenName);
                        generateToken(tokenType);
                    }
                    else if (Word.ReadingNumber)// se a word é um numero; preciso saber se é float ou int
                    {
                        handleNumber();
                    }
                    else
                    {
                        // termina a palavra 
                        endOfWord();

                        if (element == '\n')
                        {
                            // Check for newline 
                            Word.LineRow++;
                            Word.LineCol = 0;
                        }
                        else if (element == '\t')
                        {
                            Word.LineCol += 4;
                        }
                        else if (element == ' ')
                        {
                            Word.LineCol++;
                        }
                    }
                    break;

                case TypePascal.TYPE.OPERATOR:

                    if (Word.ReadingDot && element == '.') // se a word é um ponto e o elemento é um ponto /['a'..'e'] 
                    {
                        if (Word.TokenName.Equals(".")) // se for .. então gera um token intervalo 
                        {
                            Word.add('.');
                            generateToken("TK_RANGE");
                        }                        

                    }
                    else if (Word.ReadingString)
                    {
                        Word.add(element);
                    }

                    else if (Word.ReadingNumber)
                    {
                        if (Word.IsFloat && element == '.')// EX: 10..30
                        {
                            Word.IsFloat = false;
                            Word.TokenName = Word.TokenName.Substring(0, Word.TokenName.Length - 1);
                            handleNumber();

                            Word.TokenName = "..";
                            generateToken("TK_RANGE");

                        }
                        else if (Word.SciNotation && (element == '+' || element == '-'))
                        {
                            Word.add(element);
                        }
                        else if (element == '.')
                        {
                            // Found decimal in float
                            Word.IsFloat = true;
                            Word.add(element);
                        }
                        else
                        {
                            handleNumber();
                            //caso o operador seja ex: 10; onde element== ; 
                            Word.TokenName = element.ToString() ;
                            generateToken(TypePascal.GetTokenOp( element.ToString() ) );
                        }
                    }

                    else if (Word.ReadingColon && element == '=')// caso :=
                    {
                        // Handle assignment
                        Word.add(element);

                        string tokenType = TypePascal.GetTokenOp(Word.TokenName);
                        generateToken(tokenType);

                    }
                    else if (Word.ReadingBool)
                    {
                        if (Word.TokenName.Equals("<") && ((element == '=') || (element == '>')))
                        {
                            Word.add(element);
                            string tokenType = TypePascal.GetTokenOp(Word.TokenName);
                            generateToken(tokenType);
                        }
                        else if (Word.TokenName.Equals(">") && (element == '='))
                        {
                            Word.add(element);
                            string tokenType = TypePascal.GetTokenOp(Word.TokenName);
                            generateToken(tokenType);
                        }

                        Word.ReadingBool = false;
                    }
                    else
                    {
                        if (element == ';')
                        {
                            // Before end of line
                            endOfWord();

                            Word.TokenName = ";";
                            string tokenType = TypePascal.GetTokenOp(element.ToString());
                            generateToken(tokenType);
                            //     
                        }
                        else if (element == ':')
                        {
                            endOfWord();
                            Word.ReadingColon = true;
                            Word.add(element);
                        }
                        else if (element == '<' || element == '>')
                        {
                            endOfWord();
                            Word.ReadingBool = true;
                            Word.add(element);
                        }
                        else if (element == '.')
                        {
                            Word.add(element);

                            if (Word.TokenName.Equals("end."))
                            {
                                generateToken("TK_END");
                                Word.TokenName = ".";
                                generateToken("TK_DOT");
                            }
                            else
                            {
                                Word.ReadingDot = true;
                            }
                        }
                        else if (TypePascal.OpContainsKey(element))//econtra op +,-,/,...
                        {
                            endOfWord();

                            Word.add(element);
                            string tokenType = TypePascal.GetTokenOp(element.ToString() );
                            generateToken(tokenType);
                        }
                    }
                    break;

                case TypePascal.TYPE.QUOTE:
                    // Found begin/end quote
                    Word.ReadingString = !Word.ReadingString;
                    Word.add(element);

                    if (!Word.ReadingString)
                    {
                        // Remove trailing quotes
                        Word.TokenName = Word.TokenName.Substring(1, Word.TokenName.Length - 2);

                        // Found end quote
                        if (Word.TokenName.Length == 1)
                        {
                            //                        System.out.println("TK_CHARLIT: " + tokenName);
                            generateToken("TK_CHARLIT");
                        }
                        else if (Word.TokenName.Length > 1)
                        {
                            //                        System.out.println("TK_STRLIT: " + tokenName);
                            generateToken("TK_STRLIT");
                        }
                    }
                    break;
                default:
                    throw new Exception("Unhandled element scanned");
            }
            


        }



        public static void handleNumber()
        {
           
            if (Word.IsFloat)
            {
                //            System.out.println("TK_FLOATLIT: " + tokenName);
                generateToken("TK_FLOATLIT");
            }
            else
            {                //            System.out.println("TK_INTLIT: " + tokenName);
                generateToken("TK_INTLIT");
            }
        }

        public static void generateToken(string tokenType)
        {
            Token t = new Token(tokenType, Word.TokenName , Word.LineCol, Word.LineRow);
            tokenArrayList.Add(t);
            Console.WriteLine("tokenType: "+ t.TokenType+ "  Lexema: " + t.TokenValue+ "  LineCol: "+ t.LineCol + "  LineRow: "+ t.LineRow);

            Word.LineCol += Word.TokenName.Length;

            Word.clearStates();
        }


        public static void endOfWord()
        {
            string wordReserved = TypePascal.GetTokenKey(Word.TokenName);
            if ( wordReserved != null )
            {
                generateToken(wordReserved);
            }
            else
            {
                if ( Word.TokenName != "" )// filtra words vazias
                {

                    if (Word.TokenName.Equals("true") || Word.TokenName.Equals("false"))
                    {
                        generateToken("TK_BOOLLIT");
                    }
                    else
                    {
                        generateToken("TK_IDENTIFIER");
                    }
                }
            }

            Word.clearStates();
        }

    }
}
