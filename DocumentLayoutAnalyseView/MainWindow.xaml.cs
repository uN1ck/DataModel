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
using Newtonsoft.Json;

namespace DocumentLayoutAnalyseView
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TemlateElementComposer TemplateElementComposer;

        public MainWindow()
        {
            InitializeComponent();
            
            this.DataContext = this;
            Init();
        }

        /// <summary>
        /// Метод инициализации
        /// </summary>
        private void Init()
        {
            TemplateElementComposer = new TemlateElementComposer();
            CustomPreprocessingController.DilatedImageFilter = new DilatedImageFilter();
            CustomProcessingController.CannyContourImageProcessor = new CannyContourImageProcessor();
            CustomTempalteElementSampleController.TemplateElementSampleMananger = new TemplateElementSampleMananger();

            TemplateElementComposer.ImageBinarizationFilter = CustomPreprocessingController.ImageBinarizationFilter;
            TemplateElementComposer.ImageProcessor = CustomProcessingController.ImageProcessor;
            TemplateElementComposer.TemplateAnalyse = CustomTempalteElementSampleController.TemplateAnalyse;
            TemplateElementComposer.Original = new Bitmap(1, 1);

            TemplateElementComposer.RedrawElemetsHandler += onRedrawElemets;
        }

        protected void onRedrawElemets(TemlateElementComposer sender)
        {
            ImageOriginalDocument.Source = Convert(TemplateElementComposer.Original);
            ImageProcessedDocument.Source = Convert(TemplateElementComposer.Preprocessed);
            ImageMaskedDocument.Source = Convert(TemplateElementComposer.Preview);
        }

        private void Button_ApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            TemplateElementComposer.PreprocessImage();
        }

        private void Button_DetectRegions_Click(object sender, RoutedEventArgs e)
        {
            TemplateElementComposer.DetectRegions();
        }

        private void Button_OpenOriginalImage_Click(object sender, RoutedEventArgs e)
        {
            

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.Filter = "All Files|*.*";
            ofd.ShowDialog();

            if (ofd.FileName != "" || ofd.FileName != null)
            {
                TemplateElementComposer.Original = new Bitmap(ofd.FileName);
            }
        }

        private void Button_FilterRegions_Click(object sender, RoutedEventArgs e)
        {
            TemplateElementComposer.FilterRegions(CustomTempalteElementSampleController.KernelSize);
        }


        private void Button_GenerateRegionsSample_Click(object sender, RoutedEventArgs e)
        {
            var jsonTemplate = JsonConvert.SerializeObject(TemplateElementComposer.TemplateElementMananger);
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Sample files|*.sample";
            saveFileDialog.ShowDialog();

            if (saveFileDialog.SafeFileName != "")
            {
                StreamWriter sw = File.CreateText(saveFileDialog.FileName);
                sw.Write(jsonTemplate);
                sw.Close();
            }
        }

        private void Image_OriginalDocument_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            double coef = TemplateElementComposer.Original.Width/ (double)ImageMaskedDocument.ActualWidth ;
            System.Drawing.Point currentPosition =
                new System.Drawing.Point((int) (e.GetPosition(ImageMaskedDocument as IInputElement).X*coef),
                    (int) (e.GetPosition(ImageMaskedDocument as IInputElement).Y*coef));
            TemplateElement selected = TemplateElementComposer.TemplateElementMananger.getElementAtPoint(currentPosition);
            if (selected != null)
                CustomTemplateElementController.ControlledTemplatElement = selected;
        }


        /// <summary>
        /// Метод конвертации битмапов в image.битмапы
        /// </summary>
        /// <param name="inputValue"></param>
        /// <returns></returns>
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