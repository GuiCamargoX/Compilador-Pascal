using System;
using System.Collections.Generic;
using System.IO;
using Compilador.Tools;

namespace Compilador.FrontEnd {
    class Parser {
        
        public String nT() {
            //nextToken
            //Leria o proximo item da tokenArrayList
        }

        public bool parseCode() {
            //Funcao inicial que chama o primeiro n� do grafo
            return f00();
        }

        public void voltaUm(){
            //em alguns casos é necessário voltar na leitura dos tokens
        }







        //=================================================================================
        //                              FUNCOES 
        //=================================================================================

        //=================================
        //      const
        //=================================
        private bool const(){
            aux = nT();
            if(aux.TokenType == "string"){
                return true;
            }
            if(aux.TokenValue == "+" || aux.TokenValue == "-"){
                aux = nT();
                if(aux.TokenType == "coiden" || aux.TokenType == "numb"){
                    return true;
                }else return false;
            }else{
                return false;
            }
        }
        //===============================
        //      sitype
        //=================================
        private bool sitype(){
            aux = nT();
            if(aux.TokenType == "tyiden"){
                return true;
            }
            if(aux.TokenValue == "("){
                do{
                    if(nT().TokenType !="identifier") return false;
                    aux = nT();
                }while(aux.TokenValue == ",");
                if(aux.TokenValue!=")") return false;
                return true;
            }
            if(const()){
                if(nT().TokenValue!="..") return false;
                if(!const()) return false;
                return true;
            }
        }

        //=================================
        //      type
        //=================================
        private bool type(){
            aux = nT();
            if(aux.TokenValue == "arrowUp"){
                if(nT().TokenType != "tyiden") return false;
                return true;
            }
            while(aux.TokenValue == "packed"){
                aux = nT();
            }
            if(aux.TokenValue == "array"){
                if(nT().TokenValue!="[") return false;
                do {
                    if(!sitype()) return false;
                    aux = nT();
                }while(aux.TokenValue == ",");
                if(aux.TokenValue !="]") return false;
                if(nT().TokenValue != "of") return false;
                if(!type()) return false;
                return true;
            }
            if(aux.TokenValue == "file"){
                if(nT().TokenValue != "of") return false;
                if(!type()) return false;
                return true;
            }
            if(aux.TokenValue == "set"){
                if(nT().TokenValue != "of") return false;
                if(!sitype()) return false;
            }
            if(aux.TokenValue == "record"){
                if(!filist()) return false;
                if(nT().TokenValue != "end") return false;
            }
            voltaUm();
            if(!sitype()) return false;
            return true;
            
        }

        //=================================
        //      filist
        //=================================
        private bool filist(){
            
        }

        //=================================
        //      infipo
        //=================================
        private bool infipo(){
            do {
                aux = nT();
                if(aux.TokenValue == "["){
                    do{
                        if(!expr()) return false;
                        aux = nT();
                    }while(aux.TokenValue == ",");
                    if(aux != "]") return false;
                }else
                if(aux.TokenValue == "."){
                    if(nT().TokenType != "fiiden") return false;
                }
            }while(aux.TokenValue == "[" || aux.TokenValue == "." || aux.TokenValue == "arrowUp");
            return true;
        }

        //=================================
        //      factor
        //=================================
        private bool factor(){
            aux = nT();
            if(aux.TokenType == "coiden"){
                return true;
            }
            if(aux.TokenType == "numb"){
                return true;
            }
            if(aux.TokenValue == "nil"){
                return true;
            }
            if(aux.TokenType == "string"){
                return true
            }
            if(aux.TokenType == "vaiden"){
                if(!infipo()) return false;
                return true;
            }
            if(aux.TokenType == "fuiden"){
                aux = nT();
                if(aux.TokenValue == "lambda") return true;
                if(aux.TokenValue != "(") return false;
                do {
                    if(!expr()) return false;
                    aux = nT();
                }while(aux.TokenValue == ",");
                if(aux.TokenValue != ")") return false;
                return true;
            }
            if(aux.TokenValue == "{"){
                if(!expr()) reutnr false;
                if(nT().TokenValue != "}") return false;
                return true;
            }
            if(aux.TokenValue == "not"){
                if(!factor()) return false;
                return true;
            }
            if(aux.TokenValue == "["){
                if(nT().TokenValue == "]"){
                    return true;
                }
                voltaUm();
                do{
                    if(!expr())return false;
                    aux = nT();
                    if(aux.TokenValue == ".."){
                        if(!expr()) return false;
                        aux = nT();
                    }
                }while(aux.TokenValue == ",");
                if(aux.TokenValue != "]")return false;
                return true;
            }
            return false;
        }
    
        //=================================
        //      term
        //=================================
        private bool term(){
            do{
                if(!factor()) return false;
                aux = nT();
            }while(aux.TokenValue == "*"
            || aux.TokenValue == "/"
            || aux.TokenValue == "div"
            || aux.TokenValue == "mod"
            || aux.TokenValue == "and"
            );
            voltaUm();
            return true;
            
        }

        //=================================
        //      siexpr
        //=================================
        private bool siexpr(){
            aux = nT();
            if(aux.TokenValue == "+" || aux.TokenValue == "-"){
                do{
                    if(!term()) return false;
                    aux = nT();
                }while(aux.TokenValue == "+"
                || aux.TokenValue == "-"
                || aux.TokenValue == "or"
                );
                voltaUm();
                return true;
            }else{
                return false;
            }
        }

        //=================================
        //      expr
        //=================================
        private bool expr(){
            if(!siexpr()) return false;
            aux = nT();

            if(aux.TokenValue == "="
            || aux.TokenValue == "<"
            || aux.TokenValue == ">"
            || aux.TokenValue == "<>"
            || aux.TokenValue == ">="
            || aux.TokenValue == "<="
            || aux.TokenValue == "in"
            ){
                if(!siexpr()) return false;
            }else{
                voltaUm();
            }

            return true;
        }

        //=================================
        //      palist
        //=================================
        private bool palist(){
            if(nT().TokenValue == "("){
                do {
                    aux = nT();
                    if(aux.TokenValue == "proc"){
                        do{
                            if(nT().TokenType !="identifier") return false;
                            aux1 = nT();
                        }while (aux1.TokenValue == ",");
                    }else{
                        if(aux.TokenValue == "identifier") voltaUm();
                        do {
                            if(nT().TokenType != "identifier") return false;
                            aux1 = nT();
                        }while(aux1.TokenValue == ",");
                        if(aux1.TokenValue != ":") return false;
                        if(nT().TokenType != "tyiden");
                    }
                    aux = nT();
                }while(aux.TokenValue == ";");
                if(aux.TokenValue != ")") return false;
            }
            return true;
        }

        //=================================
        //      block
        //=================================
        private bool block(){

        }

        //=================================
        //      statm
        //=================================
        private bool statm(){
            
        }

        //=================================
        //      prog
        //=================================
        private bool prog(){   
            if(nT().TokenValue != "program") return false;
            if(nT().TokenType !="identifier") return false;
            if(nT().TokenValue !="(") return false;
            if(nT().TokenType !="identifier")return false;

            aux = nT();
            while(aux.TokenValue = ",")){
                if(nT().TokenType != "identifier") return false;
                aux = nT();
            }

            if(aux.TokenValue != ")") return false;
            if(nT().TokenValue !=";") return false;
            if(!block()) return false;
            if(nT().TokenValue != ".") return false;

            return true;
        }

    //======================================================================================================
    //                                          FIM FUNCOES
    //======================================================================================================


    }
}
    