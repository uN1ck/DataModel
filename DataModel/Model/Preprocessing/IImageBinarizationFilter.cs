using System.ComponentModel;
using System.Drawing;

namespace Enterra.DocumentLayoutAnalysis.Model
{ 
    /// <summary>
    /// Интерфейс классов для фильтрации изображения, используетс яв препроцессинге
    /// </summary>
    public interface IImageBinarizationFilter
    {
        /// <summary>
        /// Метод приведения изображения к бинарному
        /// </summary>
        /// <param name="inputImage">Оригинал изображения</param>
        /// <returns>Бинаризованное изображение</returns>
        Bitmap BinarizeImage(Bitmap inputImage);
    }
}
