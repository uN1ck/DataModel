using System;

using System.Drawing;

using System.Diagnostics;

using Enterra.DocumentLayoutAnalysis.Model;

namespace DataModel
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            Stopwatch t = Stopwatch.StartNew();

            Bitmap picture = new Bitmap(@"C:\Users\madn1\Documents\Visual Studio 2015\Projects\DataModel\DataModel\Docs Examples\snils.jpg");
            IImageBinarizationFilter binarizationFilter = new DilatedImageFilter();
            picture = binarizationFilter.BinarizeImage(picture);

            picture.Save(@"C:\Users\madn1\Documents\Visual Studio 2015\Projects\DataModel\DataModel\Docs Examples\Crops\preprocessed.jpg");
            ImageProcessor imageProcessor = new CannyContourImageProcessor(400);
            imageProcessor.buildPartition(picture);
            //imageProcessor.DrawMask().Save(@"C:\Users\madn1\Documents\Visual Studio 2015\Projects\DataModel\DataModel\Docs Examples\Crops\processed.jpg");

            t.Stop();
            Console.WriteLine(t.Elapsed);
            Console.WriteLine(t.ElapsedMilliseconds);
            Console.WriteLine(t.ElapsedTicks);
            Console.ReadKey();
            */
        }
    }
}
