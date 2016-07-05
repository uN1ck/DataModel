using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Enterra.DocumentLayoutAnalysis.Model;
using Microsoft.Win32;
using System.Drawing;
using System.IO;

namespace DocumentLayoutAnalyseView
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Bitmap original;
        private Bitmap processed;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            processed = PreprocessingController.ImageBinarizationFilter.BinarizeImage(original);
            //processed.Save("processed.jpg");
            Image_PrevievDocument.Source = Convert(processed);
        }

        private void Button_OpenOriginalImage_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "All Files|*.*";
            ofd.ShowDialog();

            if (ofd.FileName != "" || ofd.FileName != null)
            {
                original = new Bitmap(ofd.FileName);
                original.Save("original.jpg");
                Image_PrevievDocument.Source = Convert(original);
            }

        }

        private void Button_DetectRegions_Click(object sender, RoutedEventArgs e)
        {
            ProcessingController.ImageProcessor.buildPartition(processed);
            Image_PrevievDocument.Source = Convert(ProcessingController.ImageProcessor.DrawMask(original));
        }

        private BitmapImage Convert(Bitmap inputValue)
        {
            MemoryStream Ms = new MemoryStream();
            Bitmap ObjBitmap = inputValue;
            ObjBitmap.Save(Ms, System.Drawing.Imaging.ImageFormat.Bmp);
            Ms.Position = 0;
            BitmapImage ObjBitmapImage = new BitmapImage();
            ObjBitmapImage.BeginInit();
            ObjBitmapImage.StreamSource = Ms;
            ObjBitmapImage.EndInit();
            return ObjBitmapImage;
        }

       
    }
}
