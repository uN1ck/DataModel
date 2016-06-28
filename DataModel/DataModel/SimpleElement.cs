using System;
using System.Collections.Generic;
using System.Drawing;
using Newtonsoft.Json;

namespace DataModel.DataModel
{
    /// <summary>
    /// Базовый класс для работы с шаблоном документа,
    /// содержит в себе заголовк элемента документа и
    /// контент элемента документа.
    /// Основа для построения дерева разбора документа
    /// </summary>
    class SimpleElement
    {
        /// <summary>
        /// Список потомков вершины дерева разбора
        /// </summary>
        public List<int> Children { set; get; }

        /// <summary>
        /// Коробка-граница региона интереса, являющегося вершиной дерева разбора
        /// </summary>
        public Rectangle Rect{set; get;}

        /// <summary>
        /// Заголовок вершины дерева разбора
        /// </summary>
        public String Name { set; get; }

        /// <summary>
        /// Ссылка на предка в дереве резбора
        /// </summary>
        public int Parent { set; get; }

     
        public SimpleElement(Rectangle rect, String name)
        {
            Children = new List<int>();
            Name = name;
            Rect = rect;
        }

        public SimpleElement(Rectangle rect)
        {
            Children = new List<int>();
            Name = "New simple element";
            Rect = rect;
        }

        public SimpleElement()
        {
            Children = new List<int>();
            Name = "New simple element";
            Rect = new Rectangle();
        }

    }
}
