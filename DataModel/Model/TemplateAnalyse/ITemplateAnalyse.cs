using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enterra.DocumentLayoutAnalysis.Model
{
    public interface ITemplateAnalyse
    {
        /// <summary>
        /// Метод построения примера разбиения на основе анализа разбиений сходного фида документов
        /// </summary>
        /// <param name="inputTemplateList"> Список разбитых регионов </param>
        void BuildRegionsSample(List<TemplateElementMananger> inputTemplateList);

        /// <summary>
        /// Метод фильтрации региона на основе построенного анализом примера
        /// </summary>
        /// <param name="inputTemplate">Входное разбиение региона</param>
        /// <returns>Фильтрованный набор регионов</returns>
        TemplateElementMananger FilterRegions(TemplateElementMananger inputTemplate);

    }
}
