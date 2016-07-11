using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enterra.DocumentLayoutAnalysis.Model
{
    /// <summary>
    /// Класс обработки разбиения на основе примеров ранее разбитых документов.
    /// Вычищает регионы не проходящие проверку ан похожесть с примером разбиения
    /// </summary>
    public class TemplateElementSampleMananger : ITemplateAnalyse
    {

        /// <summary>
        /// Ключевые точки разбиения региона, отдалене которых на MaxDistance допустимо
        /// </summary>
        private List<PointF> documentLayoutTemplate;
        /// <summary>
        /// Максимальное отличие в расстоянии заданого разбиения от оригинала
        /// </summary>
        public double MaxDistance { set; get; }


        public TemplateElementSampleMananger()
        {
            documentLayoutTemplate = new List<PointF>();
        }
     

        public void BuildRegionsSample(TemplateElementMananger inputSample)
        {
            foreach(TemplateElement current in inputSample.TemplateElementContainer)
            {
                documentLayoutTemplate.Add(new PointF(current.Rectangle.Location.X/ (float)inputSample.OriginalSize.Width, current.Rectangle.Location.X / (float)inputSample.OriginalSize.Height) );
            }
        }

        public void BuildRegionsSample(List<TemplateElementMananger> inputTemplateList)
        {
            throw new NotImplementedException();
        }

        public TemplateElementMananger FilterRegions(TemplateElementMananger inputTemplate)
        {
            TemplateElementMananger result = new TemplateElementMananger();

            foreach (TemplateElement template in inputTemplate.TemplateElementContainer)
            {
                foreach (PointF sample in documentLayoutTemplate)
                {
                    if (Math.Abs((template.Rectangle.X / (float)inputTemplate.OriginalSize.Width) - sample.X) + 
                        Math.Abs((template.Rectangle.Y / (float)inputTemplate.OriginalSize.Height) - sample.Y) < MaxDistance)
                        result.Add(new TemplateElement(template));
                }
            }

            return result;
        }
    }
}
