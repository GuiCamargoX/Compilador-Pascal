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
        
                    if (SymbolTable.Lookup(label.TokenValue) == null)
                    {
                        SymbolTable.Insert(symbol);
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
        
        
                    if (SymbolTable.Lookup( var.TokenValue ) == null)
                    {
                        SymbolTable.Insert(symbol);
                    }
                }
        
                SymbolTable.SetVariableCount( variablesArrayList.Count.ToString() ) ;
                GenerateMepa( "","AMEM", variablesArrayList.Count.ToString() );
        
                match("TK_SEMI_COLON");
                
            }
          
            
        }

        private static void procDeclaration()
        {
            String l1=null, l2=null;
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
        
                if (SymbolTable.Lookup(procedureName) == null)
                {
                    SymbolTable.Insert(symbol);
                }
        
                SymbolTable.OpenScope();
                GenerateMepa(l2, "ENPR", SymbolTable.CurrentScopeLevel.ToString() );
        
                // body
                block();
        
                // hole to return the procedure
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
    }
}
