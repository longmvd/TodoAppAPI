using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Model;

namespace TodoApp.BL
{
    public interface IBaseBL
    {
        Task<T> InserOne<T>(T baseModel) where T : IBaseModel, ICreationInfo;
    }
}
