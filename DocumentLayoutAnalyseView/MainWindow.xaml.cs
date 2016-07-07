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

using Microsoft.Win32;
using System.Drawing;
using System.IO;

using Enterra.DocumentLayoutAnalysis.Model;

namespace DocumentLayoutAnalyseView
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TemlateElementComposer templateElementComposer;

        public MainWindow()
        {
            InitializeComponent();
            templateElementComposer = new TemlateElementComposer();
            templateElementComposer.ImageBinarizationFilter = PreprocessingController.ImageBinarizationFilter;
            templateElementComposer.ImageProcessor = ProcessingController.ImageProcessor;
            templateElementComposer.RedrawElemetsHandler += onRedrawElemets;
        }

        protected void onRedrawElemets(TemlateElementComposer sender)
        {
            this.templateElementComposer = sender;
            drawImages();
            Image_OriginalDocument.Source = Convert(templateElementComposer.Masked);
        }

        private void Button_ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            templateElementComposer.PreprocessImage();
            drawImages();
        }

        private void Button_DetectRegions_Click(object sender, RoutedEventArgs e)
        {
            templateElementComposer.ProcessImage();
            drawImages();
            Image_OriginalDocument.Source = Convert(templateElementComposer.Masked);
        }


        private void Button_OpenOriginalImage_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "All Files|*.*";
            ofd.ShowDialog();

            if (ofd.FileName != "" || ofd.FileName != null)
            {
                templateElementComposer.Original = new Bitmap(ofd.FileName);
                drawImages();
            }
        }

        private void drawImages()
        {
            Image_OriginalDocument.Source = Convert(templateElementComposer.Original);
            Image_ProcessedDocument.Source = Convert(templateElementComposer.Preprocessed);
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

        private void Image_OriginalDocument_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            double property = templateElementComposer.Original.Width / ((double)Image_OriginalDocument.ActualWidth);
            try
            {
                System.Drawing.Point cursorPosition = new System.Drawing.Point((int)(e.GetPosition(Image_OriginalDocument as IInputElement).X * property), (int)(e.GetPosition(Image_OriginalDocument as IInputElement).Y * property));
                TemplateElementController.setSelectedTemplateElement(
                templateElementComposer.TemplateElementMananger.getElementAtPoint(cursorPosition));
            } catch (NullReferenceException ex)
            {

            }
        }
    }
}
