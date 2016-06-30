using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using System.Drawing.Imaging;

using Enterra.DocumentLayoutAnalysis.Model;
using System.Drawing;

namespace Enterra.DocumentLayoutAnalysis.Search
{
    public class PictureProcessor
    {
        /// <summary>
        /// Оригинал картинки для обработки изображения
        /// </summary>
        private Image<Gray, byte> raw;
        /// <summary>
        /// Порог случайного шума между строками
        /// Следует вычислять по ходу дела, но пока не проработан алгоритм
        /// </summary>
        public static double MaximumPossibleNoise = 2;
        /// <summary>
        /// Пропорция отношения выосты строки к высоте документа а4 из расчета,
        /// что используется шрифт высотоый 12pt минимум
        /// </summary>
        public static double TextSizeRatio = 54;


        public PictureProcessor()
        {
            raw = new Image<Gray, byte>(0, 0);
        }

        public PictureProcessor(Bitmap picture)
        {
            raw = new Image<Gray, byte>(picture);
        }
     
        
           
        /// <summary>
        /// Построение горизонтального разбиения регионов картиники, в рамках заданного азюиения
        /// </summary>
        /// <param name="partition">Заданное ранее разбиение</param>
        /// <returns>Новое разбиение</returns>
        private List<TemplateElement> BuildHorisontalPartition( List<TemplateElement> partition, double maximum_possible_noise, double text_size_raito, int begin)
        {

            int size = partition.Count;
            for (int partitionIndex = begin; partitionIndex < size; partitionIndex++)
            {
                Byte[,,] mapped = raw.Data;
                int[] separation = new int[partition[partitionIndex].Rect.Height];

                for (int x = partition[partitionIndex].Rect.X; x < partition[partitionIndex].Rect.X + partition[partitionIndex].Rect.Width; x++)
                    for (int y = partition[partitionIndex].Rect.Y; y < partition[partitionIndex].Rect.Y + partition[partitionIndex].Rect.Height; y++)
                        if (mapped[y, x, 0] == 0)
                            separation[y - partition[partitionIndex].Rect.Y]++;

                List<KeyValuePair<int, int>> whitePartition = new List<KeyValuePair<int, int>>();
                for (int i = partition[partitionIndex].Rect.Y; i < partition[partitionIndex].Rect.Y + partition[partitionIndex].Rect.Height; i++)
                {
                    int len = 0;
                    int start = 0;
                    for (start = i; (i + 1 < partition[partitionIndex].Rect.Y + partition[partitionIndex].Rect.Height) && (Math.Abs(separation[i - partition[partitionIndex].Rect.Y] - separation[i + 1 - partition[partitionIndex].Rect.Y]) < maximum_possible_noise); i++, len++) ;
                    if ((len > (partition[partitionIndex].Rect.Width / text_size_raito)) || (i == partition[partitionIndex].Rect.Y + partition[partitionIndex].Rect.Height - 1))
                        whitePartition.Add(new KeyValuePair<int, int>(start, start + len));

                }

                if (whitePartition.Count > 0)
                    for (int i = 1; i < whitePartition.Count; i++)
                    {
                        TemplateElement currentElement = new TemplateElement(new Rectangle(partition[partitionIndex].Rect.X, whitePartition[i - 1].Value, partition[partitionIndex].Rect.Width, whitePartition[i].Key - whitePartition[i - 1].Value), "HE");
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
                int[] separation = new int[partition[partitionIndex].Rect.Width];

                for (int x = partition[partitionIndex].Rect.X; x < partition[partitionIndex].Rect.X + partition[partitionIndex].Rect.Width; x++)
                    for (int y = partition[partitionIndex].Rect.Y; y < partition[partitionIndex].Rect.Y + partition[partitionIndex].Rect.Height; y++)
                        if (mapped[y, x, 0] == 0)
                            separation[x - partition[partitionIndex].Rect.X]++;

                List<KeyValuePair<int, int>> white_partition = new List<KeyValuePair<int, int>>();
                for (int i = partition[partitionIndex].Rect.X; i < partition[partitionIndex].Rect.X + partition[partitionIndex].Rect.Width; i++)
                {
                    int len = 0;
                    int start = 0;
                    for (start = i; (i + 1 < partition[partitionIndex].Rect.X + partition[partitionIndex].Rect.Width) && (Math.Abs(separation[i - partition[partitionIndex].Rect.X] - separation[i + 1 - partition[partitionIndex].Rect.X]) < maximum_possible_noise); i++, len++) ;
                    if ((len > (partition[partitionIndex].Rect.Width / text_size_raito)) || (i == partition[partitionIndex].Rect.X + partition[partitionIndex].Rect.Width - 1)) 
                        white_partition.Add(new KeyValuePair<int, int>(start, start + len));
                }

                if (white_partition.Count > 0)
                    for (int i = 1; i < white_partition.Count; i++)
                    {
                        TemplateElement currentElement = new TemplateElement(new Rectangle(white_partition[i - 1].Value, partition[partitionIndex].Rect.Y, white_partition[i].Key - white_partition[i - 1].Value, partition[partitionIndex].Rect.Height), "VE");
                        partition[partitionIndex].TemplateContainer.Add(currentElement);
                        partition.Add(currentElement);
                    }
            }
            return partition;
        }


        /// <summary>
        /// Метод построения последовательного разбиения картинки на регионы
        /// Наивная реализация
        /// Применение глубины больше единицы вриводит к ошибкам в разбиении
        /// </summary>
        /// <param name="deepness"> глубина разбиения, по умолчанию равна единице</param>
        public void buildPartition(int deepness = 1)
        {
            List<TemplateElement> partition = new List<TemplateElement>() { new TemplateElement( new Rectangle(0, 0, raw.Cols, raw.Rows), "HEAD") };
            int previousPartitionCount = 0;
            int currentPartitionCount = 1;
            for (int i = 0; i < deepness; i++)
            {
                partition = BuildHorisontalPartition(partition, MaximumPossibleNoise, TextSizeRatio, previousPartitionCount);
                partition = BuildVerticalPartition(partition, MaximumPossibleNoise, TextSizeRatio, currentPartitionCount);

                previousPartitionCount = currentPartitionCount;
                currentPartitionCount = partition.Count;
            }
           
            DrawMask(partition);

        }

        /// <summary>
        /// Метод отрисовки маски на основании разделения
        /// </summary>
        /// <param name="partition">Разделение картинки</param>
        public void DrawMask(List<TemplateElement> partition)
        {

            Bitmap originalImage = new Bitmap(raw.ToBitmap());
            Graphics graphics = Graphics.FromImage(originalImage);
            int i = 0;
            foreach (TemplateElement rect in partition)
            {
                //graphics.DrawString(rect.Name, new Font(FontFamily.GenericSerif, 10), Brushes.Chocolate, rect.Rect.X, rect.Rect.Y);
                graphics.DrawRectangle(Pens.Red, rect.Rect);
                buildRegionImageStatistics(new Image<Bgr, Byte>(cropImage(originalImage, rect.Rect)),"crop_"+i);//.Save(@"C:\Users\madn1\Documents\Visual Studio 2015\Projects\DataModel\DataModel\Docs Examples\Crops\sep_" + i + ".jpg");
                i++;
            }
            originalImage.Save(@"C:\Users\madn1\Documents\Visual Studio 2015\Projects\DataModel\DataModel\Docs Examples\Crops\separation.jpg");
        }

        /// <summary>
        /// Метод сбора картинки, показывающей частоту встречаемости черного пикселя в строке и в столбце
        /// используется для сбора данных, в итоговой программе не нужен, либо будет переписан
        /// </summary>
        public static void buildRegionImageStatistics(Image<Bgr, byte> raw, string name)
        {
            List<TemplateElement> result = new List<TemplateElement>();
            Byte[,,] mapped = raw.Data;
            int[] left = new int[raw.Cols];
            int[] bot = new int[raw.Rows];

            for (int x = 0; x < raw.Rows; x++)
                for (int y = 0; y<raw.Cols; y++)   
                    if (mapped[x,y,0] == 0)
                    {
                        left[y]++;
                        bot[x]++;
                    }
   
            Bitmap rest = new Bitmap(raw.Width + bot.Max() + 10, raw.Height + left.Max() + 10);
            Graphics graphics = Graphics.FromImage(rest);
            graphics.DrawImage(raw.ToBitmap(), new Point(bot.Max() + 10, left.Max() + 10));

            for (int i = 0; i < left.Length; i++)
                graphics.DrawLine(Pens.DarkGray, new Point(bot.Max() + 10 + i, left[i] + 1), new Point(bot.Max() + 10 + i, left[i]));

            for (int i = 0; i < bot.Length; i++)
                graphics.DrawLine(Pens.DarkGray, new Point(bot[i] + 1, left.Max() + 10 + i), new Point(bot[i], left.Max() + 10 + i));

     
            rest.Save(@"C:\Users\madn1\Documents\Visual Studio 2015\Projects\DataModel\DataModel\Docs Examples\Crops\"+name+".jpg");

        }

        private static Bitmap cropImage(Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }

    }
}
