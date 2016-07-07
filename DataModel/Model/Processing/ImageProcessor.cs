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

        public abstract TemplateElementMananger buildPartition(Bitmap inputImage);

        public int LineWidth { set; get; }
        
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
