using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Model.Models
{
    public class UserGroup: ICreationInfo
    {
        private string _createdBy;
        private DateTime _createdDate;
        public int UserID { get; set; }
        public int GroupID { get; set; }

        [Column]
        public DateTime CreatedDate { get => _createdDate; set => _createdDate = value; }

        [Column]
        public string CreatedBy { get => _createdBy; set => _createdBy = value; }
    }
}
