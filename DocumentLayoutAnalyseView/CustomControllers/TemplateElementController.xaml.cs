using System.Windows.Controls;
using System.Drawing;
using Enterra.DocumentLayoutAnalysis.Model;
using System.ComponentModel;

namespace DocumentLayoutAnalyseView.CustomControllers
{
    /// <summary>
    /// Логика взаимодействия для TemplateElementController.xaml
    /// </summary>
    public partial class TemplateElementController : UserControl, INotifyPropertyChanged
    {
        public TemplateElement controlledTemplateElement;

        public TemplateElement ControlledTemplatElement
        {
            set
            {
                controlledTemplateElement = value;
                current = controlledTemplateElement.Rectangle;
                NotifyPropertyChanged("RectangleX");
                NotifyPropertyChanged("RectangleY");
                NotifyPropertyChanged("RectangleWidth");
                NotifyPropertyChanged("RectangleHeight");
                NotifyPropertyChanged("RectangleName");
                NotifyPropertyChanged("RectangleDescription");
            }
            get { return controlledTemplateElement; }
        }

        private System.Drawing.Rectangle current = new System.Drawing.Rectangle();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        public TemplateElementController()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public int RectangleX
        {
            set
            {
                current.X = value;
                ControlledTemplatElement.Rectangle = current;
            }
            get { return current.X; }
        }

        public int RectangleY
        {
            set
            {
                current.Y = value;
                ControlledTemplatElement.Rectangle = current;
            }
            get { return current.Y; }
        }

        public int RectangleWidth
        {
            set
            {
                current.Width = value;
                ControlledTemplatElement.Rectangle = current;
            }
            get { return current.Width; }
        }

        public int RectangleHeight
        {
            set
            {
                current.Height = value;
                ControlledTemplatElement.Rectangle = current;
            }
            get { return current.Height; }
        }

        public string RectangleName
        {
            set { ControlledTemplatElement.Name = value; }
            get { return ControlledTemplatElement.Name; }
        }

        public string RectangleDescription
        {
            set
            {
                string[] strings;
                strings = value.Split(' ');
                if (strings != null)
                    foreach (string current in strings)
                        ControlledTemplatElement.Marks.Add(current);
            }
            get
            {
                string res = "";
                foreach (string current in ControlledTemplatElement.Marks)
                    res += current + " ";
                return res;
            }
        }
    }
}