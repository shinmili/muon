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
    /// SettingsWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private SettingsViewModel ViewModel = new SettingsViewModel();

        public SettingsWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;
            ViewModel.Closing += (s, e) => { DialogResult = e.DialogResult; Close(); };
        }
    }
}
