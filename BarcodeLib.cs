using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Imaging;
using BarCodeGenerator.Symbologies;
using BarcodeStandard;
using System.Xml.Serialization;

namespace BarCodeGenerator
{
    #region Enums
    public enum TYPE : int { UNSPECIFIED, UPCA, UPCE, UPC_SUPPLEMENTAL_2DIGIT, UPC_SUPPLEMENTAL_5DIGIT, EAN13, EAN8, Interleaved2of5, Interleaved2of5_Mod10, Standard2of5, Standard2of5_Mod10, Industrial2of5, Industrial2of5_Mod10, CODE39, CODE39Extended, CODE39_Mod43, Codabar, PostNet, BOOKLAND, ISBN, JAN13, MSI_Mod10, MSI_2Mod10, MSI_Mod11, MSI_Mod11_Mod10, Modified_Plessey, CODE11, USD8, UCC12, UCC13, LOGMARS, CODE128, CODE128A, CODE128B, CODE128C, ITF14, CODE93, TELEPEN, FIM, PHARMACODE };
    public enum SaveTypes : int { JPG, BMP, PNG, GIF, TIFF, UNSPECIFIED };
    public enum AlignmentPositions : int { CENTER, LEFT, RIGHT };
    public enum LabelPositions : int { TOPLEFT, TOPCENTER, TOPRIGHT, BOTTOMLEFT, BOTTOMCENTER, BOTTOMRIGHT };
    #endregion
    public class Barcode : IDisposable
    {
        #region Variables
        private IBarcode ibarcode = new Blank();
        private string Raw_Data = "";
        private string Encoded_Value = "";
        private string _Country_Assigning_Manufacturer_Code = "N/A";
        private TYPE Encoded_Type = TYPE.UNSPECIFIED;
        private Image _Encoded_Image = null;
        private Color _ForeColor = Color.Black;
        private Color _BackColor = Color.White;
        private int _Width = 300;
        private int _Height = 150;
        private string _XML = "";
        private ImageFormat _ImageFormat = ImageFormat.Jpeg;
        private Font _LabelFont = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
        private LabelPositions _LabelPosition = LabelPositions.BOTTOMCENTER;
        private RotateFlipType _RotateFlipType = RotateFlipType.RotateNoneFlipNone;
        private bool _StandardizeLabel = true;
        private bool _embedData = false;
        #endregion

        #region Constructors
        public Barcode()
        {
        }
        public Barcode(string data)
        {
            this.Raw_Data = data;
        }
        public Barcode(string data, TYPE iType)
        {
            this.Raw_Data = data;
            this.Encoded_Type = iType;
        }
        #endregion

        #region Properties
        public string RawData
        {
            get { return Raw_Data; }
            set { Raw_Data = value; }
        }
        public string EncodedValue
        {
            get { return Encoded_Value; }
        }
        public string Country_Assigning_Manufacturer_Code
        {
            get { return _Country_Assigning_Manufacturer_Code; }
        }
        public TYPE EncodedType
        {
            set { Encoded_Type = value; }
            get { return Encoded_Type; }
        }
        public Image EncodedImage
        {
            get
            {
                return _Encoded_Image;
            }
        }
        public Color ForeColor
        {
            get { return this._ForeColor; }
            set { this._ForeColor = value; }
        }
        public Color BackColor
        {
            get { return this._BackColor; }
            set { this._BackColor = value; }
        }
        public Font LabelFont
        {
            get { return this._LabelFont; }
            set { this._LabelFont = value; }
        }
        public LabelPositions LabelPosition
        {
            get { return _LabelPosition; }
            set { _LabelPosition = value; }
        }
        public RotateFlipType RotateFlipType
        {
            get { return _RotateFlipType; }
            set { _RotateFlipType = value; }
        }
        public int Width
        {
            get { return _Width; }
            set { _Width = value; }
        }
        public int Height
        {
            get { return _Height; }
            set { _Height = value; }
        }
        public int? BarWidth { get; set; }
        public double? AspectRatio { get; set; }
        public bool IncludeLabel
        {
            get;
            set;
        }

