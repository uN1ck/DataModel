using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using System.Drawing.Imaging;

using DataModel.DataModel;
using System.Drawing;

namespace DataModel.DataOperations
{
    class PictureProcessor
    {
        private Image<Gray, byte> raw;
        public Bitmap RAW { set { raw = new Image<Gray, byte>(value); } get { return raw.ToBitmap(); } }
        private Random randomID;
        

        /// <summary>
        /// Порог случайного шума в строке
        /// Следует вычислять по ходу дела, но пока не проработан алгоритм
        /// </summary>
        public static int MAXIMUM_POSSIBLE_NOISE = 2;
        /// <summary>
        /// Пропорция отношения выосты строки к высоте документа а4 из расчета,
        /// что используется шрифт высотоый 12pt минимум
        /// </summary>
        public static double TEXT_SIZE_RAITO = 52.6;


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
        protected List<SimpleElement> BuildHorisontalPartition( List<SimpleElement> partition, int maximum_possible_noise, double text_size_raito, int begin)
        {

            int size = partition.Count;
            for (int pt = begin; pt < size; pt++)
            {
                Byte[,,] mapped = raw.Data;
                int[] separation = new int[partition[pt].Rect.Height];

                for (int x = partition[pt].Rect.X; x < partition[pt].Rect.X + partition[pt].Rect.Width; x++)
                    for (int y = partition[pt].Rect.Y; y < partition[pt].Rect.Y + partition[pt].Rect.Height; y++)
                        if (mapped[y, x, 0] == 0)
                            separation[y - partition[pt].Rect.Y]++;

                List<KeyValuePair<int, int>> white_partition = new List<KeyValuePair<int, int>>();
                for (int i = partition[pt].Rect.Y; i < partition[pt].Rect.Y + partition[pt].Rect.Height; i++)
                {
                    int len = 0;
                    int start = 0;
                    for (start = i; (i + 1 < partition[pt].Rect.Y + partition[pt].Rect.Height) && (Math.Abs(separation[i - partition[pt].Rect.Y] - separation[i + 1 - partition[pt].Rect.Y]) < maximum_possible_noise); i++, len++) ;
                    if ((len > (raw.Width / text_size_raito)) || (i == partition[pt].Rect.Y + partition[pt].Rect.Height - 1))
                        white_partition.Add(new KeyValuePair<int, int>(start, start + len));

                }

                if (white_partition.Count > 0)
                    for (int i = 1; i < white_partition.Count; i++)
                    {
                        partition[pt].Children.Add(partition.Count);
                        partition.Add(new SimpleElement(new Rectangle(partition[pt].Rect.X, white_partition[i - 1].Value, partition[pt].Rect.Width, white_partition[i].Key - white_partition[i - 1].Value), "HE"));
                    }

            }
            
            return partition;
        }

        /// <summary>
        /// Построение вертикального разбиения регионов картиники, в рамках заданного азюиения
        /// </summary>
        /// <param name="partition">Заданное ранее разбиение</param>
        /// <returns>Новое разбиение</returns>
        protected List<SimpleElement> BuildVerticalPartition(List<SimpleElement> partition, int maximum_possible_noise, double text_size_raito, int begin)
        {
            
            int size = partition.Count;
            for (int pt = begin; pt < size; pt++)
            {
                Byte[,,] mapped = raw.Data;
                int[] separation = new int[partition[pt].Rect.Width];

                for (int x = partition[pt].Rect.X; x < partition[pt].Rect.X + partition[pt].Rect.Width; x++)
                    for (int y = partition[pt].Rect.Y; y < partition[pt].Rect.Y + partition[pt].Rect.Height; y++)
                        if (mapped[y, x, 0] == 0)
                            separation[x - partition[pt].Rect.X]++;

                List<KeyValuePair<int, int>> white_partition = new List<KeyValuePair<int, int>>();
                for (int i = partition[pt].Rect.X; i < partition[pt].Rect.X + partition[pt].Rect.Width; i++)
                {
                    int len = 0;
                    int start = 0;
                    for (start = i; (i + 1 < partition[pt].Rect.X + partition[pt].Rect.Width) && (Math.Abs(separation[i - partition[pt].Rect.X] - separation[i + 1 - partition[pt].Rect.X]) < maximum_possible_noise); i++, len++) ;
                    if ((len > (raw.Width / text_size_raito)) || (i == partition[pt].Rect.X + partition[pt].Rect.Width - 1)) 
                        white_partition.Add(new KeyValuePair<int, int>(start, start + len));
                }

                if (white_partition.Count > 0)
                    for (int i = 1; i < white_partition.Count; i++)
                    {
                        partition[pt].Children.Add(partition.Count);
                        partition.Add(new SimpleElement(new Rectangle(white_partition[i - 1].Value, partition[pt].Rect.Y, white_partition[i].Key - white_partition[i - 1].Value, partition[pt].Rect.Height),"VE"));
                    }
            }
            return partition;
        }
        


