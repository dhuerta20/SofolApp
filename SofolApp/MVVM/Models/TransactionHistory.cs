using Google.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofolApp.MVVM.Models
{
    public class TransactionHistory
    { 
        public Guid TransactionID { get; set; }

        public Guid UserID { get; set; }

        public Guid CreditID { get; set; }
        
        public Date DateOfTrans { get; set; }

        public string TypeOftransaction { get; set; }

        public decimal Amount { get; set; }

        public string Status { get; set; }
    }
}
