using System.Collections.Generic;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;

namespace Enterra.DocumentLayoutAnalysis.Model
{
    /// <summary>
    /// Интерфейс поиска регионов на изображении
    /// </summary>
    public interface IImageProcessor
    {

        /// <summary>
        /// Метод построения разбиения на регионы
        /// </summary>
        /// <param name="inputImage">Исходное изображение</param>
        /// <returns>Набор регионов изображения</returns>
        TemplateElementMananger buildPartition(Bitmap inputImage);

    }
}
