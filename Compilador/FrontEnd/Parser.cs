﻿using System;
using System.Collections.Generic;
using System.IO;
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
        private static StreamWriter file=null;
        private static Stream saida=null;

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

        private static void openFileOutput()
        {
            try
            {
                saida = File.Open("Mepa.txt", FileMode.Create);
                file = new StreamWriter(saida);
            }
            catch (Exception ex) { }

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
            if (Int32.Parse(SymbolTable.getQteVariaveis()) > 0)
                GenerateMepa("", "DMEM", SymbolTable.getQteVariaveis());

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
            Symbol symbol = SymbolTable.Busca(currentToken.TokenValue);
            match("TK_A_PROC");
            Encoding u8 = Encoding.UTF8;
            byte[] bytes = BitConverter.GetBytes(symbol.getAddress());
            string l1 = u8.GetString(bytes);
            GenerateMepa("", "CHPR", l1);

        }

        public static void labelStat() {

            Symbol symbol = SymbolTable.Busca( currentToken.TokenValue );

            Encoding u8 = Encoding.UTF8;
            byte[] bytes = BitConverter.GetBytes( symbol.getAddress() );
            string l1 = u8.GetString(bytes);


            if (SymbolTable.nivel_corrente != symbol.nivel_corrente)
                new Exception("Error: Goto statements aren't allowed between different procedures");

            currentToken.TokenType = symbol.getTokenType();

            GenerateMepa(l1.Trim(), "ENRT", symbol.nivel_corrente + "," + SymbolTable.getQteVariaveis()); //Posição ao qual o GOTO TEM QUE SE REFERIR através de L1
                                                                                                   //Precisa guardar esse 11 e esse  nivel atual junto com o numero da label na tabela   
            match("TK_A_LABEL");
            match("TK_COLON");
            statements();
        }

        public static void caseStat()
        {
            Symbol exp;
            String l1 = null;
            match("TK_CASE");
            exp = SymbolTable.Busca(currentToken.TokenValue);
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
                        GenerateMepa("", "CRVL", exp.nivel_corrente + "," + exp.getAddress());
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
            symbol = SymbolTable.Busca(currentToken.TokenValue);
            currentToken.TokenType = symbol.getTokenType();

            if (SymbolTable.nivel_corrente != symbol.nivel_corrente)
                new Exception("Error: Goto statements aren't allowed between different procedures");

            Encoding u8 = Encoding.UTF8;
            byte[] bytes = BitConverter.GetBytes(symbol.getAddress());
            string l1 = u8.GetString(bytes);

            //precisa por o rotulo do simbolo, o nivel do simbolo e o nivel atual como parametros
            GenerateMepa("", "DSVR", l1 + "," + symbol.nivel_corrente + "," + SymbolTable.nivel_corrente );

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
            vaiden = SymbolTable.Busca(currentToken.TokenValue);/*guardando o simbolo da variavel k*/
            statements(); //ex k:=1
            GenerateMepa(l2, "NADA", "");
            GenerateMepa("", "CRVL", vaiden.nivel_corrente + "," + vaiden.getAddress());

            if ("TK_TO".Equals(currentToken.TokenType))
            {
                match("TK_TO");
                inc = true;
            }
            else
                if ("TK_DOWNTO".Equals(currentToken.TokenType))
                match("TK_DOWNTO");

            Expressao();//ex" 3

            GenerateMepa("", "CMEG", "");
            GenerateMepa("", "DSVF", l1);

            //Faz o que é pedido dentro do begin-end
            match("TK_DO");
            SymbolTable.openScope();
            
            statements();

            // K++ / K--
            GenerateMepa("", "CRVL", vaiden.nivel_corrente + "," + vaiden.getAddress());
            GenerateMepa("", "CRCT", "1");

            if (inc)
                GenerateMepa("", "SOMA", "");
            else
                GenerateMepa("", "SUBT", "");

            GenerateMepa("", "ARMZ", vaiden.nivel_corrente + "," + vaiden.getAddress());
            GenerateMepa("", "DSVS", l2);//volta a ver a condição


            SymbolTable.closeScope();
            GenerateMepa(l1, "NADA", ""); //Saiu do for

        } 

        public static void infipo() {

            while ("TK_OPEN_SQUARE_BRACKET".Equals(currentToken.TokenType) || "TK_DOT".Equals(currentToken.TokenType)) {
                switch (currentToken.TokenType) {
                    case "TK_OPEN_SQUARE_BRACKET":
                        match("TK_OPEN_SQUARE_BRACKET");
                        Expressao();

                        while ("TK_COMMA".Equals(currentToken.TokenType))
                        {
                            match("TK_COMMA");
                            Expressao();
                        }

                        match("TK_CLOSE_SQUARE_BRACKET");
                     break;

                    case "TK_DOT":
                        match("FIIDEN");
                    break;
                }
            }

        }

        public static void assignmentStat()
        {
            Symbol symbol = SymbolTable.Busca(currentToken.TokenValue);
            infipo();

            match("TK_VAIDEN");
            match("TK_ASSIGNMENT");

            Expressao();

            if (symbol != null)
            {
                GenerateMepa( "","ARMZ",symbol.nivel_corrente+","+ symbol.getAddress().ToString() );
            }

        }

        public static void writeStat()  
        {
            if("TK_WRITELN".Equals(currentToken.TokenType))
                match("TK_WRITELN");

            if ("TK_WRITE".Equals(currentToken.TokenType))
                match("TK_WRITE");

            match("TK_OPEN_PARENTHESIS");

            Expressao();

            while ("TK_COMMA".Equals(currentToken.TokenType)) {
                match("TK_COMMA");
                GenerateMepa("", "IMPR", "");
                Expressao();
            }
            //A expressão deixará no topo da pilha o que será escrito
            GenerateMepa("","IMPR","");
            match("TK_CLOSE_PARENTHESIS");

        }

        
        public static void readStat(){
            Symbol symbol;

            if ("TK_READLN".Equals(currentToken.TokenType))
                match("TK_READLN");

            if ("TK_READ".Equals(currentToken.TokenType))
                match("TK_READ");

            match("TK_OPEN_PARENTHESIS");

            symbol = SymbolTable.Busca(currentToken.TokenValue);
            match(symbol.getTokenType());
            GenerateMepa("", "LEIT", "");
            GenerateMepa("", "ARMZ", symbol.nivel_corrente + "," + symbol.getAddress().ToString());

            while ("TK_COMMA".Equals(currentToken.TokenType))
            {
                match("TK_COMMA");
                symbol = SymbolTable.Busca(currentToken.TokenValue);
                match(symbol.getTokenType());
                GenerateMepa("", "LEIT", "");
                GenerateMepa("", "ARMZ", symbol.nivel_corrente + "," + symbol.getAddress().ToString());
            }
            match("TK_CLOSE_PARENTHESIS");

        }


        public static void ifStat()
        {
            string l1 = null , l2=null;
            l1 = Next_Label();

            match("TK_IF");
            Expressao();
            match("TK_THEN");

            GenerateMepa("","DSVF",l1);

            statements();

            if (currentToken.TokenType.Equals("TK_ELSE"))
            {
                l2 = Next_Label();
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
                //Depois que os valores das expressões da esquerda e direita estiverem na pilha
                //devemos indicar a operação que se deseja
                switch(pred)
                {
                    case "TK_LESS_THAN":
                        GenerateMepa("","CMME","");
                    break;
                    case "TK_GREATER_THAN":
                        GenerateMepa("","CMMA","");
                    break;
                    case "TK_LESS_THAN_EQUAL":
                        GenerateMepa("","CMEG","");
                    break;
                    case "TK_GREATER_THAN_EQUAL":
                        GenerateMepa("","CMAG","");
                    break;
                    case "TK_EQUAL":
                        GenerateMepa("","CMIG","");
                    break;
                    case "TK_NOT_EQUAL":
                        GenerateMepa("","CMDG","");
                    break;
                }
              
            }

        }

        public static void ExpressaoSimples()
        {
            Termo();
            while (currentToken.TokenType.Equals("TK_PLUS") || currentToken.TokenType.Equals("TK_MINUS") || currentToken.TokenType.Equals("TK_OR")) {
                String op = currentToken.TokenType;
                match(op);
                Termo();
                switch(op)
                {
                    case "TK_PLUS":
                        GenerateMepa("","SOMA","");
                        break;
                    case "TK_MINUS":
                        GenerateMepa("","SUBT","");
                        break;
                    case "TK_OR":
                        GenerateMepa("","DISJ","");
                        break;
                }
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
                    case "TK_VAIDEN":
                        Symbol symbol = SymbolTable.Busca(currentToken.TokenValue);
                        infipo();
                        GenerateMepa("", "CRVL", symbol.nivel_corrente.ToString() + "," + symbol.getAddress().ToString());
                        match("TK_VAIDEN");

                        break;

                    case "TK_INTLIT":
                        GenerateMepa("", "CRCT", currentToken.TokenValue);
                        match("TK_INTLIT");
                        break;

                    case "TK_BOOLLIT":
                        if (Convert.ToBoolean(currentToken.TokenValue))
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
                        GenerateMepa("", "NEGA", "");
                        break;

                    default:
                        break;
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

                while ("TK_INTLIT".Equals(currentToken.TokenType))
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
                    String l1 = null;
                    l1 = Next_Label();

                    Encoding u8 = Encoding.UTF8;
                    Symbol symbol = new Symbol(label.TokenValue,
                            "TK_A_LABEL",
                            TYPE.L,
                            BitConverter.ToInt16(u8.GetBytes(l1),0) );

                    if (SymbolTable.Busca(label.TokenValue) == null)
                    {
                        SymbolTable.Insere(symbol);
                    }
                }

                match("TK_SEMI_COLON");
            }
        }

        public static void VarDeclarations()
        {
       
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
                    currentToken.TokenType = "TK_VAIDEN";
                    variablesArrayList.Add(currentToken);

                    match("TK_VAIDEN");

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
                            "TK_VAIDEN",
                            STRING_TYPE_HASH_MAP[ dataType.ToLower().Substring(3) ], 
                            dp);

                    dp++;


                    if (SymbolTable.Busca( var.TokenValue ) == null)
                    {
                        SymbolTable.Insere(symbol);
                    }
                }

                SymbolTable.setQteVariaveis( variablesArrayList.Count.ToString() ) ;
                GenerateMepa( "","AMEM", variablesArrayList.Count.ToString() );

                match("TK_SEMI_COLON");
                
            }
          
            
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
                palist();/*parametros formais*/
                match("TK_SEMI_COLON");

                // generate hole to jump past the body
                l1= Next_Label();
                l2= Next_Label();
                GenerateMepa("","DSVS",l1);

                Encoding u8 = Encoding.UTF8;
                Symbol symbol = new Symbol(procedureName, "TK_A_PROC", TYPE.P, BitConverter.ToInt16(u8.GetBytes(l2), 0) );

                if (SymbolTable.Busca(procedureName) == null)
                {
                    SymbolTable.Insere(symbol);
                }

                SymbolTable.openScope();
                GenerateMepa(l2, "ENPR", SymbolTable.nivel_corrente.ToString() );

                // body
                block();

                // hole to return the procedure
                GenerateMepa("", "RTPR", SymbolTable.nivel_corrente.ToString() +","+ SymbolTable.getQteVariaveis() );
                GenerateMepa(l1, "NADA","");
                SymbolTable.closeScope();
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
                    symb = SymbolTable.Busca(currentToken.TokenValue);

                    if (symb != null)
                        currentToken.TokenType = symb.getTokenType();
                }
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
