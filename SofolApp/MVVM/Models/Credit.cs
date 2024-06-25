using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofolApp.MVVM.Models
{
    public class Credit
    {
        public Guid CreditID { get; set; }

        public Guid UserID { get; set; }

        public decimal Amount {  get; set; }

        public decimal InterestRate { get; set; }

        public DateTime Term {  get; set; }

        public DateTime InitialDate { get; set; }

        public DateTime FinalDate { get; set; }

        public string CreditType { get; set; }
    }
}
