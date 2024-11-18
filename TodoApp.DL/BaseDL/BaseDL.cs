using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

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

        public async Task<int> ExecuteCommandTextAsync(string command, IDictionary<string, object> param, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            IDbConnection conn = transaction?.Connection != null ? transaction?.Connection : connection;
            conn.Open();
            var res = await conn.ExecuteAsync(command, param, transaction, commandType: CommandType.Text);
            conn.Close();
            return res;
        }

        public async Task<IEnumerable<T>> QueryUsingCommandTextAsync<T>(string command, IDictionary<string, object> param, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            IDbConnection conn = transaction?.Connection != null ? transaction?.Connection : connection;
            conn.Open();
            var res = await conn.QueryAsync<T>(command, param, transaction, commandType: CommandType.Text);
            conn.Close();
            return res;
        }

        public async Task<List<List<object>>> QueryMultipleUsingCommandTextAsync(List<Type> types, string command, IDictionary<string, object> param, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            List<List<object>> result = [];
            IDbConnection conn = transaction?.Connection != null ? transaction?.Connection : connection;
            try
            {
                if (conn != null)
                {
                    conn.Open();
                    GridReader? reader = await conn.QueryMultipleAsync(command, param, transaction, null, CommandType.Text);
                    int index = 0;
                    do
                    {
                        if (types != null && types.Count > index)
                        {
                            var ans = await reader.ReadAsync(types[index]);
                            result.Add(ans.ToList());
                        }
                        else
                        {
                            result.Append(await reader.ReadAsync<dynamic>());
                        }
                        index++;

                    } while (!reader.IsConsumed);
                    conn.Close();
                }
                return result;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.StackTrace);
                throw;
            }
        }
    }
}
