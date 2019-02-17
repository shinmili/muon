using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xaml;

namespace WpfApp2
{
    class StatusBlock : TextBlock
    {
        public List<Inline> InlineList
        {
            get => (List<Inline>)GetValue(InlineListProperty);
            set
            {
                SetValue(InlineListProperty, value);
            }
        }

        public static readonly DependencyProperty InlineListProperty =
            DependencyProperty.Register("InlineList", typeof(List<Inline>), typeof(StatusBlock), new PropertyMetadata(new PropertyChangedCallback(OnInlineListChanged)));

        public static void OnInlineListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var block = d as StatusBlock;
            foreach (var inline in (List<Inline>)e.NewValue)
            {
                block.Inlines.Add(inline);
            }
        }
    }
}