        public String AlternateLabel
        {
            get;
            set;
        }

        public bool StandardizeLabel
        {
            get { return _StandardizeLabel; }
            set { _StandardizeLabel = value; }
        }

        public double EncodingTime
        {
            get;
            set;
        }
        public string XML
        {
            get { return _XML; }
        }
        public ImageFormat ImageFormat
        {
            get { return _ImageFormat; }
            set { _ImageFormat = value; }
        }
        public List<string> Errors
        {
            get { return this.ibarcode.Errors; }
        }
        public AlignmentPositions Alignment
        {
            get;
            set;
        }
        public byte[] Encoded_Image_Bytes
        {
            get
            {
                if (_Encoded_Image == null)
                    return null;

                using (MemoryStream ms = new MemoryStream())
                {
                    _Encoded_Image.Save(ms, _ImageFormat);
                    return ms.ToArray();
                }
            }
        }
        public static Version Version
        {
            get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version; }
        }
        #endregion

        public class ImageSize
        {
            public ImageSize(double width, double height, bool metric)
            {
                Width = width;
                Height = height;
                Metric = metric;
            }

            public double Width { get; set; }
            public double Height { get; set; }
            public bool Metric { get; set; }
        }

        #region General Encode
        public Image Encode(TYPE iType, string StringToEncode, int Width, int Height, bool EmbedData = false)
        {
            this.Width = Width;
            this.Height = Height;
            this.IncludeLabel = EmbedData;
            return Encode(iType, StringToEncode, EmbedData);
        }   
        public Image Encode(TYPE iType, string StringToEncode, Color ForeColor, Color BackColor, int Width, int Height, bool EmbedData = false)
        {
            this.Width = Width;
            this.Height = Height;
            this.IncludeLabel = EmbedData;
            return Encode(iType, StringToEncode, ForeColor, BackColor, EmbedData);
        }     
        public Image Encode(TYPE iType, string StringToEncode, Color ForeColor, Color BackColor, bool EmbedData = false)
        {
            this.BackColor = BackColor;
            this.ForeColor = ForeColor;
            this.IncludeLabel = EmbedData;
            return Encode(iType, StringToEncode, EmbedData);
        }   
        public Image Encode(TYPE iType, string StringToEncode, bool EmbedData = false)
        {
            Raw_Data = StringToEncode;
            this.IncludeLabel = EmbedData;
            return Encode(iType);
        } 
        internal Image Encode(TYPE iType)
        {
            Encoded_Type = iType;
            return Encode();
        }
        internal Image Encode()
        {
            ibarcode.Errors.Clear();

            DateTime dtStartTime = DateTime.Now;

            if (Raw_Data.Trim() == "")
                throw new Exception("EENCODE-1: Input data not allowed to be blank.");

            if (this.EncodedType == TYPE.UNSPECIFIED)
                throw new Exception("EENCODE-2: Symbology type not allowed to be unspecified.");

            this.Encoded_Value = "";
            this._Country_Assigning_Manufacturer_Code = "N/A";

            switch (this.Encoded_Type)
            {
                case TYPE.UCC12:
                case TYPE.UPCA: 
                    ibarcode = new UPCA(Raw_Data);
                    break;
                case TYPE.UCC13:
                case TYPE.EAN13: 
                    ibarcode = new EAN13(Raw_Data);
                    break;
                case TYPE.Interleaved2of5_Mod10:
                case TYPE.Interleaved2of5: 
                    ibarcode = new Interleaved2of5(Raw_Data, Encoded_Type);
                    break;
                case TYPE.Industrial2of5_Mod10:
                case TYPE.Industrial2of5:
                case TYPE.Standard2of5_Mod10:
                case TYPE.Standard2of5: 
                    ibarcode = new Standard2of5(Raw_Data, Encoded_Type);
                    break;
                case TYPE.LOGMARS:
                case TYPE.CODE39: 
                    ibarcode = new Code39(Raw_Data);
                    break;
                case TYPE.CODE39Extended:
                    ibarcode = new Code39(Raw_Data, true);
                    break;
                case TYPE.CODE39_Mod43:
                    ibarcode = new Code39(Raw_Data, false, true);
                    break;
                case TYPE.Codabar: 
                    ibarcode = new Codabar(Raw_Data);
                    break;
                case TYPE.PostNet: 
                    ibarcode = new Postnet(Raw_Data);
                    break;
                case TYPE.ISBN:
                case TYPE.BOOKLAND: 
                    ibarcode = new ISBN(Raw_Data);
                    break;
                case TYPE.JAN13: 
                    ibarcode = new JAN13(Raw_Data);
                    break;
                case TYPE.UPC_SUPPLEMENTAL_2DIGIT: 
                    ibarcode = new UPCSupplement2(Raw_Data);
                    break;
                case TYPE.MSI_Mod10:
                case TYPE.MSI_2Mod10:
                case TYPE.MSI_Mod11:
                case TYPE.MSI_Mod11_Mod10:
                case TYPE.Modified_Plessey: 
                    ibarcode = new MSI(Raw_Data, Encoded_Type);
                    break;
                case TYPE.UPC_SUPPLEMENTAL_5DIGIT: 
                    ibarcode = new UPCSupplement5(Raw_Data);
                    break;
                case TYPE.UPCE: 
                    ibarcode = new UPCE(Raw_Data);
                    break;
                case TYPE.EAN8: 
                    ibarcode = new EAN8(Raw_Data);
                    break;
                case TYPE.USD8:
                case TYPE.CODE11: 
                    ibarcode = new Code11(Raw_Data);
                    break;
                case TYPE.CODE128: 
                    ibarcode = new Code128(Raw_Data);
                    break;
                case TYPE.CODE128A:
                    ibarcode = new Code128(Raw_Data, Code128.TYPES.A);
                    break;
                case TYPE.CODE128B:
                    ibarcode = new Code128(Raw_Data, Code128.TYPES.B);
                    break;
                case TYPE.CODE128C:
                    ibarcode = new Code128(Raw_Data, Code128.TYPES.C);
                    break;
                case TYPE.ITF14:
                    ibarcode = new ITF14(Raw_Data);
                    break;
                case TYPE.CODE93:
                    ibarcode = new Code93(Raw_Data);
                    break;
                case TYPE.TELEPEN:
                    ibarcode = new Telepen(Raw_Data);
                    break;
                case TYPE.FIM:
                    ibarcode = new FIM(Raw_Data);
                    break;
                case TYPE.PHARMACODE:
                    ibarcode = new Pharmacode(Raw_Data);
                    break;

                default: throw new Exception("EENCODE-2: Unsupported encoding type specified.");
            }

            this.Encoded_Value = ibarcode.Encoded_Value;
            this.Raw_Data = ibarcode.RawData;

            _Encoded_Image = (Image)Generate_Image();

            this.EncodedImage.RotateFlip(this.RotateFlipType);
            
            this.EncodingTime = ((TimeSpan)(DateTime.Now - dtStartTime)).TotalMilliseconds;

            _XML = GetXML();

            return EncodedImage;
        }
        #endregion

