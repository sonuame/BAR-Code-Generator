using System;
using System.Collections.Generic;
using System.Data;

namespace BarCodeGenerator.Symbologies
{
    class Code128 : BarcodeCommon, IBarcode
    {
        public static char FNC1 = Convert.ToChar(200);
        public static char FNC2 = Convert.ToChar(201);
        public static char FNC3 = Convert.ToChar(202);
        public static char FNC4 = Convert.ToChar(203);

        public enum TYPES:int { DYNAMIC, A, B, C };
        private DataTable C128_Code = new DataTable("C128");
        private List<string> _FormattedData = new List<string>();
        private List<string> _EncodedData = new List<string>();
        private DataRow StartCharacter = null;
        private TYPES type = TYPES.DYNAMIC;

        public Code128(string input)
        {
            Raw_Data = input;
        }

        public Code128(string input, TYPES type)
        {
            this.type = type;
            Raw_Data = input;
        }

        private string Encode_Code128()
        {
            this.init_Code128();

            return GetEncoding();            
        }
        private void init_Code128()
        {
            this.C128_Code.CaseSensitive = true;

            this.C128_Code.Columns.Add("Value", typeof(string));
            this.C128_Code.Columns.Add("A", typeof(string));
            this.C128_Code.Columns.Add("B", typeof(string));
            this.C128_Code.Columns.Add("C", typeof(string));
            this.C128_Code.Columns.Add("Encoding", typeof(string));

            this.C128_Code.Rows.Add(new object[] { "0", " ", " ", "00", "11011001100" });
            this.C128_Code.Rows.Add(new object[] { "1", "!", "!", "01", "11001101100" });
            this.C128_Code.Rows.Add(new object[] { "2", "\"", "\"", "02", "11001100110" });
            this.C128_Code.Rows.Add(new object[] { "3", "#", "#", "03", "10010011000" });
            this.C128_Code.Rows.Add(new object[] { "4", "$", "$", "04", "10010001100" });
            this.C128_Code.Rows.Add(new object[] { "5", "%", "%", "05", "10001001100" });
            this.C128_Code.Rows.Add(new object[] { "6", "&", "&", "06", "10011001000" });
            this.C128_Code.Rows.Add(new object[] { "7", "'", "'", "07", "10011000100" });
            this.C128_Code.Rows.Add(new object[] { "8", "(", "(", "08", "10001100100" });
            this.C128_Code.Rows.Add(new object[] { "9", ")", ")", "09", "11001001000" });
            this.C128_Code.Rows.Add(new object[] { "10", "*", "*", "10", "11001000100" });
            this.C128_Code.Rows.Add(new object[] { "11", "+", "+", "11", "11000100100" });
            this.C128_Code.Rows.Add(new object[] { "12", ",", ",", "12", "10110011100" });
            this.C128_Code.Rows.Add(new object[] { "13", "-", "-", "13", "10011011100" });
            this.C128_Code.Rows.Add(new object[] { "14", ".", ".", "14", "10011001110" });
            this.C128_Code.Rows.Add(new object[] { "15", "/", "/", "15", "10111001100" });
            this.C128_Code.Rows.Add(new object[] { "16", "0", "0", "16", "10011101100" });
            this.C128_Code.Rows.Add(new object[] { "17", "1", "1", "17", "10011100110" });
            this.C128_Code.Rows.Add(new object[] { "18", "2", "2", "18", "11001110010" });
            this.C128_Code.Rows.Add(new object[] { "19", "3", "3", "19", "11001011100" });
            this.C128_Code.Rows.Add(new object[] { "20", "4", "4", "20", "11001001110" });
            this.C128_Code.Rows.Add(new object[] { "21", "5", "5", "21", "11011100100" });
            this.C128_Code.Rows.Add(new object[] { "22", "6", "6", "22", "11001110100" });
            this.C128_Code.Rows.Add(new object[] { "23", "7", "7", "23", "11101101110" });
            this.C128_Code.Rows.Add(new object[] { "24", "8", "8", "24", "11101001100" });
            this.C128_Code.Rows.Add(new object[] { "25", "9", "9", "25", "11100101100" });
            this.C128_Code.Rows.Add(new object[] { "26", ":", ":", "26", "11100100110" });
            this.C128_Code.Rows.Add(new object[] { "27", ";", ";", "27", "11101100100" });
            this.C128_Code.Rows.Add(new object[] { "28", "<", "<", "28", "11100110100" });
            this.C128_Code.Rows.Add(new object[] { "29", "=", "=", "29", "11100110010" });
            this.C128_Code.Rows.Add(new object[] { "30", ">", ">", "30", "11011011000" });
            this.C128_Code.Rows.Add(new object[] { "31", "?", "?", "31", "11011000110" });
            this.C128_Code.Rows.Add(new object[] { "32", "@", "@", "32", "11000110110" });
            this.C128_Code.Rows.Add(new object[] { "33", "A", "A", "33", "10100011000" });
            this.C128_Code.Rows.Add(new object[] { "34", "B", "B", "34", "10001011000" });
            this.C128_Code.Rows.Add(new object[] { "35", "C", "C", "35", "10001000110" });
            this.C128_Code.Rows.Add(new object[] { "36", "D", "D", "36", "10110001000" });
            this.C128_Code.Rows.Add(new object[] { "37", "E", "E", "37", "10001101000" });
            this.C128_Code.Rows.Add(new object[] { "38", "F", "F", "38", "10001100010" });
            this.C128_Code.Rows.Add(new object[] { "39", "G", "G", "39", "11010001000" });
            this.C128_Code.Rows.Add(new object[] { "40", "H", "H", "40", "11000101000" });
            this.C128_Code.Rows.Add(new object[] { "41", "I", "I", "41", "11000100010" });
            this.C128_Code.Rows.Add(new object[] { "42", "J", "J", "42", "10110111000" });
            this.C128_Code.Rows.Add(new object[] { "43", "K", "K", "43", "10110001110" });
            this.C128_Code.Rows.Add(new object[] { "44", "L", "L", "44", "10001101110" });
            this.C128_Code.Rows.Add(new object[] { "45", "M", "M", "45", "10111011000" });
            this.C128_Code.Rows.Add(new object[] { "46", "N", "N", "46", "10111000110" });
            this.C128_Code.Rows.Add(new object[] { "47", "O", "O", "47", "10001110110" });
            this.C128_Code.Rows.Add(new object[] { "48", "P", "P", "48", "11101110110" });
            this.C128_Code.Rows.Add(new object[] { "49", "Q", "Q", "49", "11010001110" });
            this.C128_Code.Rows.Add(new object[] { "50", "R", "R", "50", "11000101110" });
            this.C128_Code.Rows.Add(new object[] { "51", "S", "S", "51", "11011101000" });
            this.C128_Code.Rows.Add(new object[] { "52", "T", "T", "52", "11011100010" });
            this.C128_Code.Rows.Add(new object[] { "53", "U", "U", "53", "11011101110" });
            this.C128_Code.Rows.Add(new object[] { "54", "V", "V", "54", "11101011000" });
            this.C128_Code.Rows.Add(new object[] { "55", "W", "W", "55", "11101000110" });
            this.C128_Code.Rows.Add(new object[] { "56", "X", "X", "56", "11100010110" });
            this.C128_Code.Rows.Add(new object[] { "57", "Y", "Y", "57", "11101101000" });
            this.C128_Code.Rows.Add(new object[] { "58", "Z", "Z", "58", "11101100010" });
            this.C128_Code.Rows.Add(new object[] { "59", "[", "[", "59", "11100011010" });
            this.C128_Code.Rows.Add(new object[] { "60",@"\",@"\", "60", "11101111010" });
            this.C128_Code.Rows.Add(new object[] { "61", "]", "]", "61", "11001000010" });
            this.C128_Code.Rows.Add(new object[] { "62", "^", "^", "62", "11110001010" });
            this.C128_Code.Rows.Add(new object[] { "63", "_", "_", "63", "10100110000" });
            this.C128_Code.Rows.Add(new object[] { "64", "\0", "`", "64", "10100001100" });
            this.C128_Code.Rows.Add(new object[] { "65", Convert.ToChar(1).ToString(), "a", "65", "10010110000" });
            this.C128_Code.Rows.Add(new object[] { "66", Convert.ToChar(2).ToString(), "b", "66", "10010000110" });
            this.C128_Code.Rows.Add(new object[] { "67", Convert.ToChar(3).ToString(), "c", "67", "10000101100" });
            this.C128_Code.Rows.Add(new object[] { "68", Convert.ToChar(4).ToString(), "d", "68", "10000100110" });
            this.C128_Code.Rows.Add(new object[] { "69", Convert.ToChar(5).ToString(), "e", "69", "10110010000" });
            this.C128_Code.Rows.Add(new object[] { "70", Convert.ToChar(6).ToString(), "f", "70", "10110000100" });
            this.C128_Code.Rows.Add(new object[] { "71", Convert.ToChar(7).ToString(), "g", "71", "10011010000" });
            this.C128_Code.Rows.Add(new object[] { "72", Convert.ToChar(8).ToString(), "h", "72", "10011000010" });
            this.C128_Code.Rows.Add(new object[] { "73", Convert.ToChar(9).ToString(), "i", "73", "10000110100" });
            this.C128_Code.Rows.Add(new object[] { "74", Convert.ToChar(10).ToString(), "j", "74", "10000110010" });
            this.C128_Code.Rows.Add(new object[] { "75", Convert.ToChar(11).ToString(), "k", "75", "11000010010" });
            this.C128_Code.Rows.Add(new object[] { "76", Convert.ToChar(12).ToString(), "l", "76", "11001010000" });
            this.C128_Code.Rows.Add(new object[] { "77", Convert.ToChar(13).ToString(), "m", "77", "11110111010" });
            this.C128_Code.Rows.Add(new object[] { "78", Convert.ToChar(14).ToString(), "n", "78", "11000010100" });
            this.C128_Code.Rows.Add(new object[] { "79", Convert.ToChar(15).ToString(), "o", "79", "10001111010" });
            this.C128_Code.Rows.Add(new object[] { "80", Convert.ToChar(16).ToString(), "p", "80", "10100111100" });
            this.C128_Code.Rows.Add(new object[] { "81", Convert.ToChar(17).ToString(), "q", "81", "10010111100" });
            this.C128_Code.Rows.Add(new object[] { "82", Convert.ToChar(18).ToString(), "r", "82", "10010011110" });
            this.C128_Code.Rows.Add(new object[] { "83", Convert.ToChar(19).ToString(), "s", "83", "10111100100" });
            this.C128_Code.Rows.Add(new object[] { "84", Convert.ToChar(20).ToString(), "t", "84", "10011110100" });
            this.C128_Code.Rows.Add(new object[] { "85", Convert.ToChar(21).ToString(), "u", "85", "10011110010" });
            this.C128_Code.Rows.Add(new object[] { "86", Convert.ToChar(22).ToString(), "v", "86", "11110100100" });
            this.C128_Code.Rows.Add(new object[] { "87", Convert.ToChar(23).ToString(), "w", "87", "11110010100" });
            this.C128_Code.Rows.Add(new object[] { "88", Convert.ToChar(24).ToString(), "x", "88", "11110010010" });
            this.C128_Code.Rows.Add(new object[] { "89", Convert.ToChar(25).ToString(), "y", "89", "11011011110" });
            this.C128_Code.Rows.Add(new object[] { "90", Convert.ToChar(26).ToString(), "z", "90", "11011110110" });
            this.C128_Code.Rows.Add(new object[] { "91", Convert.ToChar(27).ToString(), "{", "91", "11110110110" });
            this.C128_Code.Rows.Add(new object[] { "92", Convert.ToChar(28).ToString(), "|", "92", "10101111000" });
            this.C128_Code.Rows.Add(new object[] { "93", Convert.ToChar(29).ToString(), "}", "93", "10100011110" });
            this.C128_Code.Rows.Add(new object[] { "94", Convert.ToChar(30).ToString(), "~", "94", "10001011110" });

            this.C128_Code.Rows.Add(new object[] { "95", Convert.ToChar(31).ToString(), Convert.ToChar(127).ToString(), "95", "10111101000" });
            this.C128_Code.Rows.Add(new object[] { "96", FNC3, FNC3, "96", "10111100010" });
            this.C128_Code.Rows.Add(new object[] { "97", FNC2, FNC2, "97", "11110101000" });
            this.C128_Code.Rows.Add(new object[] { "98", "SHIFT", "SHIFT", "98", "11110100010" });
            this.C128_Code.Rows.Add(new object[] { "99", "CODE_C", "CODE_C", "99", "10111011110" });
            this.C128_Code.Rows.Add(new object[] { "100", "CODE_B", FNC4, "CODE_B", "10111101110" });
            this.C128_Code.Rows.Add(new object[] { "101", FNC4, "CODE_A", "CODE_A", "11101011110" });
            this.C128_Code.Rows.Add(new object[] { "102", FNC1, FNC1, FNC1, "11110101110" });
            this.C128_Code.Rows.Add(new object[] { "103", "START_A", "START_A", "START_A", "11010000100" });
            this.C128_Code.Rows.Add(new object[] { "104", "START_B", "START_B", "START_B", "11010010000" });
            this.C128_Code.Rows.Add(new object[] { "105", "START_C", "START_C", "START_C", "11010011100" });
            this.C128_Code.Rows.Add(new object[] { "", "STOP", "STOP", "STOP", "11000111010" });
        }
        private List<DataRow> FindStartorCodeCharacter(string s, ref int col)
        {
            List<DataRow> rows = new List<DataRow>();

            if (s.Length > 1 && (Char.IsNumber(s[0]) || s[0] == FNC1) && (Char.IsNumber(s[1]) || s[1] == FNC1))
            {
                if (StartCharacter == null)
                {
                    StartCharacter = this.C128_Code.Select("A = 'START_C'")[0];
                    rows.Add(StartCharacter);
                }
                else
                    rows.Add(this.C128_Code.Select("A = 'CODE_C'")[0]);

                col = 1;
            }
            else
            {
                bool AFound = false;
                bool BFound = false;
                foreach (DataRow row in this.C128_Code.Rows)
                {
                    try
                    {
                        if (!AFound && s == row["A"].ToString())
                        {
                            AFound = true;
                            col = 2;

                            if (StartCharacter == null)
                            {
                                StartCharacter = this.C128_Code.Select("A = 'START_A'")[0];
                                rows.Add(StartCharacter);
                            }
                            else
                            {
                                rows.Add(this.C128_Code.Select("B = 'CODE_A'")[0]);      
                            }
                        }
                        else if (!BFound && s == row["B"].ToString())
                        {
                            BFound = true;
                            col = 1;

                            if (StartCharacter == null)
                            {
                                StartCharacter = this.C128_Code.Select("A = 'START_B'")[0];
                                rows.Add(StartCharacter);
                            }
                            else
                                rows.Add(this.C128_Code.Select("A = 'CODE_B'")[0]);
                        }
                        else if (AFound && BFound)
                            break;
                    }
                    catch (Exception ex)
                    {
                        Error("EC128-1: " + ex.Message);
                    }
                }                

                if (rows.Count <= 0)
                    Error("EC128-2: Could not determine start character.");
            }

            return rows;
        }
        private string CalculateCheckDigit()
        {
            string currentStartChar = _FormattedData[0];
            uint CheckSum = 0;

            for (uint i = 0; i < _FormattedData.Count; i++)
            {
                string s = _FormattedData[(int)i].Replace("'", "''");

                DataRow[] rows = this.C128_Code.Select("A = '" + s + "'");

                if (rows.Length <= 0)
                    rows = this.C128_Code.Select("B = '" + s + "'");

                if (rows.Length <= 0)
                    rows = this.C128_Code.Select("C = '" + s + "'");

                uint value = UInt32.Parse(rows[0]["Value"].ToString());
                uint addition = value * ((i == 0) ? 1 : i);
                CheckSum +=  addition;
            }

            uint Remainder = (CheckSum % 103);
            DataRow[] RetRows = this.C128_Code.Select("Value = '" + Remainder.ToString() + "'");
            return RetRows[0]["Encoding"].ToString();
        }
        private void BreakUpDataForEncoding()
        {
            string temp = "";
            string tempRawData = Raw_Data;

            switch (this.type)
            {
                case TYPES.A:
                case TYPES.B:
                    {
                        foreach (char c in Raw_Data)
                            _FormattedData.Add(c.ToString());
                        return;
                    }

                case TYPES.C:
                    {
                        int indexOfFirstNumeric = -1;
                        int numericCount = 0;
                        for (int x = 0; x < RawData.Length; x++)
                        {
                            Char c = RawData[x];
                            if (Char.IsNumber(c))
                            {
                                numericCount++;
                                if (indexOfFirstNumeric == -1)
                                {
                                    indexOfFirstNumeric = x;
                                }
                            } else if (c != FNC1)
                            {
                                Error("EC128-6: Only numeric values can be encoded with C128-C (Invalid char at position " + x + ").");
                            }
                        }

                        if (numericCount % 2 == 1)
                            tempRawData = tempRawData.Insert(indexOfFirstNumeric, "0");
                        break;
                    }
            }

            foreach (char c in tempRawData)
            {
                if (Char.IsNumber(c))
                {
                    if (temp == "")
                    {
                        temp += c;
                    }
                    else
                    {
                        temp += c;
                        _FormattedData.Add(temp);
                        temp = "";
                    }
                }
                else
                {
                    if (temp != "")
                    {
                        _FormattedData.Add(temp);
                        temp = "";
                    }
                    _FormattedData.Add(c.ToString());
                }
            }

            if (temp != "")
            {
                _FormattedData.Add(temp);
                temp = "";
            }
        }
        private void InsertStartandCodeCharacters()
        {
            DataRow CurrentCodeSet = null;
            string CurrentCodeString = "";

            if (this.type != TYPES.DYNAMIC)
            {
                switch (this.type)
                {
                    case TYPES.A: _FormattedData.Insert(0, "START_A");
                        break;
                    case TYPES.B: _FormattedData.Insert(0, "START_B");
                        break;
                    case TYPES.C: _FormattedData.Insert(0, "START_C");
                        break;
                    default: Error("EC128-4: Unknown start type in fixed type encoding.");
                        break;
                }
            }
            else
            {
                try
                {
                    for (int i = 0; i < (_FormattedData.Count); i++)
                    {
                        int col = 0;
                        List<DataRow> tempStartChars = FindStartorCodeCharacter(_FormattedData[i], ref col);

                        bool sameCodeSet = false;
                        foreach (DataRow row in tempStartChars)
                        {
                            if (row["A"].ToString().EndsWith(CurrentCodeString) || row["B"].ToString().EndsWith(CurrentCodeString) || row["C"].ToString().EndsWith(CurrentCodeString))
                            {
                                sameCodeSet = true;
                                break;
                            }
                        }

                        if (CurrentCodeString == "" || !sameCodeSet)
                        {
                            CurrentCodeSet = tempStartChars[0];
                            
                                bool error = true;
                                while (error)
                                {
                                    try
                                    {
                                        CurrentCodeString = CurrentCodeSet[col].ToString().Split(new char[] { '_' })[1];
                                        error = false;
                                    }
                                    catch 
                                    { 
                                        error = true;

                                        if (col++ > CurrentCodeSet.ItemArray.Length)
                                            Error("No start character found in CurrentCodeSet.");
                                    }
                                }
                            
                            _FormattedData.Insert(i++, CurrentCodeSet[col].ToString());
                        }
                        
                    }
                }
                catch (Exception ex)
                {
                    Error("EC128-3: Could not insert start and code characters.\n Message: " + ex.Message);
                }
            }
        }
        private string GetEncoding()
        {
            BreakUpDataForEncoding();

            InsertStartandCodeCharacters();

            string CheckDigit = CalculateCheckDigit();

            string Encoded_Data = "";
            foreach (string s in _FormattedData)
            {
                string s1 = s.Replace("'", "''");
                DataRow[] E_Row;

                switch (this.type)
                {
                    case TYPES.A: E_Row = this.C128_Code.Select("A = '" + s1 + "'");
                        break;
                    case TYPES.B: E_Row = this.C128_Code.Select("B = '" + s1 + "'");
                        break;
                    case TYPES.C: E_Row = this.C128_Code.Select("C = '" + s1 + "'");
                        break;
                    case TYPES.DYNAMIC: E_Row = this.C128_Code.Select("A = '" + s1 + "'");

                                        if (E_Row.Length <= 0)
                                        {
                                            E_Row = this.C128_Code.Select("B = '" + s1 + "'");

                                            if (E_Row.Length <= 0)
                                            {
                                                E_Row = this.C128_Code.Select("C = '" + s1 + "'");
                                            }
                                        }
                        break;
                    default: E_Row = null;
                        break;
                }              

                if (E_Row == null || E_Row.Length <= 0)
                    Error("EC128-5: Could not find encoding of a value( " + s1 + " ) in C128 type " + this.type.ToString());

                Encoded_Data += E_Row[0]["Encoding"].ToString();
                _EncodedData.Add(E_Row[0]["Encoding"].ToString());
            }

            Encoded_Data += CalculateCheckDigit();
            _EncodedData.Add(CalculateCheckDigit());

            Encoded_Data += this.C128_Code.Select("A = 'STOP'")[0]["Encoding"].ToString();
            _EncodedData.Add(this.C128_Code.Select("A = 'STOP'")[0]["Encoding"].ToString());

            Encoded_Data += "11";
            _EncodedData.Add("11");

            return Encoded_Data;
        }
        
        #region IBarcode Members

        public string Encoded_Value
        {
            get { return Encode_Code128(); }
        }

        #endregion
    }
}
