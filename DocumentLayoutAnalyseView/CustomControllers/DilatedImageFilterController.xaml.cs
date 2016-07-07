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
using System.ComponentModel;

namespace DocumentLayoutAnalyseView.CustomControllers
{
    /// <summary>
    /// Логика взаимодействия для DilatedImageCFilterController.xaml
    /// </summary>
    public partial class DilatedImageFilterController : UserControl
    {
        private DilatedImageFilter dilatedImageFilter;
        public DilatedImageFilter DilatedImageFilter { set { dilatedImageFilter = value; } }

        public IImageBinarizationFilter ImageBinarizationFilter { get { return dilatedImageFilter; } }

        public DilatedImageFilterController()
        {
            dilatedImageFilter = new DilatedImageFilter();
            InitializeComponent();
            this.DataContext = this;
        }

        public int KernelSize
        {
            set
            {
                dilatedImageFilter.KernelSize = value;
            }
            get
            {
                //RaisePropertyChanged("KernelSize");
                return dilatedImageFilter.KernelSize;
            }
        }

        public int MaxBrightness
        {
            set
            {
                dilatedImageFilter.MaxBrightness = value;
            }
            get
            {
                //RaisePropertyChanged("MaxBrightness");
                return dilatedImageFilter.MaxBrightness;
            }
        }

        public int DilateCount
        {
            set
            {
                dilatedImageFilter.DilateCount = value;
            }
            get
            {
                //RaisePropertyChanged("DilateCount");
                return dilatedImageFilter.DilateCount;
            }
        }

    }
}
