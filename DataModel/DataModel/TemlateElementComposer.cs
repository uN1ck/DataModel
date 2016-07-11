using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enterra.DocumentLayoutAnalysis.Model
{
    /// <summary>
    /// Класс собирающий работу с изображением воедино, отвечает за выполнение препроцессинга, процессинга и предварительную отрисовку а так же за работу с регионами
    /// </summary>
    public class TemlateElementComposer
    {

        private Bitmap original;
        /// <summary>
        /// Оригинал изображения
        /// </summary>
        public Bitmap Original
        {
            set
            {
                original = new Bitmap(value);
                preprocessed = new Bitmap(value);
                preview = new Bitmap(value);
                TemplateElementMananger.OriginalSize = original.Size;
                RaiseRedrawElemetsEvent();
            }
            get
            {
                return original;
            }
        }

        private Bitmap preprocessed;
        /// <summary>
        /// Изображение пропущенное через фильтры препроцессинга
        /// </summary>
        public Bitmap Preprocessed { get { return preprocessed; } }

        public Bitmap preview;
        /// <summary>
        /// Предварительный просмотр разбиения на регионы
        /// </summary>
        public Bitmap Preview { get { return preview; } }

        /// <summary>
        /// Интерфейс препроцессинга
        /// </summary>
        public IImageBinarizationFilter ImageBinarizationFilter { set; get; }
        /// <summary>
        /// Интерфейс процессинга
        /// </summary>
        public IImageProcessor ImageProcessor { set; get; }
        /// <summary>
        /// Интерфейс анализа изображения на основе примера
        /// </summary>
        public ITemplateAnalyse TemplateAnalyse { set; get; }
     
        private TemplateElementMananger templateElementMananger;
        /// <summary>
        /// Текущее разбиение на регионы
        /// </summary>
        public TemplateElementMananger TemplateElementMananger {
            get { return templateElementMananger; }
        }

        public TemlateElementComposer(Bitmap inputImage = null)
        {
            templateElementMananger = new TemplateElementMananger();
            if (inputImage != null)
            {
                Original = new Bitmap(inputImage);
                preprocessed = new Bitmap(inputImage);
                preview = new Bitmap(inputImage);
            }
            else
            {
                Original = new Bitmap(1, 1);
                preprocessed = new Bitmap(1, 1);
                preview = new Bitmap(1, 1);
            }
            
        }

        /// <summary>
        /// Метод препроцессинга изображения, сохраняет данные препроцесинга как в класс так и возвращает полученное изображение
        /// </summary>
        public void PreprocessImage()
        {
            preprocessed = ImageBinarizationFilter.BinarizeImage(Original);
            RaiseRedrawElemetsEvent();
        }

        /// <summary>
        /// Метод выделения регионов на изображении, сохраняет данные препроцесинга как в класс так и возвращает полученное изображение
        /// </summary>
        public void DetectRegions()
        {
            templateElementMananger = ImageProcessor.buildPartition(Preprocessed);
            TemplateElementMananger.TempleateElementsChangedHandler += onTempleateElementsContainerChanged;
            drawRectangles();
            RaiseRedrawElemetsEvent();
        }

        /// <summary>
        /// Метод фильтрации регионов
        /// </summary>
        /// <param name="distance"></param>
        public void FilterRegions(double distance)
        {
            templateElementMananger = TemplateAnalyse.FilterRegions(TemplateElementMananger);
            drawRectangles();
            RaiseRedrawElemetsEvent();
        }

        /// <summary>
        /// Метод отрисовки регионов изображения
        /// </summary>
        private void drawRectangles()
        {
            preview = new Bitmap(original);
            Graphics rectangles = Graphics.FromImage(preview);
            foreach (TemplateElement current in TemplateElementMananger.TemplateElementContainer)
                rectangles.DrawRectangle(new Pen(Brushes.Red,5), current.Rectangle);
            //TODO: размер изменить

        }


        public delegate void RedrawElemetsEvent(TemlateElementComposer sender);

        public event RedrawElemetsEvent RedrawElemetsHandler;

        protected virtual void RaiseRedrawElemetsEvent()
        {
            if (RedrawElemetsHandler != null)
            {
                RedrawElemetsHandler(this);
            }
        }

        protected void onTempleateElementsContainerChanged(TemplateElementMananger templateElementContainer)
        {
            templateElementMananger = templateElementContainer;
            drawRectangles();
            RaiseRedrawElemetsEvent();
        }
    }
}
