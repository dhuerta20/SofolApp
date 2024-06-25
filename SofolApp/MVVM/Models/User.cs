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
        public int UserID {  get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
        
        public string Password { get; set; }

        public DateTime BirthDate  { get; set; }

        public string CivilState  { get; set; }

        public string Gender { get; set; }

        public string Ocupation { get; set; }

        public Image FacePic { get; set; }
        
    }
}
