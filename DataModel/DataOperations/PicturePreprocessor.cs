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

namespace Enterra.DocumentLayoutAnalysis.Model
{
    /// <summary>
    /// Класс препроцесинга изображения, позволяющий привести изображение к бинарному т.е.
    /// к виду, содержащему только черные и белые пиксели и удалить шумы в изображении.
    /// Это требуется для облегчения работы методов поиска регионов интереса
    /// 
    /// Основной метод очистки: BinringBitmap
    /// </summary>
    public class PicturePreprocessor
    {

        /// <summary>
        /// Константа маскимальной разницы в цветовом диапазоне, т.е. max( |r-g|, |r-b|, |g-b| )
        /// </summary>
        public const int MaxColorDifference = 55;

        /// <summary>
        /// Константа максимлаьнйо яркости 
        /// </summary>
        public const int MaxBrightness = 118;
        
        /// <summary>
        /// Бинаризация изображения мтеодом пропускания через два вида Threshold
        /// </summary>
        /// <param name="inputImage">Входная цветная картинка в формате Bgr</param>
        /// <returns>Выходная бинаризованная картинка в формате Gray</returns>
        public static Image<Gray, Byte> ThresholdingImage(Image<Bgr, Byte> inputImage)
        {
            Image<Gray, Byte> originalImage = new Image<Gray, byte>(new Size(inputImage.Width, inputImage.Height));
            Image<Gray, Byte> processedImage = new Image<Gray, byte>(new Size(inputImage.Width, inputImage.Height));
            CvInvoke.CvtColor(inputImage, originalImage, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

            try
            {
                CvInvoke.AdaptiveThreshold(originalImage, processedImage, 255, Emgu.CV.CvEnum.AdaptiveThresholdType.GaussianC, Emgu.CV.CvEnum.ThresholdType.Binary, 51, 50);
                CvInvoke.Threshold(processedImage, processedImage, 150, 255, Emgu.CV.CvEnum.ThresholdType.Otsu);
            } catch (Emgu.CV.Util.CvException e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
            
            return processedImage;
        }

        /// <summary>
        /// Метод получения окончательного бинарного изображения
        /// </summary>
        /// <param name="inputImage"> Входная картинка для бинаризации </param>
        /// <returns> Выходная бинаризованная картинка </returns>
        public static Bitmap BinarizeImage(Bitmap inputImage)
        {
            return ThresholdingImage(ConvertToGrayscale(new Image<Bgr, Byte>(inputImage))).ToBitmap();
            //return Threshold(FastGreyzation(MedianBluringDenoiesing( new Image<Bgr, Byte>(income)))).ToBitmap();
        }

        /// <summary>
        /// Выделение из картинки элементов серого цвета с помощью фильтрации из исходного изображения пикселей,
        /// удовлетворяющих условиям подходящей разности цветов (серости) и яркости. Пиксели которые не проходят
        /// эту проверку заменяются белыми, другие остаются неизменны
        /// </summary>
        /// <param name="inputImage"> Входная цветная картинка в формате Bgr </param>
        /// <returns> Выходная посеревшая цветная картинка в формате Bgr </returns>
        public static Image<Bgr, Byte> ConvertToGrayscale(Image<Bgr, Byte> inputImage)
        {
            Image<Bgr, Byte> img = inputImage.Clone();

            for (int x = 0; x < img.Rows; x++) 
            {
                for (int y = 0; y < img.Cols; y++)
                {
                    Color pixel = Color.FromArgb(img.Data[x, y, 0], img.Data[x, y, 1], img.Data[x, y, 2]);
                    if (pixel.GetBrightness() > MaxBrightness / 255.0 || (Math.Max(Math.Abs(pixel.B - pixel.G), Math.Max(Math.Abs(pixel.R - pixel.G), Math.Abs(pixel.B - pixel.R))) > MaxColorDifference))
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
        public static Image<Bgr, Byte> ApplyMedianBlurs(Image<Bgr, Byte> inputImage)
        {
            Image<Bgr, Byte> img = new Image<Bgr, byte>(inputImage.Size);
            Image<Bgr, Byte> blured = new Image<Bgr, byte>(inputImage.Size);
            try
            {
                
                CvInvoke.GaussianBlur(inputImage, blured, new Size(21, 21), 16);
                //CvInvoke.MedianBlur(income, blured, 71);
                blured = blured.Not();
                blured.Save(@"C:\Users\madn1\Documents\Visual Studio 2015\Projects\DataModel\DataModel\Docs Examples\blured0.jpg");
                
                CvInvoke.Add(inputImage, blured, img);
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
