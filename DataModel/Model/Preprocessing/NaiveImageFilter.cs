using System;
using System.Drawing;

using Emgu.CV;
using Emgu.CV.Structure;

namespace Enterra.DocumentLayoutAnalysis.Model
{
    public class NaiveImageFilter : IImageBinarizationFilter
    {

        private int maxColorDifference;
        /// <summary>
        /// Величина маскимальной разницы в цветовом диапазоне, т.е. max( |r-g|, |r-b|, |g-b| )
        /// </summary>
        public int MaxColorDifference { set { maxColorDifference = value; } get { return maxColorDifference; } }

        private int maxBrightness;
        /// <summary>
        /// Величина максимлаьной яркости 
        /// </summary>
        public int MaxBrightness { set { maxBrightness = value; } get { return maxBrightness; } }

        /// <summary>
        /// Конструктор примитивного бинаризационного фильтра изображения
        /// </summary>
        /// <param name="maxColorDifference">Величина маскимальной разницы в цветовом диапазоне</param>
        /// <param name="maxBrightness">Величина максимлаьной яркости </param>
        public NaiveImageFilter(int maxColorDifference = 55, int maxBrightness = 118)
        {
            MaxColorDifference = maxColorDifference;
            MaxBrightness = maxBrightness;
        }


        /// <summary>
        /// Выделение из картинки элементов серого цвета с помощью фильтрации из исходного изображения пикселей,
        /// удовлетворяющих условиям подходящей разности цветов (серости) и яркости. Пиксели которые не проходят
        /// эту проверку заменяются белыми, другие остаются неизменны
        /// </summary>
        /// <param name="inputImage"> Входная цветная картинка в формате Bgr </param>
        /// <returns> Выходная посеревшая цветная картинка в формате Bgr </returns>
        private Image<Bgr, Byte> ConvertToGrayscale(Image<Bgr, Byte> inputImage)
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
        /// Бинаризация изображения мтеодом пропускания через два вида Threshold
        /// </summary>
        /// <param name="inputImage">Входная цветная картинка в формате Bgr</param>
        /// <returns>Выходная бинаризованная картинка в формате Gray</returns>
        private Image<Gray, Byte> ThresholdingImage(Image<Bgr, Byte> inputImage)
        {
            Image<Gray, Byte> originalImage = new Image<Gray, byte>(new Size(inputImage.Width, inputImage.Height));
            Image<Gray, Byte> processedImage = new Image<Gray, byte>(new Size(inputImage.Width, inputImage.Height));
            CvInvoke.CvtColor(inputImage, originalImage, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);

            try
            {
                CvInvoke.AdaptiveThreshold(originalImage, processedImage, 255, Emgu.CV.CvEnum.AdaptiveThresholdType.GaussianC, Emgu.CV.CvEnum.ThresholdType.Binary, 51, 50);
                CvInvoke.Threshold(processedImage, processedImage, 150, 255, Emgu.CV.CvEnum.ThresholdType.Otsu);
            }
            catch (Emgu.CV.Util.CvException e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }

            return processedImage;
        }

        Bitmap IImageBinarizationFilter.BinarizeImage(Bitmap inputImage)
        {
            return ThresholdingImage(ConvertToGrayscale(new Image<Bgr, Byte>(inputImage))).ToBitmap();
        }

    }
}