        /// <summary>
        /// Метод построения последовательного разбиения картинки на регионы
        /// Наивная реализация
        /// </summary>
        public void buildPartition()
        {
            List<SimpleElement> partition = new List<SimpleElement>() { new SimpleElement( new Rectangle(0, 0, raw.Cols, raw.Rows), "HEAD") };
            int prev = 0;
            int curr = 1;
            for (int i = 0; i < 4; i++)
            {
                partition = BuildHorisontalPartition(partition, MAXIMUM_POSSIBLE_NOISE, TEXT_SIZE_RAITO, prev);
                partition = BuildVerticalPartition(partition, MAXIMUM_POSSIBLE_NOISE, TEXT_SIZE_RAITO, curr);
                DrawMask(partition);
                prev = curr;
                curr = partition.Count;
            }

        }

        /// <summary>
        /// Метод отрисовки маски на основании разделения
        /// </summary>
        /// <param name="partition">Разделение картинки</param>
        public void DrawMask(List<SimpleElement> partition)
        {

            Bitmap rest = new Bitmap(raw.Width,raw.Height);
            Graphics graphics = Graphics.FromImage(rest);
            foreach (SimpleElement rect in partition)
            {
                graphics.DrawString(rect.Name, new Font(FontFamily.GenericSerif, 10), Brushes.Chocolate, rect.Rect.X, rect.Rect.Y);
                graphics.DrawRectangle(Pens.Red, rect.Rect);
            }
            rest.Save(@"C:\Users\madn1\Documents\Visual Studio 2015\Projects\DataModel\DataModel\Docs Examples\separation.jpg");
        }


        /// <summary>
        /// Метод сбора картинки, показывающей частоту встречаемости черного пикселя в строке и в столбце
        /// используется для сбора данных, в итоговой программе не нужен, либо будет переписан
        /// </summary>
        public void buildRegioтImageStatistics()
        {
            List<SimpleElement> result = new List<SimpleElement>();
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
            graphics.DrawImage(RAW, new Point(bot.Max() + 10, left.Max() + 10));

            for (int i = 0; i < left.Length; i++)
                graphics.DrawLine(Pens.DarkGray, new Point(bot.Max() + 10 + i, left[i] + 1), new Point(bot.Max() + 10 + i, left[i]));

            for (int i = 0; i < bot.Length; i++)
                graphics.DrawLine(Pens.DarkGray, new Point(bot[i] + 1, left.Max() + 10 + i), new Point(bot[i], left.Max() + 10 + i));

     
            rest.Save(@"C:\Users\madn1\Documents\Visual Studio 2015\Projects\DataModel\DataModel\Docs Examples\analyse.jpg");

        }


        public void buildRegionMask()
        {
            Byte[,,] mapped = raw.Data;
            int[] left = new int[raw.Rows];

            for (int x = 0; x < raw.Rows; x++)
                for (int y = 0; y < raw.Cols; y++)
                    if (mapped[x, y, 0] == 0)
                    {
                        left[x]++;
                    }

            Bitmap rest = new Bitmap(RAW);
            Graphics graphics = Graphics.FromImage(rest);

            int start = 0;
            int end = start;
            int pe = 0;
            bool detected = false;

            for (int i = 0; i < left.Length; i++)
            {
                if ((left[i] < MAXIMUM_POSSIBLE_NOISE) && (!detected))
                {
                    detected = true;
                    start = i;
                }
                if ((left[i] < MAXIMUM_POSSIBLE_NOISE) && (detected))
                {
                    end = i;
                }
                if (((left[i] >= MAXIMUM_POSSIBLE_NOISE) && (detected)) || (i == left.Length - 1))
                {
                    end = i;
                    detected = false;
                    if ((end - start) > (raw.Width / TEXT_SIZE_RAITO) || (i == left.Length - 1))
                    {

                        graphics.FillRectangle(Brushes.DarkGray, new Rectangle(0, start, 20, end - start));
                        graphics.FillRectangle(Brushes.LightGray, new Rectangle(20, pe, 20, start - pe));
                        Console.WriteLine(pe + " " + (start - pe));
                        pe = end;
                    }
                    
                }
            }
            rest.Save(@"C:\Users\madn1\Documents\Visual Studio 2015\Projects\DataModel\DataModel\Docs Examples\mask.jpg");
        }
    }
}
