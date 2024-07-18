using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sofol_Utils.Behaviors
{
    public class MaxLengthValidatorBehavior: Behavior<Entry>
    {

        #region Properties

        public int MaxLength { get; set; }

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
            if (e.NewTextValue.Length > MaxLength)
            {
                ((Entry)sender).Text = e.NewTextValue.Substring(0, MaxLength);
            }
        }

        #endregion

    }
}
