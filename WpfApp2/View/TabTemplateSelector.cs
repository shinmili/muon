using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfApp2.ViewModel;

namespace WpfApp2.View
{
    class TabTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var vm = (TabContentViewModelBase)item;
            switch (vm)
            {
                case AccountTabViewModel _:
                    return (DataTemplate)((FrameworkElement)container).FindResource("Account");
                default:
                    return (DataTemplate)((FrameworkElement)container).FindResource("Timeline");
            }
        }
    }
}
