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
        
            GenerateMepa(l1.Trim(), "ENRT", symbol.ScopeLevel + "," + SymbolTable.GetVariableCount()); //Posição ao qual o GOTO TEM QUE SE REFERIR através de L1
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
        
            //precisa por o rotulo do simbolo, o nivel do simbolo e o nivel atual como parametros
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
            vaiden = SymbolTable.Lookup(currentToken.TokenValue);/*guardando o simbolo da variavel k*/
            statements(); //ex k:=1
            GenerateMepa(l2, "NADA", "");
            GenerateMepa("", "CRVL", vaiden.ScopeLevel + "," + vaiden.getAddress());
        
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
            SymbolTable.OpenScope();
            
            statements();
        
            // K++ / K--
            GenerateMepa("", "CRVL", vaiden.ScopeLevel + "," + vaiden.getAddress());
            GenerateMepa("", "CRCT", "1");
        
            if (inc)
                GenerateMepa("", "SOMA", "");
            else
                GenerateMepa("", "SUBT", "");
        
            GenerateMepa("", "ARMZ", vaiden.ScopeLevel + "," + vaiden.getAddress());
            GenerateMepa("", "DSVS", l2);//volta a ver a condição
        
        
            SymbolTable.CloseScope();
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
            Symbol symbol = SymbolTable.Lookup(currentToken.TokenValue);
            infipo();
        
            match("TK_VAIDEN");
            match("TK_ASSIGNMENT");
        
            Expressao();
        
            if (symbol != null)
            {
                GenerateMepa( "","ARMZ",symbol.ScopeLevel+","+ symbol.getAddress().ToString() );
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
        
            symbol = SymbolTable.Lookup(currentToken.TokenValue);
            match(symbol.getTokenType());
            GenerateMepa("", "LEIT", "");
            GenerateMepa("", "ARMZ", symbol.ScopeLevel + "," + symbol.getAddress().ToString());
        
            while ("TK_COMMA".Equals(currentToken.TokenType))
            {
                match("TK_COMMA");
                symbol = SymbolTable.Lookup(currentToken.TokenValue);
                match(symbol.getTokenType());
                GenerateMepa("", "LEIT", "");
                GenerateMepa("", "ARMZ", symbol.ScopeLevel + "," + symbol.getAddress().ToString());
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
    }
}
