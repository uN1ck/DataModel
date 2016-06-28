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

        /// <summary>
        /// Константа маскимальной разницы в цветовом диапазоне, т.е. max( |r-g|, |r-b|, |g-b| )
        /// </summary>
        public static int MAX_COLOR_DIFFERENCE = 55;
        
        /// <summary>
        /// Константа максимлаьнйо яркости 
        /// </summary>
        public static int MAX_BRIGHTNESS = 118;
        
        public PicturePreprocessor(string name)
        {
            Bitmap pic;
            pic = new Bitmap(@"C:\Users\madn1\Documents\Visual Studio 2015\Projects\DataModel\DataModel\Docs Examples\"+name+".jpg");
            pic = BinaringBitmap(pic);
            pic.Save(@"C:\Users\madn1\Documents\Visual Studio 2015\Projects\DataModel\DataModel\Docs Examples\result.jpg");
        }

        /// <summary>
        /// Бинаризация изображения мтеодом пропускания через два вида Threshold
        /// </summary>
        /// <param name="income">Входная цветная картинка в формате Bgr</param>
        /// <returns>Выходная бинаризованная картинка в формате Gray</returns>
        public static Image<Gray, Byte> Threshold(Image<Bgr, Byte> income)
        {
            Image<Gray, Byte> img = new Image<Gray, byte>(new Size(income.Width, income.Height));
            Image<Gray, Byte> res = new Image<Gray, byte>(new Size(income.Width, income.Height));
            CvInvoke.CvtColor(income, img, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

            try
            {
                CvInvoke.AdaptiveThreshold(img, res, 255, Emgu.CV.CvEnum.AdaptiveThresholdType.GaussianC, Emgu.CV.CvEnum.ThresholdType.Binary, 51, 50);
                CvInvoke.Threshold(res, img, 150, 255, Emgu.CV.CvEnum.ThresholdType.Otsu);
            } catch (Emgu.CV.Util.CvException e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
            
            return img;
        }

        /// <summary>
        /// Метод получения окончательного бинарного изображения
        /// </summary>
        /// <param name="income"> Входная картинка для бинаризации </param>
        /// <returns> Выходная бинаризованная картинка </returns>
        public static Bitmap BinaringBitmap(Bitmap income)
        {
            return Threshold(FastGreyzation(new Image<Bgr, Byte>(income))).ToBitmap();
            //return Threshold(FastGreyzation(MedianBluringDenoiesing( new Image<Bgr, Byte>(income)))).ToBitmap();
        }

        /// <summary>
        /// Выделение из картинки элементов серого цвета с помощью фильтрации из исходного изображения пикселей,
        /// удовлетворяющих условиям подходящей разности цветов (серости) и яркости. Пиксели которые не проходят
        /// эту проверку заменяются белыми, другие остаются неизменны
        /// </summary>
        /// <param name="income"> Входная цветная картинка в формате Bgr </param>
        /// <returns> Выходная посеревшая цветная картинка в формате Bgr </returns>
        public static Image<Bgr, Byte> FastGreyzation(Image<Bgr, Byte> income)
        {
            Image<Bgr, Byte> img = income.Clone();

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

            return img;
        }


        /// <summary>
        /// Метод блюринга изображения, возвращает изображение, с вычетом фона-размытия изображения
        /// </summary>
        /// <param name="income">Входная цветная картинка в формате Bgr</param>
        /// <returns> Выходная обесфоненая картинка</returns>
        public static Image<Bgr, Byte> MedianBluringDenoiesing(Image<Bgr, Byte> income)
        {
            Image<Bgr, Byte> img = new Image<Bgr, byte>(income.Size);
            Image<Bgr, Byte> blured = new Image<Bgr, byte>(income.Size);
            try
            {
                CvInvoke.MedianBlur(income, blured, 71);
                blured = blured.Not();
                blured.Save(@"C:\Users\madn1\Documents\Visual Studio 2015\Projects\DataModel\DataModel\Docs Examples\blured0.jpg");
                
                CvInvoke.Add(income, blured, img);
                img.Save(@"C:\Users\madn1\Documents\Visual Studio 2015\Projects\DataModel\DataModel\Docs Examples\blured1.jpg");
                //img = img.Not();
                
            }   catch (Emgu.CV.Util.CvException ex)
            {
                Console.WriteLine(ex);
                Console.ReadKey();
            }
            img.Save(@"C:\Users\madn1\Documents\Visual Studio 2015\Projects\DataModel\DataModel\Docs Examples\blured2.jpg");

            return img;
        }

    }
}
