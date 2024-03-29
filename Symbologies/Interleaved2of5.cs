using System;

namespace BarCodeGenerator.Symbologies
{
    class Interleaved2of5 : BarcodeCommon, IBarcode
    {
        private readonly string[] I25_Code = { "NNWWN", "WNNNW", "NWNNW", "WWNNN", "NNWNW", "WNWNN", "NWWNN", "NNNWW", "WNNWN", "NWNWN" };
        private readonly TYPE Encoded_Type = TYPE.UNSPECIFIED;

        public Interleaved2of5(string input, TYPE EncodedType)
        {
            Encoded_Type = EncodedType;
            Raw_Data = input;
        }
        private string Encode_Interleaved2of5()
        {
            if (Raw_Data.Length % 2 != (Encoded_Type == TYPE.Interleaved2of5_Mod10 ? 1 : 0))
                Error("EI25-1: Data length invalid.");

            if (!CheckNumericOnly(Raw_Data))
                Error("EI25-2: Numeric Data Only");
            
            string result = "1010";
            string data = Raw_Data + (Encoded_Type == TYPE.Interleaved2of5_Mod10 ? CalculateMod10CheckDigit().ToString() : "");

            for (int i = 0; i < data.Length; i += 2)
            {
                bool bars = true;
                string patternbars = I25_Code[Int32.Parse(data[i].ToString())];
                string patternspaces = I25_Code[Int32.Parse(data[i + 1].ToString())];
                string patternmixed = "";

                while (patternbars.Length > 0)
                {
                    patternmixed += patternbars[0].ToString() + patternspaces[0].ToString();
                    patternbars = patternbars.Substring(1);
                    patternspaces = patternspaces.Substring(1);
                }

                foreach (char c1 in patternmixed)
                {
                    if (bars)
                    {
                        if (c1 == 'N')
                            result += "1";
                        else
                            result += "11";
                    }
                    else
                    {
                        if (c1 == 'N')
                            result += "0";
                        else
                            result += "00";
                    }
                    bars = !bars;
                }
            }
            
            result += "1101";
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
            get { return this.Encode_Interleaved2of5(); }
        }

        #endregion
    }
}
