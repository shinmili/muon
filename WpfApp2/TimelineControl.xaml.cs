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

namespace WpfApp2
{
    /// <summary>
    /// TimelineListBox.xaml の相互作用ロジック
    /// </summary>
    public partial class TimelineControl : UserControl
    {
        private string type;
        public string Type
        {
            get => type;
            set
            {
                type = value;
                switch (type)
                {
                    case "Home":
                        DataContext = new TimelineViewModel(new HomeTimelineModel());
                        break;
                    case "Local":
                        DataContext = new TimelineViewModel(new LocalTimelineModel());
                        break;
                    case "Federated":
                        DataContext = new TimelineViewModel(new FederatedTimelineModel());
                        break;
                }
            }
        }

        public TimelineControl()
        {
            InitializeComponent();
        }
    }
}
