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
                        Symbol symbol = SymbolTable.Lookup(currentToken.TokenValue);
                        infipo();
                        GenerateMepa("", "CRVL", symbol.ScopeLevel.ToString() + "," + symbol.getAddress().ToString());
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
    }
}
