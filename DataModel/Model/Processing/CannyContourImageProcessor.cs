using System;
using System.Collections.Generic;

using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using System.Drawing;

namespace Enterra.DocumentLayoutAnalysis.Model
{
    public class CannyContourImageProcessor : ImageProcessor
    {
        private int sizeOfMinimumRegion;
        /// <summary>
        /// Площадь минимального регона
        /// </summary>
        public int SizeOfMinimumRegion { set { sizeOfMinimumRegion = value; } get { return sizeOfMinimumRegion; } }

        private double cannyThreshold;
        /// <summary>
        /// Тресхолд для canny-метода поиска граней
        /// </summary>
        public double CannyThreshold { set { cannyThreshold = value; } get { return cannyThreshold; } }

        private double cannyThresLinking;
        /// <summary>
        /// Тресхолд для canny-метода объединения граней
        /// </summary>
        public double CannyThresLinking { set { cannyThresLinking = value; } get { return cannyThresLinking; } }

        private bool isCannyActive;
        /// <summary>
        /// Используется ли метод нахождения граней Canny при выполнении процессинга
        /// </summary>
        public bool IsCannyActive { set { isCannyActive = value; } get { return isCannyActive; } }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="inputImage">Входное изображение</param>
        /// <param name="sizeOfMinimumRegion">Площадь минимального регона</param>
        public CannyContourImageProcessor(int sizeOfMinimumRegion = 216, double cannyThreshold = 80.0, double cannyThresLinking = 120.0, bool isCannyActive = true)
        {

            SizeOfMinimumRegion = sizeOfMinimumRegion;
            CannyThreshold = cannyThreshold;
            CannyThresLinking = cannyThresLinking;
            IsCannyActive = isCannyActive;
        }



        public override IList<TemplateElement> buildPartition(Bitmap inputImage)
        {
            raw = new Image<Gray, byte>(inputImage);
            List<TemplateElement> partition = new List<TemplateElement>();
            try
            {
                Image<Gray, Byte> res = raw.Canny(cannyThreshold, cannyThresLinking);
                IInputOutputArray cannyEdges;

                if (isCannyActive)
                    cannyEdges = res;
                else
                    cannyEdges = raw;

                Image<Gray, Byte> result = new Image<Gray, byte>(raw.Size);

                using (Mat hierachy = new Mat())
                using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
                {
                    CvInvoke.FindContours(cannyEdges, contours, hierachy, Emgu.CV.CvEnum.RetrType.Tree, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxNone);

                    for (int i = 0; i < contours.Size; i++)
                    {
                        Rectangle current = CvInvoke.BoundingRectangle(contours[i]);
                        if (current.Width * current.Height > sizeOfMinimumRegion)                            
                            partition.Add(new TemplateElement(current, "R-" + i));   
                    }
                }
            }
            catch (Emgu.CV.Util.CvException ex)
            {
                throw new Exception(ex.Message);
            }
            this.partition = partition;
            mergeCrossingRectangles();
            return partition;
        }

        private bool rectangleContains(Rectangle l, Rectangle r)
        {
            return (l.X <= r.X && l.Y <= r.Y && l.Right >= r.Right && l.Bottom >= r.Bottom);
        }

        private void mergeCrossingRectangles()
        {
            partition.Sort((TemplateElement l, TemplateElement r) =>
            {
                return ((l.Rectangle.Width * l.Rectangle.Height > r.Rectangle.Width * r.Rectangle.Height) ? 1 : -1);
            });
            for (int i = 0; i < partition.Count; i++)
            {
                bool detected = true;
                for (int k = i+1; k < partition.Count; k++)
                    if ((i != k) && (rectangleContains(partition[k].Rectangle, partition[i].Rectangle)))
                    {
                        partition.RemoveAt(i);
                        i--;
                        break;
                    }
            }
        }

        
    }
}
