using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
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
            MainViewModel mvm = (MainViewModel)DataContext;

            foreach (dynamic item in Tab.Items)
            {
                TimelineViewModel tlvm = item.Content.DataContext;
                tlvm.InReplyTo
                    .Where(svm => svm != null)
                    .Subscribe(svm =>
                    {
                        mvm.InReplyTo.Value = svm;
                        Observable.FromEventPattern(TextBox, "TextChanged")
                            .Take(1)
                            .Subscribe(_ =>
                            {
                                TextBox.Focus();
                                TextBox.CaretIndex = TextBox.Text.Length;
                            });
                    });
                mvm.InReplyTo
                    .Where(svm => svm != tlvm.InReplyTo.Value)
                    .Subscribe(svm => tlvm.InReplyTo.Value = null);
                tlvm.ReloadCommand.Execute();
            }
        }
    }
}
