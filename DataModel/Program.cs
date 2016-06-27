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

namespace DataModel
{
    class Program
    {
        static void Main(string[] args)
        {
            Bitmap picture = new Bitmap(@"C:\Users\madn1\Documents\Visual Studio 2015\Projects\DataModel\DataModel\Docs Examples\list.jpg");
            picture = PicturePreprocessor.BinaringBitmap(picture);
            PictureProcessor pProc = new PictureProcessor();
            picture.Save(@"C:\Users\madn1\Documents\Visual Studio 2015\Projects\DataModel\DataModel\Docs Examples\0p_Preprocessed.jpg");

            pProc.RAW = picture;
            pProc.buildRegioтImageStatistics();
            pProc.buildRegionMask();
            pProc.buildPartition();

            Console.ReadKey();

        }
    }
}
