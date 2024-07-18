using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sofol_Utils.Behaviors
{
    public class RFCValidatorBehavior:Behavior<Entry>
    {
        #region Properties

        public string Pattern { get; set; }

        #endregion

        #region Methods

        protected override void OnAttachedTo(Entry entry)
        {
            base.OnAttachedTo(entry);
            entry.TextChanged += OnEntryTextChanged;
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            base.OnDetachingFrom(entry);
            entry.TextChanged -= OnEntryTextChanged;
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;
            if (entry == null || string.IsNullOrEmpty(Pattern))
                return;

            var regex = new Regex(Pattern);
            if (!regex.IsMatch(entry.Text))
            {
                // Aquí puedes manejar el caso de no coincidencia, por ejemplo:
                entry.TextColor = Colors.Red; // Cambia el color de fondo a rojo para indicar error
            }
            else
            {
                entry.TextColor = Colors.Black; // Restablece el color de fondo
            }
        }

        #endregion

    }
}
