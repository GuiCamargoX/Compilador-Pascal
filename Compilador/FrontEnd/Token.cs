using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador.FrontEnd
{
    class Token
    {
        public string TokenType
        { get; private set; }


        public string TokenValue { get; private set; } = ""; //==lexema
        public int LineCol { get; private set; } = 0;
        public int LineRow { get; private set; } = 0;

        public Token(string tokenType, string tokenValue, int lineCol, int lineRow) {
            this.TokenType = tokenType;
            this.TokenValue = tokenValue;

            this.LineCol = lineCol;
            this.LineRow = lineRow;
        }


        public override String ToString()
        {
            return TokenValue;
        }

    }
}
