using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;

namespace SmsServer.Helpers
{
    public class ImageHandler
    {
        public static byte[] ReadAndResizeImage(HttpPostedFileBase image, int maxWidth, int maxHeight)
        {
            var imageData = new byte[image.ContentLength];
            var imgOrig = Image.FromStream(image.InputStream);
            if (imgOrig.Width < maxWidth && imgOrig.Height < maxHeight)
            {
                MemoryStream ms2 = new MemoryStream();
                imgOrig.Save(ms2, imgOrig.RawFormat);
                return ms2.ToArray();
            }

            var lnRatio = 0.0m;
            int lnNewWidth = 0;
            int lnNewHeight = 0;

            if (imgOrig.Width > imgOrig.Height)
            {
                lnRatio = (decimal)maxWidth / imgOrig.Width;
                lnNewWidth = maxWidth;
                decimal lnTemp = imgOrig.Height * lnRatio;
                lnNewHeight = (int)lnTemp;
            }
            else
            {
                lnRatio = (decimal)maxHeight / imgOrig.Height;
                lnNewHeight = maxHeight;
                decimal lnTemp = imgOrig.Width * lnRatio;
                lnNewWidth = (int)lnTemp;
            }

            Image resizedImg = new Bitmap(lnNewWidth, lnNewHeight);
            Graphics g = Graphics.FromImage(resizedImg);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(imgOrig, 0, 0, lnNewWidth, lnNewHeight);
            MemoryStream ms = new MemoryStream();
            resizedImg.Save(ms, imgOrig.RawFormat);
            return ms.ToArray();
        }
    }
}