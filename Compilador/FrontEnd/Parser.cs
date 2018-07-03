﻿using System;
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

        private static Dictionary<String, TYPE> STRING_TYPE_HASH_MAP;

        private static int dp = 0; // data pointer for vars
        private static Token currentToken;
        private static IEnumerator<Token> it;

        private static readonly int INSTRUCTION_SIZE = 1000;

        private static Byte[] byteArray = new Byte[INSTRUCTION_SIZE];
        private static int s = -1;
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

            return byteArray;
        }


        public static string block()
        {
            string n = null;

            if( "TK_LABEL".Equals(currentToken.TokenType) )
                /*n = labelDeclarations();*/
                
            if("TK_VAR".Equals( currentToken.TokenType ) )
                n = VarDeclarations();
                   

            while ("TK_PROCEDURE".Equals(currentToken.TokenType) || "TK_FUNCTION".Equals(currentToken.TokenType)) {
                switch (currentToken.TokenType) {

                    case "TK_PROCEDURE":
                        procDeclaration();
                        break;

                    case "TK_FUNCTION":
                        /*funcDeclaration();*/
                        break;
                }
            }

            if("TK_BEGIN".Equals( currentToken.TokenType ) )
                comando_composto(n);

            return n;
        }

        public static void comando_composto(string n)
        {
            match("TK_BEGIN");
            statements();
            match("TK_END");
            GenerateMepa("", "DMEM", n);
        }

        public static void statements()
        {
            while (!currentToken.TokenType.Equals("TK_END"))
            {
                switch (currentToken.TokenType)
                {
                    case "TK_GOTO":
                        goToStat();
                        break;
                    case "TK_WHILE":
                        whileStat();
                        break;
                    case "TK_IF":
                        ifStat();
                        break;
                    case "TK_WRITELN":
                        writeStat();
                        break;
                    case "TK_IDENTIFIER":
                        Symbol symbol = SymbolTable.Busca(currentToken.TokenValue);
                        if (symbol != null)
                        {
                            // assign token type to be var, proc, or label
                            currentToken.TokenType = symbol.getTokenType();
                        }
                        break;
                    case "TK_A_VAR":
                        assignmentStat();
                        break;
                    case "TK_A_PROC":
                        procedureStat();
                        break;
                    case "TK_SEMI_COLON":
                        match("TK_SEMI_COLON");
                        break;
                    default:
                        return;
                }
            }

        }

        public static void ifStat()
        {
            string l1 = null , l2=null;
            Next_Label(l1);

            match("TK_IF");
            Expressao();
            match("TK_THEN");

            GenerateMepa("","DSVF",l1);

            statements();

            if (currentToken.TokenType.Equals("TK_ELSE"))
            {
                Next_Label(l2);
                GenerateMepa("", "DSVS", l2);
                GenerateMepa(l1, "NADA", "");
                match("TK_ELSE");
                statements();
                GenerateMepa(l2, "NADA", "");
            }
            else {
                GenerateMepa(l1, "NADA", "");
            }

        }

        public static void Expressao()
        {
            ExpressaoSimples();
            if (currentToken.TokenType.Equals("TK_LESS_THAN") ||
                    currentToken.TokenType.Equals("TK_GREATER_THAN") ||
                    currentToken.TokenType.Equals("TK_LESS_THAN_EQUAL") ||
                    currentToken.TokenType.Equals("TK_GREATER_THAN_EQUAL") ||
                    currentToken.TokenType.Equals("TK_EQUAL") ||
                    currentToken.TokenType.Equals("TK_NOT_EQUAL"))
            {
                String pred = currentToken.TokenType;
                match(pred);
                ExpressaoSimples();

              
            }

        }

        public static void ExpressaoSimples()
        {
            Termo();
            while (currentToken.TokenType.Equals("TK_PLUS") || currentToken.TokenType.Equals("TK_MINUS") || currentToken.TokenType.Equals("TK_OR")) {
                String op = currentToken.TokenType;
                match(op);
                Termo();
            }

        }

        public static void Termo()
        {
            Fator();
            while (currentToken.TokenType.Equals("TK_MULTIPLY") ||
                    currentToken.TokenType.Equals("TK_DIVIDE") ||
                    currentToken.TokenType.Equals("TK_AND"))
            {
                String op = currentToken.TokenType;
                match(op);
                Fator();
                switch (op)
                {
                    case "TK_MULTIPLY":
                        GenerateMepa("", "MULT", "");
                        break;

                    case "TK_DIVIDE":
                        GenerateMepa("", "DIV", "");
                        break;

                    case "TK_AND":
                        GenerateMepa("", "CONJ", "");
                        break;

                }

                
            }
           
        }

        public static void Fator()
        {
            switch (currentToken.TokenType)
            {
                case "TK_IDENTIFIER":
                    Symbol symbol = SymbolTable.Busca(currentToken.TokenValue );
                    if (symbol != null)
                    {
                        if (symbol.getTokenType().Equals("TK_A_VAR"))
                        {
                            // variable
                            currentToken.TokenType = "TK_A_VAR";

                            GenerateMepa("","CRVL", symbol.nivel_corrente.ToString()+","+ symbol.getAddress().ToString() );

                            match("TK_A_VAR");
                        }

                    }
                    else
                    {
                        throw new Exception(String.Format("Symbol not found (%s)", currentToken.TokenValue));
                    }
                    break;

                case "TK_INTLIT":
                    GenerateMepa("","CRCT",currentToken.TokenValue);
                    match("TK_INTLIT");
                    break;

                case "TK_BOOLLIT":
                    if( Convert.ToBoolean(currentToken.TokenValue) )
                        GenerateMepa("", "CRCT", "1");
                    else
                        GenerateMepa("", "CRCT", "0");

                    match("TK_BOOLLIT");
                    break;

                case "TK_OPEN_PARENTHESIS":
                    match("TK_OPEN_PARENTHESIS");
                    Expressao();
                    match("TK_CLOSE_PARENTHESIS");
                    break;

                case "TK_NOT":
                    match("TK_NOT");
                    Fator();
                    GenerateMepa("","NEGA","");
                    break;

                default:
                    throw new Exception("Unknown data type");
            }

        }


        private static void labelDeclarations()
        {
            while (true)
            {
                if ("TK_LABEL".Equals(currentToken.TokenType) )
                {
                    match("TK_LABEL");
                }
                else
                {
                    // currentToken is not "TK_LABEL"
                    break;
                }

                // Store labels in a list
                List<Token> labelsArrayList = new List<Token>();

                while ("TK_IDENTIFIER".Equals(currentToken.TokenType))
                {
                    currentToken.TokenType = "TK_A_LABEL";
                    labelsArrayList.Add(currentToken);

                    match("TK_A_LABEL");

                    if ("TK_COMMA".Equals(currentToken.TokenType))
                    {
                        match("TK_COMMA");
                    }
                }

                // insert all labels into SymbolTable
                foreach (Token label in labelsArrayList)
                {


                    Symbol symbol = new Symbol(label.TokenValue,
                            "TK_A_LABEL",
                            TYPE.L,
                            0);

                    if (SymbolTable.Busca(label.TokenValue) == null)
                    {
                        SymbolTable.Insere(symbol);
                    }
                }

                match("TK_SEMI_COLON");
            }
        }

        public static string VarDeclarations()
        {
            string n = null;

            while (true)
            {
                if ("TK_VAR".Equals(currentToken.TokenType))
                {
                    match("TK_VAR");
                }
                else
                {
                    // currentToken is not "TK_VAR"
                    break;
                }

                // Store variables in a list
                List<Token> variablesArrayList = new List<Token>();

                while ("TK_IDENTIFIER".Equals(currentToken.TokenType))
                {
                    currentToken.TokenType = "TK_A_VAR";
                    variablesArrayList.Add(currentToken);

                    match("TK_A_VAR");

                    if ("TK_COMMA".Equals(currentToken.TokenType))
                    {
                        match("TK_COMMA");
                    }
                }

                match("TK_COLON");
                String dataType = currentToken.TokenType;
                match(dataType);

                // Add the correct datatype for each identifier and insert into symbol table
                foreach (Token var in variablesArrayList)
                {

                    Symbol symbol = new Symbol(var.TokenValue,
                            "TK_A_VAR",
                            STRING_TYPE_HASH_MAP[ dataType.ToLower().Substring(3) ], 
                            dp);

                    dp++;


                    if (SymbolTable.Busca( var.TokenValue ) == null)
                    {
                        SymbolTable.Insere(symbol);
                    }
                }

                n = variablesArrayList.Count.ToString();
                GenerateMepa( "","AMEM", variablesArrayList.Count.ToString() );

                match("TK_SEMI_COLON");
                
            }
            return n;
            
        }

        private static void procDeclaration()
        {
            String l1=null, l2=null , n;
            // declaration
            if (currentToken.TokenType.Equals("TK_PROCEDURE"))
            {
                match("TK_PROCEDURE");
                currentToken.TokenType = "TK_A_PROC";

                String procedureName = currentToken.TokenValue;

                match("TK_A_PROC");
                match("TK_SEMI_COLON");

                // generate hole to jump past the body
                Next_Label(l1);
                Next_Label(l2);
                GenerateMepa("","DSVS",l1);

                Encoding u8 = Encoding.UTF8;
                Symbol symbol = new Symbol(procedureName, "TK_A_PROC", TYPE.P, BitConverter.ToInt32(u8.GetBytes(l2), 0) );

                SymbolTable.openScope();
                GenerateMepa(l2, "ENPR", SymbolTable.nivel_corrente.ToString() );

                if (SymbolTable.Busca(procedureName) == null)
                {
                    SymbolTable.Insere(symbol);
                }

                // body
                n = block();

                // hole to return the procedure
                GenerateMepa("", "RTPR", SymbolTable.nivel_corrente.ToString() +","+ n );
                GenerateMepa(l1, "NADA","");
                SymbolTable.closeScope();
            }
        }


        public static void GenerateMepa(string label , string code, string par1) {

        }

        public static void Next_Label(string l) {
            l = "L" + qte_label;
            qte_label++;
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
