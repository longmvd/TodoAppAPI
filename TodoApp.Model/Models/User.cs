using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Model.Models
{
    public class User: ICreationInfo, IModificationInfo
    {
        private string _createdBy;
        private DateTime _createdDate;
        private string _modifiedBy;
        private DateTime _modifiedDate;
        [Key]
        public string UserID { get; set; }

        [Column]
        public string Username { get; set; }

        [Column]
        public string Password { get; set; }

        [Column]
        public string Fullname { get; set; }

        [Column]
        public DateTime CreatedDate { get => _createdDate; set => _createdDate = value; }

        [Column]
        public string CreatedBy { get => _createdBy; set => _createdBy = value; }

        [Column]
        public DateTime ModifiedDate { get => _modifiedDate; set => _modifiedDate = value; }

        [Column]
        public string ModifiedBy { get => _modifiedBy; set => _modifiedBy = value; }
    }
}
