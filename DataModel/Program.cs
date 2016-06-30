using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using System.Drawing.Imaging;
using System.Drawing;

using System.Diagnostics;

using Enterra.DocumentLayoutAnalysis.Model;
using Enterra.DocumentLayoutAnalysis.Search;

namespace DataModel
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch t = Stopwatch.StartNew();

            Bitmap picture = new Bitmap(@"C:\Users\madn1\Documents\Visual Studio 2015\Projects\DataModel\DataModel\Docs Examples\INN.jpg");
            picture = PicturePreprocessor.BinaringBitmap(picture);
            PictureProcessor pictureProcessor = new PictureProcessor();
            picture.Save(@"C:\Users\madn1\Documents\Visual Studio 2015\Projects\DataModel\DataModel\Docs Examples\0p_Preprocessed.jpg");

            pictureProcessor.RAW = picture;
            pictureProcessor.buildRegionMask();
            pictureProcessor.buildPartition();

            t.Stop();
            Console.WriteLine(t.Elapsed);
            Console.WriteLine(t.ElapsedMilliseconds);
            Console.WriteLine(t.ElapsedTicks);
            Console.ReadKey();
            
        }
    }
}