        #region Image Functions
        private Bitmap Generate_Image()
        {
            if (Encoded_Value == "") throw new Exception("EGENERATE_IMAGE-1: Must be encoded first.");
            Bitmap bitmap = null;

            DateTime dtStartTime = DateTime.Now;

            switch (this.Encoded_Type)
            {
                case TYPE.ITF14:
                    {
                        if (BarWidth.HasValue)
                        {
                            Width = (int)(241 / 176.9 * Encoded_Value.Length * BarWidth.Value + 1);
                        }
                        Height = (int?)(Width / AspectRatio) ?? Height;

                        int ILHeight = Height;
                        if (IncludeLabel)
                        {
                            ILHeight -= this.LabelFont.Height;
                        }

                        bitmap = new Bitmap(Width, Height);

                        int bearerwidth = (int)((bitmap.Width) / 12.05);
                        int iquietzone = Convert.ToInt32(bitmap.Width * 0.05);
                        int iBarWidth = (bitmap.Width - (bearerwidth * 2) - (iquietzone * 2)) / Encoded_Value.Length;
                        int shiftAdjustment = ((bitmap.Width - (bearerwidth * 2) - (iquietzone * 2)) % Encoded_Value.Length) / 2;

                        if (iBarWidth <= 0 || iquietzone <= 0)
                            throw new Exception("EGENERATE_IMAGE-3: Image size specified not large enough to draw image. (Bar size determined to be less than 1 pixel or quiet zone determined to be less than 1 pixel)");

                        int pos = 0;

                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            g.Clear(BackColor);

                            using (Pen pen = new Pen(ForeColor, iBarWidth))
                            {
                                pen.Alignment = PenAlignment.Right;

                                while (pos < Encoded_Value.Length)
                                {
                                    if (Encoded_Value[pos] == '1')
                                        g.DrawLine(pen, new Point((pos * iBarWidth) + shiftAdjustment + bearerwidth + iquietzone, 0), new Point((pos * iBarWidth) + shiftAdjustment + bearerwidth + iquietzone, Height));

                                    pos++;
                                }

                                pen.Width = (float)ILHeight / 8;
                                pen.Color = ForeColor;
                                pen.Alignment = PenAlignment.Center;
                                g.DrawLine(pen, new Point(0, 0), new Point(bitmap.Width, 0));
                                g.DrawLine(pen, new Point(0, ILHeight), new Point(bitmap.Width, ILHeight));
                                g.DrawLine(pen, new Point(0, 0), new Point(0, ILHeight));
                                g.DrawLine(pen, new Point(bitmap.Width, 0), new Point(bitmap.Width, ILHeight));
                            }
                        }

                        if (IncludeLabel)
                            Labels.Label_ITF14(this, bitmap);

                        break;
                    }
                case TYPE.UPCA:
                    {
                        Width = BarWidth * Encoded_Value.Length ?? Width;

                        Height = (int?)(Width / AspectRatio) ?? Height;

                        int ILHeight = Height;
                        int topLabelAdjustment = 0;

                        int shiftAdjustment = 0;
                        int iBarWidth = Width / Encoded_Value.Length;

                        switch (Alignment)
                        {
                            case AlignmentPositions.LEFT:
                                shiftAdjustment = 0;
                                break;
                            case AlignmentPositions.RIGHT:
                                shiftAdjustment = (Width % Encoded_Value.Length);
                                break;
                            case AlignmentPositions.CENTER:
                            default:
                                shiftAdjustment = (Width % Encoded_Value.Length) / 2;
                                break;
                        }

                        if (IncludeLabel)
                        {
                            if ((AlternateLabel == null || RawData.StartsWith(AlternateLabel)) && _StandardizeLabel)
                            {
                                string defTxt = RawData;
                                string labTxt = defTxt.Substring(0, 1) + "--" + defTxt.Substring(1, 6) + "--" + defTxt.Substring(7);
                                
                                Font labFont = new Font(this.LabelFont != null ? this.LabelFont.FontFamily.Name : "Arial", Labels.getFontsize(Width, Height, labTxt), FontStyle.Regular);
                                if (this.LabelFont != null)
                                {
                                    this.LabelFont.Dispose();
                                }
                                LabelFont = labFont;

                                ILHeight -= (labFont.Height / 2);

                                iBarWidth = (int)Width / Encoded_Value.Length;
                            }
                            else
                            {
                                if ((LabelPosition & (LabelPositions.TOPCENTER | LabelPositions.TOPLEFT | LabelPositions.TOPRIGHT)) > 0)
                                    topLabelAdjustment = this.LabelFont.Height;

                                ILHeight -= this.LabelFont.Height;
                            }
                        }

                        bitmap = new Bitmap(Width, Height);
                        int iBarWidthModifier = 1;
                        if (iBarWidth <= 0)
                            throw new Exception("EGENERATE_IMAGE-2: Image size specified not large enough to draw image. (Bar size determined to be less than 1 pixel)");

                        int pos = 0;
                        int halfBarWidth = (int)(iBarWidth * 0.5);

                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            g.Clear(BackColor);

                            using (Pen backpen = new Pen(BackColor, iBarWidth / iBarWidthModifier))
                            {
                                using (Pen pen = new Pen(ForeColor, iBarWidth / iBarWidthModifier))
                                {
                                    while (pos < Encoded_Value.Length)
                                    {
                                        if (Encoded_Value[pos] == '1')
                                        {
                                            g.DrawLine(pen, new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, topLabelAdjustment), new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, ILHeight + topLabelAdjustment));
                                        }

                                        pos++;
                                    }
                                }
                            }
                        }
                        if (IncludeLabel)
                        {
                            if ((AlternateLabel == null || RawData.StartsWith(AlternateLabel)) && _StandardizeLabel)
                            {
                                Labels.Label_UPCA(this, bitmap);
                            }
                            else
                            {
                                Labels.Label_Generic(this, bitmap);
                            }
                        }

                        break;
                    }
                case TYPE.EAN13:
                    {
                        Width = BarWidth * Encoded_Value.Length ?? Width;

                        Height = (int?)(Width / AspectRatio) ?? Height;

                        int ILHeight = Height;
                        int topLabelAdjustment = 0;

                        int shiftAdjustment = 0;

                        switch (Alignment)
                        {
                            case AlignmentPositions.LEFT:
                                shiftAdjustment = 0;
                                break;
                            case AlignmentPositions.RIGHT:
                                shiftAdjustment = (Width % Encoded_Value.Length);
                                break;
                            case AlignmentPositions.CENTER:
                            default:
                                shiftAdjustment = (Width % Encoded_Value.Length) / 2;
                                break;
                        }

                        if (IncludeLabel)
                        {
                            if (((AlternateLabel == null) || RawData.StartsWith(AlternateLabel)) && _StandardizeLabel)
                            {
                                string defTxt = RawData;
                                string labTxt = defTxt.Substring(0, 1) + "--" + defTxt.Substring(1, 6) + "--" + defTxt.Substring(7);

                                Font font = this.LabelFont;
                                Font labFont = new Font(font != null ? font.FontFamily.Name : "Arial", Labels.getFontsize(Width, Height, labTxt), FontStyle.Regular);

                                if (font != null)
                                {
                                    this.LabelFont.Dispose();
                                }

                                LabelFont = labFont;

                                ILHeight -= (labFont.Height / 2);
                            }
                            else
                            {
                                if ((LabelPosition & (LabelPositions.TOPCENTER | LabelPositions.TOPLEFT | LabelPositions.TOPRIGHT)) > 0)
                                    topLabelAdjustment = this.LabelFont.Height;

                                ILHeight -= this.LabelFont.Height;
                            }
                        }

                        bitmap = new Bitmap(Width, Height);
                        int iBarWidth = Width / Encoded_Value.Length;
                        int iBarWidthModifier = 1;
                        if (iBarWidth <= 0)
                            throw new Exception("EGENERATE_IMAGE-2: Image size specified not large enough to draw image. (Bar size determined to be less than 1 pixel)");

                        int pos = 0;
                        int halfBarWidth = (int)(iBarWidth * 0.5);

                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            g.Clear(BackColor);

                            using (Pen backpen = new Pen(BackColor, iBarWidth / iBarWidthModifier))
                            {
                                using (Pen pen = new Pen(ForeColor, iBarWidth / iBarWidthModifier))
                                {
                                    while (pos < Encoded_Value.Length)
                                    {
                                        if (Encoded_Value[pos] == '1')
                                        {
                                            g.DrawLine(pen, new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, topLabelAdjustment), new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, ILHeight + topLabelAdjustment));
                                        }

                                        pos++;
                                    }
                                }
                            }
                        }
                        if (IncludeLabel)
                        {
                            if (((AlternateLabel == null) || RawData.StartsWith(AlternateLabel)) && _StandardizeLabel)
                            {
                                Labels.Label_EAN13(this, bitmap);
                            }
                            else
                            {
                                Labels.Label_Generic(this, bitmap);
                            }
                        }

                        break;
                    }
                default:
                    {
                        Width = BarWidth * Encoded_Value.Length ?? Width;

                        Height = (int?)(Width / AspectRatio) ?? Height;

                        int ILHeight = Height;
                        int topLabelAdjustment = 0;

                        if (IncludeLabel)
                        {
                            if ((LabelPosition & (LabelPositions.TOPCENTER | LabelPositions.TOPLEFT | LabelPositions.TOPRIGHT)) > 0)
                                topLabelAdjustment = this.LabelFont.Height;

                            ILHeight -= this.LabelFont.Height;
                        }


                        bitmap = new Bitmap(Width, Height);
                        int iBarWidth = Width / Encoded_Value.Length;
                        int shiftAdjustment = 0;
                        int iBarWidthModifier = 1;

                        if (this.Encoded_Type == TYPE.PostNet)
                            iBarWidthModifier = 2;

                        switch (Alignment)
                        {
                            case AlignmentPositions.LEFT:
                                shiftAdjustment = 0;
                                break;
                            case AlignmentPositions.RIGHT:
                                shiftAdjustment = (Width % Encoded_Value.Length);
                                break;
                            case AlignmentPositions.CENTER:
                            default:
                                shiftAdjustment = (Width % Encoded_Value.Length) / 2;
                                break;
                        }

                        if (iBarWidth <= 0)
                            throw new Exception("EGENERATE_IMAGE-2: Image size specified not large enough to draw image. (Bar size determined to be less than 1 pixel)");

                        int pos = 0;
                        int halfBarWidth = (int)Math.Round(iBarWidth * 0.5);

                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            g.Clear(BackColor);

                            using (Pen backpen = new Pen(BackColor, iBarWidth / iBarWidthModifier))
                            {
                                using (Pen pen = new Pen(ForeColor, iBarWidth / iBarWidthModifier))
                                {
                                    while (pos < Encoded_Value.Length)
                                    {
                                        if (this.Encoded_Type == TYPE.PostNet)
                                        {
                                            if (Encoded_Value[pos] == '0')
                                                g.DrawLine(pen, new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, ILHeight + topLabelAdjustment), new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, (ILHeight / 2) + topLabelAdjustment));
                                            else
                                                g.DrawLine(pen, new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, ILHeight + topLabelAdjustment), new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, topLabelAdjustment));
                                        }
                                        else
                                        {
                                            if (Encoded_Value[pos] == '1')
                                                g.DrawLine(pen, new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, topLabelAdjustment), new Point(pos * iBarWidth + shiftAdjustment + halfBarWidth, ILHeight + topLabelAdjustment));
                                        }
                                        pos++;
                                    }
                                }
                            }
                        }
                        if (IncludeLabel)
                        {
                            Labels.Label_Generic(this, bitmap);
                        }

                        break;
                    }
            }

            _Encoded_Image = (Image)bitmap;

            this.EncodingTime += ((TimeSpan)(DateTime.Now - dtStartTime)).TotalMilliseconds;

            return bitmap;
        }
        public byte[] GetImageData(SaveTypes savetype)
        {
            byte[] imageData = null;

            try
            {
                if (_Encoded_Image != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        SaveImage(ms, savetype);
                        imageData = ms.ToArray();
                        ms.Flush();
                        ms.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("EGETIMAGEDATA-1: Could not retrieve image data. " + ex.Message);
            }  
            return imageData;
        }
        public void SaveImage(string Filename, SaveTypes FileType)
        {
            try
            {
                if (_Encoded_Image != null)
                {
                    ImageFormat imageformat;
                    switch (FileType)
                    {
                        case SaveTypes.BMP: imageformat = ImageFormat.Bmp; break;
                        case SaveTypes.GIF: imageformat = ImageFormat.Gif; break;
                        case SaveTypes.JPG: imageformat = ImageFormat.Jpeg; break;
                        case SaveTypes.PNG: imageformat = ImageFormat.Png; break;
                        case SaveTypes.TIFF: imageformat = ImageFormat.Tiff; break;
                        default: imageformat = ImageFormat; break;
                    }
                    ((Bitmap)_Encoded_Image).Save(Filename, imageformat);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ESAVEIMAGE-1: Could not save image.\n\n=======================\n\n" + ex.Message);
            }
        } 
        public void SaveImage(Stream stream, SaveTypes FileType)
        {
            try
            {
                if (_Encoded_Image != null)
                {
                    ImageFormat imageformat;
                    switch (FileType)
                    {
                        case SaveTypes.BMP: imageformat = ImageFormat.Bmp; break;
                        case SaveTypes.GIF: imageformat = ImageFormat.Gif; break;
                        case SaveTypes.JPG: imageformat = ImageFormat.Jpeg; break;
                        case SaveTypes.PNG: imageformat = ImageFormat.Png; break;
                        case SaveTypes.TIFF: imageformat = ImageFormat.Tiff; break;
                        default: imageformat = ImageFormat; break;
                    }
                    ((Bitmap)_Encoded_Image).Save(stream, imageformat);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ESAVEIMAGE-2: Could not save image.\n\n=======================\n\n" + ex.Message);
            }
        } 

        public ImageSize GetSizeOfImage(bool Metric)
        {
            double Width = 0;
            double Height = 0;
            if (this.EncodedImage != null && this.EncodedImage.Width > 0 && this.EncodedImage.Height > 0)
            {
                double MillimetersPerInch = 25.4;
                using (Graphics g = Graphics.FromImage(this.EncodedImage))
                {
                    Width = this.EncodedImage.Width / g.DpiX;
                    Height = this.EncodedImage.Height / g.DpiY;

                    if (Metric)
                    {
                        Width *= MillimetersPerInch;
                        Height *= MillimetersPerInch;
                    }
                }
            }

            return new ImageSize(Width, Height, Metric);
        }
        #endregion

        #region XML Methods
        private string GetXML()
        {
            if (EncodedValue == "")
                throw new Exception("EGETXML-1: Could not retrieve XML due to the barcode not being encoded first.  Please call Encode first.");
            else
            {
                try
                {
                    using (SaveData xml = new SaveData())
                    {
                        xml.Type = EncodedType.ToString();
                        xml.RawData = RawData;
                        xml.EncodedValue = EncodedValue;
                        xml.EncodingTime = EncodingTime;
                        xml.IncludeLabel = IncludeLabel;
                        xml.Forecolor = ColorTranslator.ToHtml(ForeColor);
                        xml.Backcolor = ColorTranslator.ToHtml(BackColor);
                        xml.CountryAssigningManufacturingCode = Country_Assigning_Manufacturer_Code;
                        xml.ImageWidth = Width;
                        xml.ImageHeight = Height;
                        xml.RotateFlipType = RotateFlipType;
                        xml.LabelPosition = (int)LabelPosition;
                        xml.LabelFont = LabelFont.ToString();
                        xml.ImageFormat = ImageFormat.ToString();
                        xml.Alignment = (int)Alignment;

                        using (MemoryStream ms = new MemoryStream())
                        {
                            EncodedImage.Save(ms, ImageFormat);
                            xml.Image = Convert.ToBase64String(ms.ToArray(), Base64FormattingOptions.None);
                        }

                        XmlSerializer writer = new XmlSerializer(typeof(SaveData));
                        StringWriter sw = new StringWriter();
                        writer.Serialize(sw, xml);
                        return sw.ToString();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("EGETXML-2: " + ex.Message);
                }
            }
        }
        public static SaveData GetSaveDataFromFile(string fileContents)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
                SaveData saveData;
                using (TextReader reader = new StringReader(fileContents))
                {
                    saveData = (SaveData)serializer.Deserialize(reader);
                }

                return saveData;
            }
            catch (Exception ex)
            {
                throw new Exception("EGETIMAGEFROMXML-1: " + ex.Message);
            }
        }
        public static Image GetImageFromXML(String internalXML)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
                SaveData result;
                using (TextReader reader = new StringReader(internalXML))
                {
                    result = (SaveData)serializer.Deserialize(reader);
                }
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(result.Image)))
                {
                    return Image.FromStream(ms);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("EGETIMAGEFROMXML-1: " + ex.Message);
            }
        }
        #endregion

        #region Static Encode Methods
        public static Image DoEncode(TYPE iType, string Data)
        {
            using (Barcode b = new Barcode())
            {
                return b.Encode(iType, Data);
            }
        }
        public static Image DoEncode(TYPE iType, string Data, ref string XML)
        {
            using (Barcode b = new Barcode())
            {
                Image i = b.Encode(iType, Data);
                XML = b.XML;
                return i;
            }
        }
        public static Image DoEncode(TYPE iType, string Data, bool IncludeLabel)
        {
            using (Barcode b = new Barcode())
            {
                b.IncludeLabel = IncludeLabel;
                return b.Encode(iType, Data);
            }
        }
        public static Image DoEncode(TYPE iType, string Data, bool IncludeLabel, int Width, int Height)
        {
            using (Barcode b = new Barcode())
            {
                b.IncludeLabel = IncludeLabel;
                return b.Encode(iType, Data, Width, Height);
            }
        }
        public static Image DoEncode(TYPE iType, string Data, bool IncludeLabel, Color DrawColor, Color BackColor)
        {
            using (Barcode b = new Barcode())
            {
                b.IncludeLabel = IncludeLabel;
                return b.Encode(iType, Data, DrawColor, BackColor);
            }
        }
        public static Image DoEncode(TYPE iType, string Data, bool IncludeLabel, Color DrawColor, Color BackColor, int Width, int Height)
        {
            using (Barcode b = new Barcode())
            {
                b.IncludeLabel = IncludeLabel;
                return b.Encode(iType, Data, DrawColor, BackColor, Width, Height);
            }
        }
        public static Image DoEncode(TYPE iType, string Data, bool IncludeLabel, Color DrawColor, Color BackColor, int Width, int Height, ref string XML)
        {
            using (Barcode b = new Barcode())
            {
                b.IncludeLabel = IncludeLabel;
                Image i = b.Encode(iType, Data, DrawColor, BackColor, Width, Height);
                XML = b.XML;
                return i;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;     

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                try
                {
                    LabelFont.Dispose();
                    LabelFont = null;

                    _Encoded_Image.Dispose();
                    _Encoded_Image = null;

                    _XML = null;
                    Raw_Data = null;
                    Encoded_Value = null;
                    _Country_Assigning_Manufacturer_Code = null;
                    _ImageFormat = null;
                }
                catch (Exception ex)
                {
                    throw new Exception("EDISPOSE-1: " + ex.Message);
                }

                disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }
        #endregion

        #endregion
    } 
} 