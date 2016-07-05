using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace Enterra.DocumentLayoutAnalysis.Model
{
    public abstract class ImageProcessor
    {
        /// <summary>
        /// Оригинал картинки для обработки изображения
        /// </summary>
        protected Image<Gray, byte> raw;
        protected List<TemplateElement> partition = new List<TemplateElement>();

        public IList<TemplateElement> Partition { get { return partition; } }

        public abstract IList<TemplateElement> buildPartition(Bitmap inputImage);

        public int LineWidth { set; get; }
        
        /// <summary>
        /// Метод наложения маски регонов на исходное изображение
        /// Используется для разработки, в релизе не нужен
        /// </summary>
        /// <returns>Исходное изображение с выделенными регонами</returns>
        public Bitmap DrawMask(Bitmap originalPicture = null)
        {
            Bitmap originalImage;
            if (originalPicture == null)
                originalImage = new Bitmap(raw.Width, raw.Height);
            else
                originalImage = new Bitmap(originalPicture);

            Graphics graphics = Graphics.FromImage(originalImage);

            foreach (TemplateElement rect in partition)
            {
                graphics.DrawString(rect.Name, new Font(FontFamily.GenericSerif, 10), Brushes.Chocolate, rect.Rectangle.X, rect.Rectangle.Y);
                graphics.DrawRectangle(new Pen(Color.Red,LineWidth), rect.Rectangle);
            }
            return originalImage;
        }

        /// <summary>
        /// Метод обрезки изображения по прямоугольнику
        /// </summary>
        /// <param name="img">Изображение</param>
        /// <param name="cropArea">Границы обрезки</param>
        /// <returns>Обрезанное изображение</returns>
        private Bitmap cropImage(Bitmap img, Rectangle cropArea)
        {
            return img.Clone(cropArea, img.PixelFormat);
        }

    }
}
