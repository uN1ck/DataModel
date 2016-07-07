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
        /// Процент покрытия прямоугольника прямоугольником, определяющий считать ли их пересекающимися
        /// </summary>
        public double RectangleCrossPercent { set; get; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="inputImage">Входное изображение</param>
        /// <param name="sizeOfMinimumRegion">Площадь минимального регона</param>
        public CannyContourImageProcessor(int sizeOfMinimumRegion = 600, double cannyThreshold = 80.0, double cannyThresLinking = 120.0, double rectangleCrossPercent = 5, bool isCannyActive = true)
        {
            RectangleCrossPercent = rectangleCrossPercent;
            SizeOfMinimumRegion = sizeOfMinimumRegion;
            CannyThreshold = cannyThreshold;
            CannyThresLinking = cannyThresLinking;
            IsCannyActive = isCannyActive;
        }


        public override TemplateElementMananger buildPartition(Bitmap inputImage)
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
            
            return new TemplateElementMananger(mergeCrossingRectangles(partition));
        }

        /// <summary>
        /// Метод пересечения прямоугольников с учетом их покрытия друг другом
        /// </summary>
        /// <param name="l">Прямоугольник относитеьно котрого вычисляется процент покрытия</param>
        /// <param name="r">Прямоугольник</param>
        /// <returns>Истину, если прямоугольник l покрыл прямоугольник r на <see cref="RectangleCrossPercent" от меньшего/> процентов</returns>
        private bool rectangleCross(Rectangle l, Rectangle r)
        {
            double size = Math.Min(r.Width * r.Height, l.Width * l.Height) / 100 * RectangleCrossPercent;

            Rectangle intersection = new Rectangle(l.Location, l.Size);
            intersection.Intersect(r);
            return intersection.Width * intersection.Height < size;
        }

        /// <summary>
        /// Метод слияния пересекающихся прямоугольников
        /// </summary>
        private List<TemplateElement> mergeCrossingRectangles(List<TemplateElement> inputPartition)
        {
            List<TemplateElement> partition = new List<TemplateElement>(inputPartition);

            bool isAdding = false;
            partition.Sort((TemplateElement l, TemplateElement r) =>
            {
                return ((l.Rect.Width * l.Rect.Height > r.Rect.Width * r.Rect.Height) ? 1 : -1);
            });

            for (int i = 0; i < partition.Count; i++)
            {
                for (int k = i+1; k < partition.Count; k++)
                    if ((i != k) && (!rectangleCross(partition[k].Rect, partition[i].Rect)))
                    {
                        if (isAdding)
                        {
                            Point leftUpCorner = new Point(Math.Min(partition[i].Rect.X, partition[k].Rect.X), Math.Min(partition[i].Rect.Y, partition[k].Rect.Y));
                            Point rightBottomCorner = new Point(Math.Min(partition[i].Rect.Right, partition[k].Rect.Right) - leftUpCorner.X, Math.Min(partition[i].Rect.Bottom, partition[k].Rect.Bottom) - leftUpCorner.Y);

                            partition.Add(new TemplateElement(new Rectangle(leftUpCorner.X, leftUpCorner.Y, rightBottomCorner.X, rightBottomCorner.Y), "UN_" + i));

                            partition.RemoveAt(i);
                            partition.RemoveAt(k);
                        }
                        else
                        {
                            partition.RemoveAt(i);
                        }

                        i--;
                        break;
                    }
            }
            return partition;
        }
    }
}
