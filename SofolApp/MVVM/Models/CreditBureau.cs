using Google.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofolApp.MVVM.Models
{
    public class CreditBureau
    {
        public Guid BureauID { get; set; }

        public Guid UserID { get; set; }

        public float CreditScore { get; set; }

        public Date  ConsultDate { get; set; }

        public string CreditHistoryRsume { get; set; }
    }
}
