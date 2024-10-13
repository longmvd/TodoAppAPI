using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Model.Models
{
    public class Todo: ICreationInfo, IModificationInfo
    {
        private string _createdBy;
        private DateTime _createdDate;
        private string _modifiedBy;
        private DateTime _modifiedDate;

        [Key]
        public string TodoID { get; set; }

        [Column]
        public string TodoName { get; set; }

        [Column]
        public DateTime StartDate { get; set; }

        [Column]
        public DateTime EndDate { get; set; }

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
