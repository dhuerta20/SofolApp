using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SofolApp.MVVM.ViewModels
{
    public class RegisterPageViewModel : ViewModelBase
    {
        private bool _isFormVisible;
        public bool IsFormVisible
        {
            get => _isFormVisible;
            set
            {
                _isFormVisible = value;
                OnPropertyChanged(nameof(IsFormVisible));
            }
        }

        public ICommand ShowFormCommand => new Command(() => IsFormVisible = true);
    }
}
