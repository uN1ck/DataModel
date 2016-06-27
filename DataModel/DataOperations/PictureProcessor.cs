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
        public static int MAXIMUM_POSSIBLE_NOISE = 5;
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
        public List<Rectangle> BuildHorisontalPartition( List<Rectangle> partition, int maximum_possible_noise, double text_size_raito)
        {
            List<Rectangle> newPartition = new List<Rectangle>();
           
            foreach (Rectangle part in partition)
            {
                Byte[,,] mapped = raw.Data;
                int[] separation = new int[part.Height];

                for (int x = part.X; x < part.X+part.Width; x++)
                    for (int y = part.Y; y < part.Y+part.Height; y++)
                        if (mapped[y, x, 0] == 0)
                            separation[y]++;

                List<KeyValuePair<int, int>> white_partition = new List<KeyValuePair<int, int>>();
                for (int i = part.Y; i < part.Y + part.Height; i++)
                {
                    int len = 0;
                    int start = 0;
                    for (start = i; (i < part.Y + part.Height) && (separation[i] < maximum_possible_noise); i++, len++) ;
                    if ((len > (raw.Width / text_size_raito)) || (i == part.Y + part.Height - 1))
                        white_partition.Add(new KeyValuePair<int, int>(start, start + len));

                }

                if (white_partition.Count > 0)
                    for (int i = 1; i < white_partition.Count; i++)
                        newPartition.Add(new Rectangle(part.X, white_partition[i - 1].Value, part.Width, white_partition[i].Key - white_partition[i - 1].Value));
                else
                    newPartition.Add(part);
            }
            
            return newPartition;
        }


        /// <summary>
        /// Построение вертикального разбиения регионов картиники, в рамках заданного азюиения
        /// </summary>
        /// <param name="partition">Заданное ранее разбиение</param>
        /// <returns>Новое разбиение</returns>
        public List<Rectangle> BuildVerticalPartition(List<Rectangle> partition, int maximum_possible_noise, double text_size_raito)
        {
            List<Rectangle> newPartition = new List<Rectangle>();

            foreach (Rectangle part in partition)
            {
                Byte[,,] mapped = raw.Data;
                int[] separation = new int[part.Width];

                for (int x = part.X; x < part.X + part.Width; x++)
                    for (int y = part.Y; y < part.Y + part.Height; y++)
                        if (mapped[y, x, 0] == 0)
                            separation[x]++;

                List<KeyValuePair<int, int>> white_partition = new List<KeyValuePair<int, int>>();
                for (int i = part.X; i < part.X + part.Width; i++)
                {
                    int len = 0;
                    int start = 0;
                    for (start = i; (i < part.X + part.Width) && (separation[i] < maximum_possible_noise); i++, len++) ;
                    if ((len > (raw.Width / text_size_raito)) || (i == part.X + part.Width - 1)) 
                        white_partition.Add(new KeyValuePair<int, int>(start, start + len));
                }

                if (white_partition.Count > 0)
                    for (int i = 1; i < white_partition.Count; i++)
                        newPartition.Add(new Rectangle(white_partition[i - 1].Value, part.Y, white_partition[i].Key - white_partition[i - 1].Value, part.Height));
                else
                    newPartition.Add(part);
            }
            return newPartition;
        }


        /// <summary>
        /// Метод построения последовательного разбиения картинки на регионы
        /// Наивная реализация
        /// </summary>
        public void buildPartition()
        {
            List<Rectangle> partition = new List<Rectangle>() { new Rectangle(0, 0, raw.Cols, raw.Rows) };
            partition = BuildHorisontalPartition(partition, MAXIMUM_POSSIBLE_NOISE, TEXT_SIZE_RAITO);
            partition = BuildVerticalPartition(partition, MAXIMUM_POSSIBLE_NOISE, TEXT_SIZE_RAITO);
            DrawMask(partition);
        }

        /// <summary>
        /// Метод отрисовки маски на основании разделения
        /// </summary>
        /// <param name="partition">Разделение картинки</param>
        public void DrawMask(List<Rectangle> partition)
        {

            Bitmap rest = new Bitmap(raw.Width,raw.Height);
            Graphics graphics = Graphics.FromImage(rest);
            foreach (Rectangle rect in partition)
            {
                graphics.DrawRectangle(Pens.Red, rect);
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
