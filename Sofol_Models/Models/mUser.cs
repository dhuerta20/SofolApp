using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sofol_Models.Models
{
    public class mUser
    {
        #region Properties

        /// <summary>
        /// UserId contains the identifier provided by Firebase
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// UserName contains the user name for login the app. 
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Name of the user
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// MiddleName of the user
        /// </summary>
        public string MiddleName {get;set;}

        /// <summary>
        /// LastName of the user
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// BirthDate of the user
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// Gender of the user
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// CivilState of the user
        /// </summary>
        public string CivilState { get; set; }

        /// <summary>
        /// Occupation of the user (Job)
        /// </summary>
        public string Occupation { get; set; }

        /// <summary>
        /// UserPhoto contains the image of the user
        /// </summary>
        public Image UserPhoto { get; set; }


        #endregion
    } 
}
