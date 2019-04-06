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
using WpfApp2.ViewModel;

namespace WpfApp2.View
{
    /// <summary>
    /// TootBox.xaml の相互作用ロジック
    /// </summary>
    public partial class NewTootBox : UserControl
    {
        public NewTootBox()
        {
            InitializeComponent();
            NewTootBoxViewModel vm = (NewTootBoxViewModel)DataContext;
            Model.InReplyToModel.Instance.InReplyTo.Where(s => s != null)
                .Subscribe(_ =>
                {
                    Observable.FromEventPattern(TextBox, "TextChanged")
                        .Take(1)
                        .Subscribe(__ =>
                        {
                            TextBox.Focus();
                            TextBox.CaretIndex = TextBox.Text.Length;
                        });
                });
        }
    }
}
