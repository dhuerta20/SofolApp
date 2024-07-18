using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sofol_Utils.Utils
{
    public class ValidateLayout
    {
        public static bool ValidatePlaceHolderColor(StackLayout _container)
        {

           bool _isValid = true;
            foreach (var item in _container.Children)
            {
                if (item is Entry)
                {
                    Entry _entry = (Entry)item;
                    if (_entry.PlaceholderColor == Colors.Red)
                    {
                        _isValid = false;
                        break;
                    }
                }
            }
            return _isValid;
        }
    }
}
