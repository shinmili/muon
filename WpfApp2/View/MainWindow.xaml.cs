﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

            foreach (TabParameters tabParams in Properties.Settings.Default.Tabs)
            {
                TimelineViewModel tlvm = new TimelineViewModel(tabParams.Type, tabParams.StreamingOnStartup);
                Tab.Items.Add(tlvm);
                tlvm.InReplyTo
                    .Where(svm => svm != null)
                    .Subscribe(svm => mvm.InReplyTo.Value = svm);
                tlvm.ReplyCommand.Subscribe(_ =>
                    Observable.FromEventPattern(TextBox, "TextChanged")
                        .Take(1)
                        .Subscribe(__ =>
                        {
                            TextBox.Focus();
                            TextBox.CaretIndex = TextBox.Text.Length;
                        }));
                mvm.InReplyTo
                    .Where(svm => svm != tlvm.InReplyTo.Value)
                    .Subscribe(svm => tlvm.InReplyTo.Value = null);
                tlvm.ReloadCommand.Execute();
            }
        }
    }
}
