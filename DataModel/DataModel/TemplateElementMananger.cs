using System.Collections.Generic;
using System.Drawing;
using ZedGraph;

namespace Enterra.DocumentLayoutAnalysis.Model
{
    /// <summary>
    /// Класс доукумента, разложенного на регионы в абсолютных
    /// </summary>
    public class TemplateElementMananger
    {
        /// <summary>
        /// Разбиение документа на регионы, размеры которого заданы относительно оригинального размера документа 
        /// </summary>
        private List<TemplateElement> templateElementContainer;
        /// <summary>
        /// Интерфейс разбиеня на регионы
        /// Возвращает неизменяемые данные
        /// </summary>
        public IReadOnlyList<TemplateElement> TemplateElementContainer { get { return templateElementContainer; } }
        /// <summary>
        /// Доступ к элементу разбиения документа по индексу
        /// </summary>
        /// <param name="index">Индекс элемента</param>
        /// <returns>Элемент разбиения</returns>
        public TemplateElement this[int index] {
            set {
                templateElementContainer[index] = value;
                templateElementContainer[index].TempleateElementChangedHandler += OnTemplateElementChange;
                RaiseTempleateElementsContainerChange();
            }
            get {
                return templateElementContainer[index];
            }
        }
        /// <summary>
        /// Метод добавлеия нового региона разбиения
        /// </summary>
        /// <param name="inputTemplateElement">Регион разбиения</param>
        public void Add(TemplateElement inputTemplateElement)
        {
            templateElementContainer.Add(inputTemplateElement);
            inputTemplateElement.TempleateElementChangedHandler += OnTemplateElementChange;
        }
        /// <summary>
        /// Метод добавления многих регионов разбиения
        /// </summary>
        /// <param name="inputTemplateElementsList">Список регионов разбиения</param>
        public void Add(List<TemplateElement> inputTemplateElementsList)
        {
            foreach (TemplateElement current in inputTemplateElementsList)
                Add(current);
        }


        private Size originalSize;
        /// <summary>
        /// Размер оригинала документа
        /// </summary>
        public Size OriginalSize
        {
            set
            {
                originalSize = value;
                RaiseTempleateElementsContainerChange();
            }
            get
            {
                return originalSize;
            }
        }


        /// <summary>
        /// Делегат обработки события изменения контейнера
        /// </summary>
        /// <param name="templateElementContainer"></param>
        public delegate void TempleateElementsContainerChanged(TemplateElementMananger templateElementContainer);
        /// <summary>
        /// Событие изменения контейнера
        /// </summary>
        public event TempleateElementsContainerChanged TempleateElementsChangedHandler;
        /// <summary>
        /// Метот возбужденяи события
        /// </summary>
        /// <param name="selectedTemplateElement"></param>
        protected virtual void RaiseTempleateElementsContainerChange()
        {
            if (TempleateElementsChangedHandler != null)
                TempleateElementsChangedHandler(this);
        }
        /// <summary>
        /// Обработчик события изменения региона
        /// </summary>
        private void OnTemplateElementChange(TemplateElement templateElement)
        {
            RaiseTempleateElementsContainerChange();
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public TemplateElementMananger()
        {
            templateElementContainer = new List<TemplateElement>();
        }

        /// <summary>
        /// Констурктор класса
        /// </summary>
        /// <param name="inputList">Входнйо набор разбитых регионов</param>
        public TemplateElementMananger(List<TemplateElement> inputList)
        {
            templateElementContainer = new List<TemplateElement>();
            Add(inputList);
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
                if (current.Rectangle.Contains(point))
                    return current;
            }
            return null;
        }

    }
}
