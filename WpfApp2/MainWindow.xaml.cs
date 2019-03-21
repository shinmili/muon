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
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            foreach (TabItem item in Tab.Items)
            {
                TimelineViewModel tlvm = (TimelineViewModel)((TimelineControl)item.Content).DataContext;
                if (tlvm.ReloadCommand.CanExecute(null))
                {
                    tlvm.ReloadCommand.Execute(null);
                }
            }
        }
    }
}
