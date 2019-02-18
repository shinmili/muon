using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfApp2
{
    class DelegateCommand : ICommand
    {
        public Action<object> ExecuteHandler { get; set; }
        public Func<object, bool> CanExecuteHandler { get; set; }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => CanExecuteHandler?.Invoke(parameter) ?? true;
        public void Execute(object parameter) => ExecuteHandler?.Invoke(parameter);
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, null);
    }
}
