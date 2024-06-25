using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofolApp.MVVM.Models
{
    public class BankInfo
    {
        public Guid BankInfoId {  get; set; }

        public Guid UserID { get; set; }

        public string BankName { get; set; }

        public string AccountNumber { get; set; }

        public int BankKey { get; set; }

    }
}
