using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enterra.DocumentLayoutAnalysis.Model
{
    public class TemplateElementMananger
    {
        private List<TemplateElement> templateElementContainer;
        public IReadOnlyList<TemplateElement> TemplateElementContainer { get { return templateElementContainer; } }
        public TemplateElement this[int index] {
            set {
                templateElementContainer[index] = value;
                templateElementContainer[index].TempleateElementChangedHandler += RaiseTempleateElementsContainerChangedEvent;
            }
            get {
                return templateElementContainer[index];
            }
        }

        public void Add(TemplateElement inputTemplateElement)
        {
            templateElementContainer.Add(inputTemplateElement);
            inputTemplateElement.TempleateElementChangedHandler += RaiseTempleateElementsContainerChangedEvent;
        }

        public void AddAll(List<TemplateElement> inputTemplateElementsList)
        {
            foreach (TemplateElement current in inputTemplateElementsList)
                Add(current);
        }

        

        /// <summary>
        /// Делегат обработки события изменения контейнера
        /// </summary>
        /// <param name="templateElementContainer"></param>
        public delegate void TempleateElementsContainerChanged(List<TemplateElement> templateElementContainer);
        /// <summary>
        /// Событие изменения контейнера
        /// </summary>
        public event TempleateElementsContainerChanged TempleateElementsChangedHandler;
        /// <summary>
        /// Источник события
        /// </summary>
        /// <param name="selectedTemplateElement"></param>
        public virtual void RaiseTempleateElementsContainerChangedEvent(TemplateElement selectedTemplateElement)
        {
            if (TempleateElementsChangedHandler != null)
                TempleateElementsChangedHandler(templateElementContainer);
        }


        public TemplateElementMananger()
        {
            templateElementContainer = new List<TemplateElement>();
        }

        public TemplateElementMananger(List<TemplateElement> inputList)
        {
            foreach (TemplateElement current in inputList)
                current.TempleateElementChangedHandler += RaiseTempleateElementsContainerChangedEvent;
            templateElementContainer = inputList;
        }

        /// <summary>
        /// Метод доступа к региону по координате точки содержащейся в нем
        /// </summary>
        /// <param name="point">Точка по которой проводится поиск региона</param>
        /// <returns>Регион, если был обнаружен, иначе null</returns>
        public TemplateElement getElementAtPoint(Point point)
        {
            foreach (TemplateElement current in templateElementContainer)
            {
                if (current.Rect.Contains(point))
                    return current;
            }
            return null;
        }

    }
}
