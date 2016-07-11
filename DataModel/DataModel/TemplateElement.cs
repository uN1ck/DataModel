using System;
using System.Collections.Generic;
using System.Drawing;
using Newtonsoft.Json;

namespace Enterra.DocumentLayoutAnalysis.Model
{
    /// <summary>
    /// Базовый класс для работы с шаблоном документа,
    /// содержит в себе заголовк элемента документа и
    /// контент элемента документа.
    /// </summary>
    public class TemplateElement
    {

        private Rectangle rectangle;
        /// <summary>
        /// Коробка-граница региона интереса, являющегося вершиной дерева разбора
        /// </summary>
        public Rectangle Rectangle
        {
            set
            {
                rectangle = value;
                RaiseTempleateElementChangedEvent();
            }
            get
            {
                return rectangle;
            }
        }

        private String name;
        /// <summary>
        /// Заголовок вершины дерева разбора
        /// </summary>
        public String Name
        {
            set
            {
                name = value;
                RaiseTempleateElementChangedEvent();
            }
            get
            {
                return name;
            }
        }

        private List<String> marks;
        /// <summary>
        /// Массив строковых меток региона
        /// </summary>
        public IList<String> Marks
        {
            set
            {
                marks = value as List<String>;
                RaiseTempleateElementChangedEvent();
            }
            get
            {
                return marks;
            }
        }

        /// <summary>
        /// Делегат обработки события изменения элемента
        /// </summary>
        /// <param name="templateElementContainer">Измененный элемент</param>
        public delegate void TempleateElementChanged(TemplateElement templateElementContainer);
        /// <summary>
        /// Событие изменения элемента
        /// </summary>
        public event TempleateElementChanged TempleateElementChangedHandler;
        /// <summary>
        /// Источник события
        /// </summary>
        protected virtual void RaiseTempleateElementChangedEvent()
        {
            if (TempleateElementChangedHandler != null)
                TempleateElementChangedHandler(this);
        }


        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="rect">Регион</param>
        /// <param name="name">Имя региона</param>
        public TemplateElement(Rectangle rect, String name)
        {
            Name = name;
            Rectangle = rect;
            Marks = new List<String>();
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="rect">Регион</param>
        public TemplateElement(Rectangle rect)
        {
            
            Name = "New simple element";
            Rectangle = rect;
            Marks = new List<string>();
        }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        public TemplateElement()
        {
            
            Name = "New simple element";
            Rectangle = new Rectangle();
            Marks = new List<String>();
        }

        public TemplateElement(TemplateElement templateElement)
        {
            rectangle = templateElement.rectangle;
            name = templateElement.name;
            marks = new List<String>(templateElement.marks);
        }
    }
}
