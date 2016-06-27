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
        public List<SimpleElement> Children { set; get; }

        /// <summary>
        /// Коробка-граница региона интереса, являющегося вершиной дерева разбора
        /// </summary>
        public Rectangle Rect{set; get;}

        /// <summary>
        /// Заголовок вершины дерева разбора
        /// </summary>
        public String Name { set; get; }

        /// <summary>
        /// Идентификатор контента.
        /// Используется для идентификации контента, содержащегося в вершине дерева разбора
        /// </summary>
        private String id;
        public String ID { get { return id; } }

        /// <summary>
        /// Конструктор вершины дерева разбора
        /// </summary>
        /// <param name="ID">Уникальный идентификатор контента</param>
        public SimpleElement(String ID)
        {
            Children = new List<SimpleElement>();
            Name = "New simple element";
            id = ID;
        }

    }
}
