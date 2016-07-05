using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enterra.DocumentLayoutAnalysis.Model
{
    public class TopDownImageProcessor : ImageProcessor
    {
        private double maximumPossibleNoise;
        /// <summary>
        /// Порог случайного шума между строками
        /// Следует вычислять по ходу дела, но пока не проработан алгоритм
        /// </summary>
        public double MaximumPossibleNoise { set { maximumPossibleNoise = value; } get { return maximumPossibleNoise; } }

        private double textSizeRatio;
        /// <summary>
        /// Пропорция отношения выосты строки к высоте документа а4 из расчета,
        /// что используется шрифт высотоый 12pt минимум
        /// </summary>
        public double TextSizeRatio { set { textSizeRatio = value; } get { return textSizeRatio; } }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="inputImage">Входное изображение</param>
        /// <param name="textSizeRatio"></param>
        /// <param name="maximumPossibleNoise"></param>
        public TopDownImageProcessor(Bitmap inputImage, double textSizeRatio = 54, double maximumPossibleNoise = 2)
        {
            raw = new Image<Gray, byte>(inputImage);
            TextSizeRatio = textSizeRatio;
            MaximumPossibleNoise = maximumPossibleNoise;
        }

        /// <summary>
        /// Построение горизонтального разбиения регионов картиники, в рамках заданного азюиения
        /// </summary>
        /// <param name="partition">Заданное ранее разбиение</param>
        /// <returns>Новое разбиение</returns>
        private List<TemplateElement> BuildHorisontalPartition(List<TemplateElement> partition, double maximum_possible_noise, double text_size_raito, int begin)
        {

            int size = partition.Count;
            for (int partitionIndex = begin; partitionIndex < size; partitionIndex++)
            {
                Byte[,,] mapped = raw.Data;
                int[] separation = new int[partition[partitionIndex].Rectangle.Height];

                for (int x = partition[partitionIndex].Rectangle.X; x < partition[partitionIndex].Rectangle.X + partition[partitionIndex].Rectangle.Width; x++)
                    for (int y = partition[partitionIndex].Rectangle.Y; y < partition[partitionIndex].Rectangle.Y + partition[partitionIndex].Rectangle.Height; y++)
                        if (mapped[y, x, 0] == 0)
                            separation[y - partition[partitionIndex].Rectangle.Y]++;

                List<KeyValuePair<int, int>> whitePartition = new List<KeyValuePair<int, int>>();
                for (int i = partition[partitionIndex].Rectangle.Y; i < partition[partitionIndex].Rectangle.Y + partition[partitionIndex].Rectangle.Height; i++)
                {
                    int len = 0;
                    int start = 0;
                    for (start = i; (i + 1 < partition[partitionIndex].Rectangle.Y + partition[partitionIndex].Rectangle.Height) && (Math.Abs(separation[i - partition[partitionIndex].Rectangle.Y] - separation[i + 1 - partition[partitionIndex].Rectangle.Y]) < maximum_possible_noise); i++, len++) ;
                    if ((len > (partition[partitionIndex].Rectangle.Width / text_size_raito)) || (i == partition[partitionIndex].Rectangle.Y + partition[partitionIndex].Rectangle.Height - 1))
                        whitePartition.Add(new KeyValuePair<int, int>(start, start + len));

                }

                if (whitePartition.Count > 0)
                    for (int i = 1; i < whitePartition.Count; i++)
                    {
                        TemplateElement currentElement = new TemplateElement(new Rectangle(partition[partitionIndex].Rectangle.X, whitePartition[i - 1].Value, partition[partitionIndex].Rectangle.Width, whitePartition[i].Key - whitePartition[i - 1].Value), "HE");
                        partition[partitionIndex].TemplateContainer.Add(currentElement);
                        partition.Add(currentElement);
                    }

            }

            return partition;
        }

        /// <summary>
        /// Построение вертикального разбиения регионов картиники, в рамках заданного азюиения
        /// </summary>
        /// <param name="partition">Заданное ранее разбиение</param>
        /// <returns>Новое разбиение</returns>
        private List<TemplateElement> BuildVerticalPartition(List<TemplateElement> partition, double maximum_possible_noise, double text_size_raito, int begin)
        {

            int size = partition.Count;
            for (int partitionIndex = begin; partitionIndex < size; partitionIndex++)
            {
                Byte[,,] mapped = raw.Data;
                int[] separation = new int[partition[partitionIndex].Rectangle.Width];

                for (int x = partition[partitionIndex].Rectangle.X; x < partition[partitionIndex].Rectangle.X + partition[partitionIndex].Rectangle.Width; x++)
                    for (int y = partition[partitionIndex].Rectangle.Y; y < partition[partitionIndex].Rectangle.Y + partition[partitionIndex].Rectangle.Height; y++)
                        if (mapped[y, x, 0] == 0)
                            separation[x - partition[partitionIndex].Rectangle.X]++;

                List<KeyValuePair<int, int>> white_partition = new List<KeyValuePair<int, int>>();
                for (int i = partition[partitionIndex].Rectangle.X; i < partition[partitionIndex].Rectangle.X + partition[partitionIndex].Rectangle.Width; i++)
                {
                    int len = 0;
                    int start = 0;
                    for (start = i; (i + 1 < partition[partitionIndex].Rectangle.X + partition[partitionIndex].Rectangle.Width) && (Math.Abs(separation[i - partition[partitionIndex].Rectangle.X] - separation[i + 1 - partition[partitionIndex].Rectangle.X]) < maximum_possible_noise); i++, len++) ;
                    if ((len > (partition[partitionIndex].Rectangle.Width / text_size_raito)) || (i == partition[partitionIndex].Rectangle.X + partition[partitionIndex].Rectangle.Width - 1))
                        white_partition.Add(new KeyValuePair<int, int>(start, start + len));
                }

                if (white_partition.Count > 0)
                    for (int i = 1; i < white_partition.Count; i++)
                    {
                        TemplateElement currentElement = new TemplateElement(new Rectangle(white_partition[i - 1].Value, partition[partitionIndex].Rectangle.Y, white_partition[i].Key - white_partition[i - 1].Value, partition[partitionIndex].Rectangle.Height), "VE");
                        partition[partitionIndex].TemplateContainer.Add(currentElement);
                        partition.Add(currentElement);
                    }
            }
            return partition;
        }

        public override IList<TemplateElement> buildPartition(Bitmap inputImage)
        {
            throw new NotImplementedException();
        }
    }
}
