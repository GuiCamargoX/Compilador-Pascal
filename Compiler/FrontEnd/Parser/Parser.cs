using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.FrontEnd
{
    public class Parser
    {
        public enum TYPE
        {
            I, R, B, C, S, P, L, A
        }
        private static StreamWriter file=null;
        private static Stream saida=null;

        private static Dictionary<String, TYPE> STRING_TYPE_HASH_MAP;

        private static int dp = 0;
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

            getToken();

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

        public static void statements()
        {
                switch (currentToken.TokenType)
                {
                    case "TK_CASE":
                       caseStat();
                        break;
                    case "TK_GOTO":
                        goToStat();
                        break;
                    case "TK_WHILE":
                        whileStat();
                        break;
                    case "TK_REPEAT":
                        repeatStat();
                        break;
                    case "TK_FOR":
                        forStat();
                        break;
                    case "TK_IF":
                        ifStat();
                        break;
                    case "TK_WRITELN":
                        writeStat();
                        break;
                    case "TK_WRITE":
                        writeStat();
                        break;
                    case "TK_READLN":
                        readStat();
                        break;
                    case "TK_READ":
                        readStat();
                        break;
                    case "TK_VAIDEN":
                        assignmentStat();
                        break;
                    case "TK_A_PROC":
                        procedureStat();
                        break;
                    case "TK_INTLIT":
                        labelStat();
                        break;
                    case "TK_BEGIN":
                        comando_Begin();
                        break;
                    default:
                        return;
                }
        }

        public static void procedureStat()
        {
            Symbol symbol = SymbolTable.Lookup(currentToken.TokenValue);
            match("TK_A_PROC");
            Encoding u8 = Encoding.UTF8;
            byte[] bytes = BitConverter.GetBytes(symbol.getAddress());
            string l1 = u8.GetString(bytes);
            GenerateMepa("", "CHPR", l1);

        }

        public static void labelStat() {

            Symbol symbol = SymbolTable.Lookup( currentToken.TokenValue );

            Encoding u8 = Encoding.UTF8;
            byte[] bytes = BitConverter.GetBytes( symbol.getAddress() );
            string l1 = u8.GetString(bytes);

            if (SymbolTable.CurrentScopeLevel != symbol.ScopeLevel)
                new Exception("Error: Goto statements aren't allowed between different procedures");

            currentToken.TokenType = symbol.getTokenType();

            GenerateMepa(l1.Trim(), "ENRT", symbol.ScopeLevel + "," + SymbolTable.GetVariableCount());

            match("TK_A_LABEL");
            match("TK_COLON");
            statements();
        }

        public static void caseStat()
        {
            Symbol exp;
            String l1 = null;
            match("TK_CASE");
            exp = SymbolTable.Lookup(currentToken.TokenValue);
            String valor;
            Expressao();
            match("TK_OF");
            l1 = Next_Label();
            do
            {
                if ("TK_END".Equals(currentToken.TokenType))
                {
                    match("TK_END");
                    break;
                }
                GenerateMepa(l1, "NADA", "");
                l1 = Next_Label();
                if ("TK_SEMI_COLON".Equals(currentToken.TokenType))
                {
                    match("TK_SEMI_COLON");
                }
                else
                {
                    valor = currentToken.TokenValue;
                    match("TK_INTLIT");

                    if ("TK_COLON".Equals(currentToken.TokenType))
                    {
                        match("TK_COLON");
                        GenerateMepa("", "CRVL", exp.ScopeLevel + "," + exp.getAddress());
                        GenerateMepa("", "CRCT", valor);
                        GenerateMepa("", "CMIG", "");
                        GenerateMepa("", "DSVF", l1);
                        statements();
                        if ("TK_END".Equals(currentToken.TokenType))
                        {
                            match("TK_END");
                            break;
                        }
                        else
                        {
                            match("TK_SEMI_COLON");
                        }
                    }else
                    match("TK_COMMA");
                }
            } while (true);

            GenerateMepa(l1, "NADA", "");

        }

        public static void goToStat()
        {
            Symbol symbol;

            match("TK_GOTO");
            symbol = SymbolTable.Lookup(currentToken.TokenValue);
            currentToken.TokenType = symbol.getTokenType();

            if (SymbolTable.CurrentScopeLevel != symbol.ScopeLevel)
                new Exception("Error: Goto statements aren't allowed between different procedures");

            Encoding u8 = Encoding.UTF8;
            byte[] bytes = BitConverter.GetBytes(symbol.getAddress());
            string l1 = u8.GetString(bytes);

            GenerateMepa("", "DSVR", l1 + "," + symbol.ScopeLevel + "," + SymbolTable.CurrentScopeLevel );

            match("TK_A_LABEL");
        }

        public static void repeatStat()
        {
            String l1 = null, l2 = null;

            l1 = Next_Label();
            l2 = Next_Label();

            match("TK_REPEAT");
            GenerateMepa(l1,"NADA","");

            statements();

            while ("TK_SEMI_COLON".Equals(currentToken.TokenType)) {
                match("TK_SEMI_COLON");
                statements();
            }
            match("TK_UNTIL");

            Expressao();
            GenerateMepa("","DSVF",l1);
            GenerateMepa(l2,"NADA","");
        }

        public static void whileStat()
        {
            String l1 = null, l2 = null;
            l1 = Next_Label();

            match("TK_WHILE");
            GenerateMepa(l1, "NADA", "");
            Expressao();
            match("TK_DO");

            l2 = Next_Label();
            GenerateMepa("", "DSVF", l2);

            statements();

            GenerateMepa("", "DSVS", l1);
            GenerateMepa(l2, "NADA", "");
        }

        public static void forStat()
        {
            String l1 = null, l2 = null;
            bool inc = false;
            Symbol vaiden;
            l1 = Next_Label();
            l2 = Next_Label();

            match("TK_FOR");
            vaiden = SymbolTable.Lookup(currentToken.TokenValue);

                match("TK_SEMI_COLON");

                l1= Next_Label();
                l2= Next_Label();
                GenerateMepa("","DSVS",l1);

                Encoding u8 = Encoding.UTF8;
                Symbol symbol = new Symbol(procedureName, "TK_A_PROC", TYPE.P, BitConverter.ToInt16(u8.GetBytes(l2), 0) );

                if (SymbolTable.Lookup(procedureName) == null)
                {
                    SymbolTable.Insert(symbol);
                }

                SymbolTable.OpenScope();
                GenerateMepa(l2, "ENPR", SymbolTable.CurrentScopeLevel.ToString() );

                block();

                GenerateMepa("", "RTPR", SymbolTable.CurrentScopeLevel.ToString() +","+ SymbolTable.GetVariableCount() );
                GenerateMepa(l1, "NADA","");
                SymbolTable.CloseScope();
                match("TK_SEMI_COLON");
            }
        }

        public static void palist() {
            bool loop= true;

            if (!"TK_OPEN_PARENTHESIS".Equals(currentToken.TokenType))
                return;

            match("TK_OPEN_PARENTHESIS");

            do
            {
                if ("TK_PROCEDURE".Equals(currentToken.TokenType))
                {
                    match("TK_PROCEDURE");
                    match("TK_IDENTIFIER");
                    while ("TK_COMMA".Equals(currentToken.TokenType))
                    {
                        match("TK_COMMA");
                        match("TK_IDENTIFIER");
                    }

                }

                if ("TK_VAR".Equals(currentToken.TokenType) || "TK_FUNCTION".Equals(currentToken.TokenType))
                {
                    string type = currentToken.TokenType;
                    match(type);

                    match("TK_IDENTIFIER");
                    while ("TK_COMMA".Equals(currentToken.TokenType))
                    {
                        match("TK_COMMA");
                        match("TK_IDENTIFIER");
                    }
                    match("TK_COLON");
                    match("TK_IDENTIFIER");
                }

                if ("TK_SEMI_COLON".Equals(currentToken.TokenType)){
                    match("TK_SEMI_COLON");
                }
                else{
                    loop = false;
                }

            } while (loop);

            match("TK_CLOSE_PARENTHESIS");

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

                getToken();
            }
        }

    }
}
