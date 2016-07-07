
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

        public TemplateElement ControlledTemplatelement { set; get; }
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

        public void setSelectedTemplateElement(TemplateElement inputTemplateElement) {
            ControlledTemplatelement = inputTemplateElement;
            current = ControlledTemplatelement.Rect;
            NotifyPropertyChanged("RectangleX");
            NotifyPropertyChanged("RectangleY");
            NotifyPropertyChanged("RectangleWidth");
            NotifyPropertyChanged("RectangleHeight");
            NotifyPropertyChanged("RectangleName");
            NotifyPropertyChanged("RectangleDescription");
        }

        public int RectangleX {
            set
            {
                current.X = value;
                ControlledTemplatelement.Rect = current;
                ControlledTemplatelement.RaiseTempleateElementChangedEvent(ControlledTemplatelement);
            }
            get
            {
                return current.X;
            }
        }
        public int RectangleY {
            set
            {
                current.Y = value;
                ControlledTemplatelement.Rect = current;
                ControlledTemplatelement.RaiseTempleateElementChangedEvent(ControlledTemplatelement);
            }
            get
            {
                return current.Y;
            }
        }
        public int RectangleWidth {
            set
            {
                current.Width = value;
                ControlledTemplatelement.Rect = current;
                ControlledTemplatelement.RaiseTempleateElementChangedEvent(ControlledTemplatelement);
            }
            get
            {
                return current.Width;
            }
        }
        public int RectangleHeight {
            set
            {
                current.Height = value;
                ControlledTemplatelement.Rect = current;
                ControlledTemplatelement.RaiseTempleateElementChangedEvent(ControlledTemplatelement);
            }
            get
            {
                return current.Height;
            }
        }
        public string RectangleName {
            set
            {
                ControlledTemplatelement.Name = value;
                ControlledTemplatelement.RaiseTempleateElementChangedEvent(ControlledTemplatelement);
            }
            get
            {
                return ControlledTemplatelement.Name;
            }
        }
        public string RectangleDescription {
            set
            {
                string[] strings;
                strings = value.Split(' ');
                if (strings != null)
                    foreach (string current in strings)
                        ControlledTemplatelement.Marks.Add(current);
                ControlledTemplatelement.RaiseTempleateElementChangedEvent(ControlledTemplatelement);
            }
            get
            {
                string res = "";
                foreach (string current in ControlledTemplatelement.Marks)
                    res += current + " ";
                return res;
            }
        }

    }
}
