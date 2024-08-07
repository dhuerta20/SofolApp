using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofolApp.MVVM.Models
{
    public class Users
    {
        public string userId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string IsValid { get; set; }
        public bool IsAdmin { get; set; }
        public Dictionary<string, string> Images { get; set; } = new Dictionary<string, string>();
        public string? FirstReference { get; set; }
        public string? SecondReference { get; set; }
        public string? ThirdReference { get; set; }
        public string? AdminNotes { get; set; }
    }
}
