using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.Model
{
    public interface IBaseModel
    {
        string GetTableName();

        void SetPrimaryKey(dynamic key);
    }
}
