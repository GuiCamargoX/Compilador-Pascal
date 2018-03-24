using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador.FrontEnd
{
    static class Word //word= sequencia de caracters do alafabeto , classe auxilio para juntar os caracters em uma string 
    {
        public static string TokenName { get; set; } = "";
        public static int LineRow { get; set; } = 0;
        public static int LineCol { get; set; } = 0;
        public static bool ReadingString { get; set; } = false;
        public static bool ReadingNumber { get; set; } = false;
        public static bool IsFloat { get; set; } = false;
        public static bool SciNotation { get; set; } = false;
        public static bool ReadingColon { get; set; } = false;
        public static bool ReadingBool { get; set; } = false;
        public static bool ReadingDot { get; set; } = false;

        public static void clearStates()
        {
            ReadingString = false;
            ReadingNumber = false;
            IsFloat = false;
            SciNotation = false;
            ReadingColon = false;
            ReadingBool = false;
            ReadingDot = false;
            TokenName = "";
        }

        public static void add(char ch) {
            TokenName += ch;
        }

        public static bool isEmpty() {
            return TokenName == "";
        }

    }
}
