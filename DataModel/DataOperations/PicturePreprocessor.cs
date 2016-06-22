using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;

namespace DataModel.DataOperations
{
    class PicturePreprocessor
    {
        public Bitmap pic { set; get; }
        public Bitmap mask { set;  get; }
        

        public PicturePreprocessor()
        {
            pic = new Bitmap(@"C:\Users\madn1\Documents\visual studio 2015\Projects\DataModel\DataModel\Docs Examples\snils.jpg");
            mask = Threshold(pic);
            mask.Save(@"C:\Users\madn1\Documents\visual studio 2015\Projects\DataModel\DataModel\Docs Examples\maskFile.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }



        public Bitmap Greyzation(Bitmap pic)
        {
            mask = new Bitmap(pic);
            for (int x = 0; x < pic.Width; x++)
            {
                for (int y = 0; y < pic.Height; y++)
                {
                    int color = (int)(pic.GetPixel(x, y).GetBrightness() * 255);
                    mask.SetPixel(x, y, Color.FromArgb(255, color, color, color));
                }
            }
            return mask;
        }
        

        public Bitmap Binarize(Bitmap pic)
        {
            mask = new Bitmap(pic);
            for (int x = 0; x<pic.Width; x++)
            {
                for (int y = 0; y< pic.Height; y++)
                {
                    
                    Color pixel = mask.GetPixel(x, y);
                    if (pixel.GetBrightness() < 0.2)
                        mask.SetPixel(x, y, Color.Black);
                    else
                        mask.SetPixel(x, y, Color.White);
                }
            }
            return mask;
        }


        public Bitmap Threshold(Bitmap income)
        {
            Image<Bgr, Byte> img = new Image<Bgr, Byte>(income);
            Image<Bgr, Byte> res = new Image<Bgr, Byte>(income);

            CvInvoke.Dct(img, res, Emgu.CV.CvEnum.DctType.Forward);

            //CvInvoke.AdaptiveThreshold(img, res, 255, Emgu.CV.CvEnum.AdaptiveThresholdType.GaussianC, Emgu.CV.CvEnum.ThresholdType.Binary, 3, 1);
            //CvInvoke.Threshold(img, res, 110, 255, Emgu.CV.CvEnum.ThresholdType.Binary);
            //CvInvoke.FastNlMeansDenoising(res, img);
            //CvInvoke.FindContours(img, res, conts, Emgu.CV.CvEnum.RetrType.Tree, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxNone);
            //CvInvoke.Threshold(res, img, 0, 100, Emgu.CV.CvEnum.ThresholdType.BinaryInv);
            //res.ToBitmap().Save(@"C:\Users\madn1\Documents\visual studio 2015\Projects\DataModel\DataModel\Docs Examples\ClearedFile.bmp");
            return res.ToBitmap();
        }


        public Bitmap Denoiseing(Bitmap income)
        {
            Bitmap result = new Bitmap(income);

            return result;
        }

    }
}
