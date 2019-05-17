using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Muon.ViewModel;

namespace Muon.View
{
    public class TabTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var vm = (TabContentViewModelBase)item;
            switch (vm)
            {
                case AccountTabViewModel _:
                    return (DataTemplate)((FrameworkElement)container).FindResource("Account");
                case TimelineViewModel _:
                    return (DataTemplate)((FrameworkElement)container).FindResource("Timeline");
                case NotificationsViewModel _:
                    return (DataTemplate)((FrameworkElement)container).FindResource("Notifications");
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
