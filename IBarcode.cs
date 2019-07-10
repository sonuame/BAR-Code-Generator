using System.Collections.Generic;

namespace BarCodeGenerator
{
    interface IBarcode
    {
        string Encoded_Value
        {
            get;
        }

        string RawData
        {
            get;
        }

        List<string> Errors
        {
            get;
        }

    }
}
