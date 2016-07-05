using System;
using System.ComponentModel;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Enterra.DocumentLayoutAnalysis.Model
{
    public class DilatedImageFilter : IImageBinarizationFilter
    {
       
        private int maxBrightness;
        /// <summary>
        /// Величина максимлаьной яркости 
        /// </summary>
        public int MaxBrightness { set { maxBrightness = value; } get { return maxBrightness; } }

        private int dilateCount;
        /// <summary>
        /// Количество итерации операции Dilate (Emgu CV)
        /// http://www.emgu.com/wiki/files/3.1.0/document/html/25f5c3ea-ed93-fcbd-20b7-b046874f8fbe.htm
        /// </summary>
        public int DilateCount { set { dilateCount = value; } get { return dilateCount; } }

        private int kernelSize;

        /// <summary>
        /// Диаметр ядра для операции Dilate
        /// </summary>
        public int KernelSize { set { kernelSize = value; } get { return kernelSize; } }

        /// <summary>
        /// Конструктор бинаризационного фильтра оснвоанного на Dilate-методе (Emgu CV)
        /// http://www.emgu.com/wiki/files/3.1.0/document/html/25f5c3ea-ed93-fcbd-20b7-b046874f8fbe.htm
        /// </summary>
        /// <param name="maxBrightness">Величина маскимальной разницы в цветовом диапазоне</param>
        /// <param name="dilateCount">Количество итерации операции Dilate (Emgu CV)</param>
        public DilatedImageFilter( int maxBrightness = 118, int dilateCount = 10, int kernelSize = 3)
        {
            MaxBrightness = maxBrightness;
            DilateCount = dilateCount;
            KernelSize = kernelSize;
        }

        Bitmap IImageBinarizationFilter.BinarizeImage(Bitmap inputImage)
        {
            Image<Bgr, Byte> originalImage = new Image<Bgr, Byte>(inputImage);
            Image<Gray, Byte> grayImage = new Image<Gray, Byte>(inputImage.Size);
            Image<Gray, Byte> thresholdedImage = new Image<Gray, Byte>(inputImage.Size);
            Image<Gray, Byte> dilatedImage = new Image<Gray, Byte>(inputImage.Size);

            CvInvoke.CvtColor(originalImage, grayImage, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
            CvInvoke.Threshold(grayImage, thresholdedImage, MaxBrightness, 255, Emgu.CV.CvEnum.ThresholdType.BinaryInv);
            Mat morphKernel = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Ellipse, new Size(KernelSize, KernelSize), new Point(-1, -1));
            CvInvoke.Dilate(thresholdedImage, dilatedImage, morphKernel, new Point(-1, -1), DilateCount, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar(0));

            dilatedImage.Save(@"C:\Users\madn1\Documents\Visual Studio 2015\Projects\DataModel\DataModel\Docs Examples\Crops\preprocessed.jpg");
            return dilatedImage.ToBitmap();
        }

    }
}
