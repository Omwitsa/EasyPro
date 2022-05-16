using System;

namespace EasyPro.Utils
{
    public class Decryptor
    {
        public static string Decript_String(string str)
        {
            int strLen, j, ChVal;
            string Ch, EncptStr;

            strLen = str.Length;
            EncptStr = "";
            for (j = 0; j < strLen; ++j)
            {
                Ch = str.Substring(j, 1);
                char CH = Convert.ToChar(Ch);// problem
                ChVal = (int)CH;

                Char myChar = Convert.ToChar(Decript_Char_Value(ChVal));// was commented
                EncptStr = EncptStr + Convert.ToChar(Decript_Char_Value(ChVal));
            }
            return EncptStr;
        }
        public static int Decript_Char_Value(int Encpt)
        {
            int MaxCharval, c;
            c = 32;
            MaxCharval = 128;
            return MaxCharval - Encpt + c;
        }
    }
}
