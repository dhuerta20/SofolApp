using Google.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofolApp.MVVM.Models
{
    public class PaymentCard
    {
        public Guid CardID { get; set; }

        public Guid UserID { get; set; }

        public string CardNumber { get; set; }

        public Date ExpireDate { get; set; }

        public string CardType { get; set; }
    }
}
