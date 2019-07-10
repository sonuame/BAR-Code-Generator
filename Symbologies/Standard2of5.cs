using System;

namespace BarCodeGenerator.Symbologies
{
    class Standard2of5 : BarcodeCommon, IBarcode
    {
        private readonly string[] S25_Code = { "11101010101110", "10111010101110", "11101110101010", "10101110101110", "11101011101010", "10111011101010", "10101011101110", "10101110111010", "11101010111010", "10111010111010" };
        private readonly TYPE Encoded_Type = TYPE.UNSPECIFIED;

        public Standard2of5(string input, TYPE EncodedType)
        {
            Raw_Data = input;
            Encoded_Type = EncodedType;
        }

        private string Encode_Standard2of5()
        {
            if (!CheckNumericOnly(Raw_Data))
                Error("ES25-1: Numeric Data Only");

            string result = "11011010";

            foreach (char c in Raw_Data)
            {
                result += S25_Code[Int32.Parse(c.ToString())];
            }

            result += Encoded_Type == TYPE.Standard2of5_Mod10 ? S25_Code[CalculateMod10CheckDigit()] : "";

            result += "1101011";
            return result;
        }

        private int CalculateMod10CheckDigit()
        {
            int sum = 0;
            bool even = true;
            for (int i = Raw_Data.Length - 1; i >= 0; --i)
            {
                sum += Raw_Data[i] * (even ? 3 : 1);
                even = !even;
            }

            return sum % 10;
        }

        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_Standard2of5(); }
        }

        #endregion
    }
}
