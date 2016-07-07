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
    /// Основа для построения дерева разбора документа
    /// </summary>
    public class TemplateElement
    {
        /// <summary>
        /// Список потомков вершины дерева разбора
        /// </summary>
        public List<TemplateElement> TemplateContainer { set; get; }

        /// <summary>
        /// Коробка-граница региона интереса, являющегося вершиной дерева разбора
        /// </summary>
        public Rectangle Rect{ set; get; }

        /// <summary>
        /// Заголовок вершины дерева разбора
        /// </summary>
        public String Name { set; get; }

        /// <summary>
        /// Массив строковых меток региона
        /// </summary>
        public IList<String> Marks { set; get; }

        /// <summary>
        /// Ссылка на предка в дереве резбора
        /// </summary>
        public TemplateElement Parent { set; get; }

        /// <summary>
        /// Делегат обработки события изменения элемента
        /// </summary>
        /// <param name="templateElementContainer"></param>
        public delegate void TempleateElementChanged(TemplateElement templateElementContainer);
        /// <summary>
        /// Событие изменения элемента
        /// </summary>
        public event TempleateElementChanged TempleateElementChangedHandler;
        /// <summary>
        /// Источник события
        /// </summary>
        /// <param name="selectedTemplateElement"></param>
        public virtual void RaiseTempleateElementChangedEvent(TemplateElement selectedTemplateElement)
        {
            if (TempleateElementChangedHandler != null)
                TempleateElementChangedHandler(this);
        }


        public TemplateElement(Rectangle rect, String name)
        {
            TemplateContainer = new List<TemplateElement>();
            Name = name;
            Rect = rect;
            Marks = new List<String>();
        }

        public TemplateElement(Rectangle rect)
        {
            TemplateContainer = new List<TemplateElement>();
            Name = "New simple element";
            Rect = rect;
            Marks = new List<string>();
        }

        public TemplateElement()
        {
            TemplateContainer = new List<TemplateElement>();
            Name = "New simple element";
            Rect = new Rectangle();
            Marks = new List<String>();
        }

    }
}
