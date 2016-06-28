using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel.DataOperations;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using System.Drawing.Imaging;
using System.Drawing;

using System.Diagnostics;

namespace DataModel
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch t = Stopwatch.StartNew();

            Bitmap picture = new Bitmap(@"C:\Users\madn1\Documents\Visual Studio 2015\Projects\DataModel\DataModel\Docs Examples\snils.jpg");
            picture = PicturePreprocessor.BinaringBitmap(picture);
            PictureProcessor pProc = new PictureProcessor();
            picture.Save(@"C:\Users\madn1\Documents\Visual Studio 2015\Projects\DataModel\DataModel\Docs Examples\0p_Preprocessed.jpg");

            pProc.RAW = picture;
            pProc.buildRegioтImageStatistics();
            pProc.buildRegionMask();
            pProc.buildPartition();

            t.Stop();
            Console.WriteLine(t.Elapsed);
            Console.WriteLine(t.ElapsedMilliseconds);
            Console.WriteLine(t.ElapsedTicks);
            Console.ReadKey();
            
        }
    }
}
