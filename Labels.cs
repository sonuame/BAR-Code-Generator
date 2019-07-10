using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace BarCodeGenerator
{
    class Labels
    {
        public static Image Label_ITF14(Barcode Barcode, Bitmap img)
        {
            try
            {
                Font font = Barcode.LabelFont;

                using (Graphics g = Graphics.FromImage(img))
                {
                    g.DrawImage(img, (float)0, (float)0);

                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;

                    using (SolidBrush backBrush = new SolidBrush(Barcode.BackColor))
                    {
                        g.FillRectangle(backBrush, new Rectangle(0, img.Height - (font.Height - 2), img.Width, font.Height));
                    }

                    StringFormat f = new StringFormat();
                    f.Alignment = StringAlignment.Center;

                    using (SolidBrush foreBrush = new SolidBrush(Barcode.ForeColor))
                    {
                        g.DrawString(Barcode.AlternateLabel == null ? Barcode.RawData : Barcode.AlternateLabel, font, foreBrush, (float)(img.Width / 2), img.Height - font.Height + 1, f);
                    }

                    using (Pen pen = new Pen(Barcode.ForeColor, (float)img.Height / 16))
                    {
                        pen.Alignment = PenAlignment.Inset;
                        g.DrawLine(pen, new Point(0, img.Height - font.Height - 2), new Point(img.Width, img.Height - font.Height - 2));
                    }

                    g.Save();
                }
                return img;
            }
            catch (Exception ex)
            {
                throw new Exception("ELABEL_ITF14-1: " + ex.Message);
            }
        }

        public static Image Label_Generic(Barcode Barcode, Bitmap img)
        {
            try
            {
                Font font = Barcode.LabelFont;

                using (Graphics g = Graphics.FromImage(img))
                {
                    g.DrawImage(img, (float)0, (float)0);

                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                    StringFormat f = new StringFormat();
                    f.Alignment = StringAlignment.Near;
                    f.LineAlignment = StringAlignment.Near;
                    int LabelX = 0;
                    int LabelY = 0;

                    switch (Barcode.LabelPosition)
                    {
                        case LabelPositions.BOTTOMCENTER:
                            LabelX = img.Width / 2;
                            LabelY = img.Height - (font.Height);
                            f.Alignment = StringAlignment.Center;
                            break;
                        case LabelPositions.BOTTOMLEFT:
                            LabelX = 0;
                            LabelY = img.Height - (font.Height);
                            f.Alignment = StringAlignment.Near;
                            break;
                        case LabelPositions.BOTTOMRIGHT:
                            LabelX = img.Width;
                            LabelY = img.Height - (font.Height);
                            f.Alignment = StringAlignment.Far;
                            break;
                        case LabelPositions.TOPCENTER:
                            LabelX = img.Width / 2;
                            LabelY = 0;
                            f.Alignment = StringAlignment.Center;
                            break;
                        case LabelPositions.TOPLEFT:
                            LabelX = img.Width;
                            LabelY = 0;
                            f.Alignment = StringAlignment.Near;
                            break;
                        case LabelPositions.TOPRIGHT:
                            LabelX = img.Width;
                            LabelY = 0;
                            f.Alignment = StringAlignment.Far;
                            break;
                    }

                    using (SolidBrush backBrush = new SolidBrush(Barcode.BackColor))
                    {
                        g.FillRectangle(backBrush, new RectangleF((float)0, (float)LabelY, (float)img.Width, (float)font.Height));
                    }

                    using (SolidBrush foreBrush = new SolidBrush(Barcode.ForeColor))
                    {
                        g.DrawString(Barcode.AlternateLabel == null ? Barcode.RawData : Barcode.AlternateLabel, font, foreBrush, new RectangleF((float)0, (float)LabelY, (float)img.Width, (float)font.Height), f);
                    }

                    g.Save();
                }
                return img;
            }
            catch (Exception ex)
            {
                throw new Exception("ELABEL_GENERIC-1: " + ex.Message);
            }
        }


        public static Image Label_EAN13(Barcode Barcode, Bitmap img)
        {
            try
            {
                int iBarWidth = Barcode.Width / Barcode.EncodedValue.Length;
                string defTxt = Barcode.RawData;

                using (Font labFont = new Font("Arial", getFontsize(Barcode.Width - Barcode.Width % Barcode.EncodedValue.Length, img.Height, defTxt), FontStyle.Regular))
                {
                    int shiftAdjustment;
                    switch (Barcode.Alignment)
                    {
                        case AlignmentPositions.LEFT:
                            shiftAdjustment = 0;
                            break;
                        case AlignmentPositions.RIGHT:
                            shiftAdjustment = (Barcode.Width % Barcode.EncodedValue.Length);
                            break;
                        case AlignmentPositions.CENTER:
                        default:
                            shiftAdjustment = (Barcode.Width % Barcode.EncodedValue.Length) / 2;
                            break;
                    }

                    using (Graphics g = Graphics.FromImage(img))
                    {
                        g.DrawImage(img, (float)0, (float)0);

                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                        StringFormat f = new StringFormat
                        {
                            Alignment = StringAlignment.Near,
                            LineAlignment = StringAlignment.Near
                        };
                        int LabelY = 0;

                        LabelY = img.Height - labFont.Height;
                        f.Alignment = StringAlignment.Near;

                        float w1 = iBarWidth * 4;    
                        float w2 = iBarWidth * 42;    
                        float w3 = iBarWidth * 42;    

                        float s1 = shiftAdjustment - iBarWidth;
                        float s2 = s1 + (iBarWidth * 4);     
                        float s3 = s2 + w2 + (iBarWidth * 5);     

                        using (SolidBrush backBrush = new SolidBrush(Barcode.BackColor))
                        {
                            g.FillRectangle(backBrush, new RectangleF(s2, (float)LabelY, w2, (float)labFont.Height));
                            g.FillRectangle(backBrush, new RectangleF(s3, (float)LabelY, w3, (float)labFont.Height));

                        }

                        using (SolidBrush foreBrush = new SolidBrush(Barcode.ForeColor))
                        {
                            using (Font smallFont = new Font(labFont.FontFamily, labFont.SizeInPoints * 0.5f, labFont.Style))
                            {
                                g.DrawString(defTxt.Substring(0, 1), smallFont, foreBrush, new RectangleF(s1, (float)img.Height - (float)(smallFont.Height * 0.9), (float)img.Width, (float)labFont.Height), f);
                            }
                            g.DrawString(defTxt.Substring(1, 6), labFont, foreBrush, new RectangleF(s2, (float)LabelY, (float)img.Width, (float)labFont.Height), f);
                            g.DrawString(defTxt.Substring(7), labFont, foreBrush, new RectangleF(s3 - iBarWidth, (float)LabelY, (float)img.Width, (float)labFont.Height), f);
                        }

                        g.Save();
                    }
                }
                return img;
            }
            catch (Exception ex)
            {
                throw new Exception("ELABEL_EAN13-1: " + ex.Message);
            }
        }

        public static Image Label_UPCA(Barcode Barcode, Bitmap img)
        {
            try
            {
                int iBarWidth = (int)(Barcode.Width / Barcode.EncodedValue.Length);
                int halfBarWidth = (int)(iBarWidth * 0.5);
                string defTxt = Barcode.RawData;

                using (Font labFont = new Font("Arial", getFontsize((int)((Barcode.Width - Barcode.Width % Barcode.EncodedValue.Length) * 0.9f), img.Height, defTxt), FontStyle.Regular))
                {
                    int shiftAdjustment;
                    switch (Barcode.Alignment)
                    {
                        case AlignmentPositions.LEFT:
                            shiftAdjustment = 0;
                            break;
                        case AlignmentPositions.RIGHT:
                            shiftAdjustment = (Barcode.Width % Barcode.EncodedValue.Length);
                            break;
                        case AlignmentPositions.CENTER:
                        default:
                            shiftAdjustment = (Barcode.Width % Barcode.EncodedValue.Length) / 2;
                            break;
                    }

                    using (Graphics g = Graphics.FromImage(img))
                    {
                        g.DrawImage(img, (float)0, (float)0);

                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                        StringFormat f = new StringFormat();
                        f.Alignment = StringAlignment.Near;
                        f.LineAlignment = StringAlignment.Near;
                        int LabelY = 0;

                        LabelY = img.Height - labFont.Height;
                        f.Alignment = StringAlignment.Near;

                        float w1 = iBarWidth * 4;    
                        float w2 = iBarWidth * 34;    
                        float w3 = iBarWidth * 34;    

                        float s1 = shiftAdjustment - iBarWidth;
                        float s2 = s1 + (iBarWidth * 12);     
                        float s3 = s2 + w2 + (iBarWidth * 5);     
                        float s4 = s3 + w3 + (iBarWidth * 8) - halfBarWidth;

                        using (SolidBrush backBrush = new SolidBrush(Barcode.BackColor))
                        {
                            g.FillRectangle(backBrush, new RectangleF(s2, (float)LabelY, w2, (float)labFont.Height));
                            g.FillRectangle(backBrush, new RectangleF(s3, (float)LabelY, w3, (float)labFont.Height));
                        }

                        using (SolidBrush foreBrush = new SolidBrush(Barcode.ForeColor))
                        {
                            using (Font smallFont = new Font(labFont.FontFamily, labFont.SizeInPoints * 0.5f, labFont.Style))
                            {
                                g.DrawString(defTxt.Substring(0, 1), smallFont, foreBrush, new RectangleF(s1, (float)img.Height - smallFont.Height, (float)img.Width, (float)labFont.Height), f);
                                g.DrawString(defTxt.Substring(1, 5), labFont, foreBrush, new RectangleF(s2 - iBarWidth, (float)LabelY, (float)img.Width, (float)labFont.Height), f);
                                g.DrawString(defTxt.Substring(6, 5), labFont, foreBrush, new RectangleF(s3 - iBarWidth, (float)LabelY, (float)img.Width, (float)labFont.Height), f);
                                g.DrawString(defTxt.Substring(11), smallFont, foreBrush, new RectangleF(s4, (float)img.Height - smallFont.Height, (float)img.Width, (float)labFont.Height), f);
                            }
                        }

                        g.Save();
                    }
                }
                return img;
            }
            catch (Exception ex)
            {
                throw new Exception("ELABEL_UPCA-1: " + ex.Message);
            }
        }

        public static int getFontsize(int wid, int hgt, string lbl)
        {
            int fontSize = 10;

            if (lbl.Length > 0)
            {
                Image fakeImage = new Bitmap(1, 1);                   

                using (Graphics gr = Graphics.FromImage(fakeImage))
                {
                    for (int i = 1; i <= 100; i++)
                    {
                        using (Font test_font = new Font("Arial", i))
                        {
                            SizeF text_size = gr.MeasureString(lbl, test_font);
                            if ((text_size.Width > wid) || (text_size.Height > hgt))
                            {
                                fontSize = i - 1;
                                break;
                            }
                        }
                    }
                }


            };

            return fontSize;
        }
    }
}
