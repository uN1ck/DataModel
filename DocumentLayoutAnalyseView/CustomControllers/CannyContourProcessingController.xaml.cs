﻿using System.Windows.Controls;
using Enterra.DocumentLayoutAnalysis.Model;

namespace DocumentLayoutAnalyseView.CustomControllers
{
    /// <summary>
    /// Логика взаимодействия для CannyContourProcessingController.xaml
    /// </summary>
    public partial class CannyContourProcessingController : UserControl
    {

        private CannyContourImageProcessor cannyContourImageProcessor;
        public CannyContourImageProcessor CannyContourImageProcessor { set { cannyContourImageProcessor = value; } }
        public IImageProcessor ImageProcessor { get { return cannyContourImageProcessor; } }

        public CannyContourProcessingController()
        {
            InitializeComponent();
            cannyContourImageProcessor = new CannyContourImageProcessor();
            this.DataContext = this;
        }

        public bool IsCannyActive
        {
            set { cannyContourImageProcessor.IsCannyActive = value; }
            get { return cannyContourImageProcessor.IsCannyActive; }
        }

        public int SizeOfMinimumRegion
        {
            set { cannyContourImageProcessor.SizeOfMinimumRegion = value; }
            get { return cannyContourImageProcessor.SizeOfMinimumRegion; }
        }

        public double CannyThreshold
        {
            set { cannyContourImageProcessor.CannyThreshold = value; }
            get { return cannyContourImageProcessor.CannyThreshold; }
        }

        public double CannyThresLinking
        {
            set { cannyContourImageProcessor.CannyThresLinking = value; }
            get { return cannyContourImageProcessor.CannyThresLinking; }
        }

        public double RectangleCrossPercent
        {
            set { cannyContourImageProcessor.RectangleCrossPercent = value; }
            get { return cannyContourImageProcessor.RectangleCrossPercent; }
        }
    }
}
