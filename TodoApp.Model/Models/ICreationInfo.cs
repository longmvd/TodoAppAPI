using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Model.Models
{
    public interface ICreationInfo
    {
        DateTime CreatedDate { get; set; }

        string CreatedBy { get; set; }
    }
}
