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
            pic = new Bitmap(@"C:\Users\madn1\Documents\visual studio 2015\Projects\DataModel\DataModel\Docs Examples\1-list-polozheniya-s-pechatyami.jpg");
            mask = BinaringBitmap(pic);
            mask.Save(@"C:\Users\madn1\Documents\visual studio 2015\Projects\DataModel\DataModel\Docs Examples\maskFile.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }


        /// <summary>
        /// Бинаризация изображения мтеодом пропускания через два вида Threshold
        /// </summary>
        /// <param name="income">Картинка для бинаризации</param>
        /// <returns>Бинаризованная картинка</returns>
        public Bitmap Threshold(Bitmap income)
        {
            Image<Gray, Byte> img = new Image<Gray, Byte>(income);
            Image<Gray, Byte> res = new Image<Gray, Byte>(income);

            Gray lowColor = new Gray(0);
            Gray highColor = new Gray(80);

            try
            {
                CvInvoke.AdaptiveThreshold(img, res, 255, Emgu.CV.CvEnum.AdaptiveThresholdType.GaussianC, Emgu.CV.CvEnum.ThresholdType.Binary, 51, 50);
                CvInvoke.Threshold(res, img, 150, 255, Emgu.CV.CvEnum.ThresholdType.Otsu);
            } catch (Emgu.CV.Util.CvException e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
            
            return img.ToBitmap();
        }

        /// <summary>
        /// Выделение из картинки элементов серого цвета
        /// </summary>
        /// <param name="income"></param>
        /// <returns></returns>
        public Bitmap Greyzation (Bitmap income)
        {
            Bitmap result = new Bitmap(income);
            for (int x = 0; x<result.Width; x++)
            {
                for (int y = 0; y<result.Height; y++)
                {
                    Color pixel = income.GetPixel(x, y);

                    int colorMaxDifference = 40;
                    double brighness = 100;


                    if (pixel.GetBrightness() > brighness/255 || (Math.Max( Math.Abs(pixel.B-pixel.G), Math.Max(Math.Abs(pixel.R - pixel.G), Math.Abs(pixel.B - pixel.R))) > colorMaxDifference))
                    {
                        result.SetPixel(x, y, Color.White);
                    } 
                }
            }
            return result;
        }


        /// <summary>
        /// Метод получения окончательного бинарного изображения
        /// </summary>
        /// <param name="income"></param>
        /// <returns></returns>
        public Bitmap BinaringBitmap(Bitmap income)
        {
           return Threshold(Greyzation(income));
        }

    }
}
