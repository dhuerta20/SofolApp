using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofolApp.MVVM.Models
{
    class User 
    {
        public string Id {  get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
        
        public string Password { get; set; }

        public DateTime BirthDate  { get; set; }

        public string CivilState  { get; set; }

        public string Gender { get; set; }

        public string Ocupation { get; set; }

        public string IdPhotoUrl { get; set; }

        public string ProofOfAddressUrl { get; set; }

        public string ProofOfIncomeUrl { get; set; }

        public string FacePhotoUrl { get; set; }

        public bool IsValid { get; set; }

        public bool IsAdmin { get; set; }
        
    }
}
