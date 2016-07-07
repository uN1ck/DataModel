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
        public Bitmap Original { set
            {
                original = new Bitmap(value);
                Preprocessed = new Bitmap(value);
                Masked = new Bitmap(value);
            }
            get { return original; }
        }
        public Bitmap Preprocessed { set; get; }
        public Bitmap Masked { set; get; }

        public IImageBinarizationFilter ImageBinarizationFilter { set; get; }
        public ImageProcessor ImageProcessor { set; get; }
        public TemplateElementMananger TemplateElementMananger { set; get; }


        public delegate void RedrawElemets(TemlateElementComposer sender);
        public event RedrawElemets RedrawElemetsHandler;
        public virtual void RaiseRedrawElemetsEvent()
        {
            if (RedrawElemetsHandler != null)
            {
                RedrawElemetsHandler(this);
            }
        }

        protected void onTempleateElementsContainerChanged(List<TemplateElement> templateElementContainer)
        {
            TemplateElementMananger = new TemplateElementMananger(templateElementContainer);
            drawRectangles();
            RaiseRedrawElemetsEvent();
        }

        public TemlateElementComposer(Bitmap inputImage = null)
        {
            if (inputImage != null)
            {
                Original = new Bitmap(inputImage);
                Preprocessed = new Bitmap(inputImage);
                Masked = new Bitmap(inputImage);
            }
            else
            {
                Original = new Bitmap(1,1);
                Preprocessed = new Bitmap(1, 1);
                Masked = new Bitmap(1, 1);
            }
        }

        public void PreprocessImage()
        {
            Preprocessed = ImageBinarizationFilter.BinarizeImage(Original);
        }

        public void ProcessImage()
        {
            TemplateElementMananger = ImageProcessor.buildPartition(Preprocessed);
            TemplateElementMananger.TempleateElementsChangedHandler += onTempleateElementsContainerChanged;
            drawRectangles();
        }

        private void drawRectangles()
        {
            Masked = new Bitmap(original);
            Graphics rectangles = Graphics.FromImage(Masked);
            foreach (TemplateElement current in TemplateElementMananger.TemplateElementContainer)
                rectangles.DrawRectangle(new Pen(Brushes.Red,ImageProcessor.LineWidth), current.Rect);
        }

    }
}
