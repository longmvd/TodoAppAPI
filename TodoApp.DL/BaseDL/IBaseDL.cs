using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.DL
{
    public interface IBaseDL
    {
        IDbConnection GetConnection(string connectionString);
        Task<T> ExecuteScalarCommandTextAsync<T>(string command, IDictionary<string, object> param, IDbConnection? connection = null, IDbTransaction? transaction = null);
    }
}
