using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using System.Drawing.Imaging;

namespace DataModel.DataOperations
{
    /// <summary>
    /// Класс препроцесинга изображения, позволяющий привести изображение к бинарному т.е.
    /// к виду, содержащему только черные и белые пиксели и удалить шумы в изображении.
    /// Это требуется для облегчения работы методов поиска регионов интереса
    /// 
    /// Основной метод очистки: BinringBitmap
    /// </summary>
    class PicturePreprocessor
    {

        public static int MAX_COLOR_DIFFERENCE = 55;
        public static int MAX_BRIGHTNESS = 115;
        public static int LINES_STANDART_COUNT = 70;

        public PicturePreprocessor()
        {
            Bitmap pic;
            pic = new Bitmap(@"C:\Users\madn1\Documents\visual studio 2015\Projects\DataModel\DataModel\Docs Examples\7.jpg");
            Bitmap buffer = FastGreyzation(pic); //BinaringBitmap(pic);
            buffer.Save(@"C:\Users\madn1\Documents\visual studio 2015\Projects\DataModel\DataModel\Docs Examples\0a_greyzied.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            buffer = Threshold(buffer);
            buffer.Save(@"C:\Users\madn1\Documents\visual studio 2015\Projects\DataModel\DataModel\Docs Examples\0c_final.bmp", System.Drawing.Imaging.ImageFormat.Bmp);  
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
        /// Метод получения окончательного бинарного изображения
        /// </summary>
        /// <param name="income"></param>
        /// <returns></returns>
        public Bitmap BinaringBitmap(Bitmap income)
        {
           return Threshold(FastGreyzation(income));
        }

        /// <summary>
        /// Выделение из картинки элементов серого цвета
        /// </summary>
        /// <param name="income"></param>
        /// <returns></returns>
        public Bitmap FastGreyzation(Bitmap income)
        {
            Image<Bgr, Byte> img = new Image<Bgr, Byte>(income);

            for (int x = 0; x < img.Rows; x++) 
            {
                for (int y = 0; y < img.Cols; y++)
                {
                    Color pixel = Color.FromArgb(img.Data[x, y, 0], img.Data[x, y, 1], img.Data[x, y, 2]);
                    if (pixel.GetBrightness() > MAX_BRIGHTNESS / 255.0 || (Math.Max(Math.Abs(pixel.B - pixel.G), Math.Max(Math.Abs(pixel.R - pixel.G), Math.Abs(pixel.B - pixel.R))) > MAX_COLOR_DIFFERENCE))
                    {
                        img.Data[x, y, 0] = 255;
                        img.Data[x, y, 1] = 255;
                        img.Data[x, y, 2] = 255;
                    }
                }
            }

            return img.ToBitmap();
        }


        public void TotalNoiseCount(Bitmap income)
        {
            double res = income.Size.Width * income.Size.Height;
            double black = 0;

            for (int x = 0; x<income.Width; x++)
            {
                for (int y = 0; y<income.Height; y++)
                {
                    Color pix = income.GetPixel(x, y);
                    if (pix.R == 0 && pix.G == 0 && pix.B == 0)
                        black++;
                }
            }
            Console.WriteLine("Total: " + res + " Black: " + black + " White: " + (res - black) + " Black/Total: " + (black / res));
        }
    }
}
