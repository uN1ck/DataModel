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

namespace DocumentLayoutAnalyseView.CustomControllers
{
    /// <summary>
    /// Логика взаимодействия для TemplateElementSampleMananger.xaml
    /// </summary>
    public partial class TemplateElementSampleManangerController : UserControl
    {
        private TemplateElementSampleMananger sampleMananger;
        public TemplateElementSampleMananger TemplateElementSampleMananger {
            set { sampleMananger = value; }
        }
        public ITemplateAnalyse TemplateAnalyse {
            get { return sampleMananger; }
        }

        public TemplateElementSampleManangerController()
        {
            InitializeComponent();
            sampleMananger = new TemplateElementSampleMananger();
            this.DataContext = this;
        }

        public double KernelSize { set { sampleMananger.MaxDistance = value; } get { return sampleMananger.MaxDistance; } }

        private void Button_Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "All Files|*.*";
            ofd.ShowDialog();

            string[] samples = null;

            if (ofd.FileName != "" || ofd.FileName != null)
            {
                samples = ofd.FileNames;
            }
        }

    }
}
