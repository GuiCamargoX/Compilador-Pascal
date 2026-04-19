using System;
using System.Collections.Generic;
using System.IO;
using Compiler.Tools;

namespace Compiler.FrontEnd
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

        public List<Token> GetTokens()
        {
            return getAnalex();
        }

        public static void CheckCharacter(char element) {

            switch ( TypePascal.Get( element )) {

                case TypePascal.TYPE.LETTER:

                    Word.add(element);

                    if (element == 'E' && Word.ReadingNumber)
                    {
                        Word.SciNotation = true;
                    }

                    break;

                case TypePascal.TYPE.DIGIT:

                    if (Word.isEmpty()){
                        Word.ReadingNumber = true;
                    }
                    Word.add(element);

                    break;

                case TypePascal.TYPE.SPACE:

                    if (Word.ReadingString)
                    {
                        Word.add(element);
                    }
                    else if (Word.ReadingColon)
                    {
                        string tokenType = TypePascal.GetTokenOp(Word.TokenName);
                        generateToken( tokenType );

                    }
                    else if (Word.ReadingBool)
                    {
                        string tokenType = TypePascal.GetTokenOp(Word.TokenName);
                        generateToken(tokenType);
                    }
                    else if (Word.ReadingNumber)
                    {
                        handleNumber();
                    }
                    else
                    {

                        endOfWord();

                        if (element == '\n')
                        {

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

                    if (Word.ReadingDot && element == '.')
                    {
                        if (Word.TokenName.Equals("."))
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
                        if (Word.IsFloat && element == '.')
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

                            Word.IsFloat = true;
                            Word.add(element);
                        }
                        else
                        {
                            handleNumber();

                            Word.TokenName = element.ToString() ;
                            generateToken(TypePascal.GetTokenOp( element.ToString() ) );
                        }
                    }

                    else if (Word.ReadingColon && element == '=')
                    {

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

                            endOfWord();

                            Word.TokenName = ";";
                            string tokenType = TypePascal.GetTokenOp(element.ToString());
                            generateToken(tokenType);

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
                        else if (TypePascal.OpContainsKey(element))
                        {
                            endOfWord();

                            Word.add(element);
                            string tokenType = TypePascal.GetTokenOp(element.ToString() );
                            generateToken(tokenType);
                        }
                    }
                    break;

                case TypePascal.TYPE.QUOTE:

                    Word.ReadingString = !Word.ReadingString;
                    Word.add(element);

                    if (!Word.ReadingString)
                    {

                        Word.TokenName = Word.TokenName.Substring(1, Word.TokenName.Length - 2);

                        if (Word.TokenName.Length == 1)
                        {

                            generateToken("TK_CHARLIT");
                        }
                        else if (Word.TokenName.Length > 1)
                        {

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

                generateToken("TK_FLOATLIT");
            }
            else
            {
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
                if ( Word.TokenName != "" )
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
