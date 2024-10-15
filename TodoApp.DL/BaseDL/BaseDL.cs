using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApp.DL
{
    public class BaseDL: IBaseDL
    {
        public IDbConnection GetConnection(string connectionString)
        {
            IDbConnection connection = null;
            if(!string.IsNullOrEmpty(connectionString))
            {
                connection = new MySqlConnection(connectionString);
            }else
            {
                connection = new MySqlConnection(DatabaseConstant.ConnectionString);
            }
            return connection;
        }

        public async Task<T> ExecuteScalarCommandTextAsync<T>(string command, IDictionary<string, object> param, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            IDbConnection conn = transaction?.Connection != null ? transaction?.Connection : connection;
            conn.Open();
            var res = await conn.ExecuteScalarAsync<T>(command, param, transaction, commandType: CommandType.Text);
            conn.Close();
            return res;
        }
    }
}
