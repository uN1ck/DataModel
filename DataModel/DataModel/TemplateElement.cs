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
        public Rectangle Rectangle{ set; get; }

        /// <summary>
        /// Заголовок вершины дерева разбора
        /// </summary>
        public String Name { set; get; }

        /// <summary>
        /// Ссылка на предка в дереве резбора
        /// </summary>
        public TemplateElement Parent { set; get; }

     
        public TemplateElement(Rectangle rect, String name)
        {
            TemplateContainer = new List<TemplateElement>();
            Name = name;
            Rectangle = rect;
        }

        public TemplateElement(Rectangle rect)
        {
            TemplateContainer = new List<TemplateElement>();
            Name = "New simple element";
            Rectangle = rect;
        }

        public TemplateElement()
        {
            TemplateContainer = new List<TemplateElement>();
            Name = "New simple element";
            Rectangle = new Rectangle();
        }

    }
}
