using Google.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofolApp.MVVM.Models
{
    public class SessionHistory
    {
        public Guid SessionID {  get; set; }

        public Guid UserID{ get; set; }

        public Date StartTime { get; set; }

        public Date EndTime { get; set; }

        public string Device {  get; set; }

        public float IpDir { get; set; }
    }
}
