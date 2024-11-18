using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Common.Utils;
using TodoApp.DL;
using TodoApp.Model;
using TodoApp.Model.Enums;
using TodoApp.Model.Models.DTO;

namespace TodoApp.BL
{
    public class BaseBL : IBaseBL
    {
        protected IBaseDL _dl;

        private static Dictionary<string, List<string>> ValidColumnsOfTable { get; set; } = new Dictionary<string, List<string>>();

        public async Task<List<string>> GetValidColumns(string tableName)
        {
            try
            {
                if (ValidColumnsOfTable.TryGetValue(tableName, out var columns))
                {
                    return columns;
                }
                else
                {
                    using(var connection = _dl.GetConnection(""))
                    {
                        var databaseColumns = await _dl.QueryUsingCommandTextAsync<DatabaseColumn>($"SHOW COLUMNS FROM {tableName};", null, connection:connection);
                        columns = databaseColumns.Select(x => x.Field).ToList();
                    }
                    ValidColumnsOfTable.Add(tableName, columns);
                }
                return columns;

            }catch (Exception ex)
            {
                return [];
            }
        }

        public BaseBL(IBaseDL dl)
        {
            this._dl = dl;
        }

        public async Task<PagingResponse> GetPaging<T>(PagingRequest pagingRequest) where T : IBaseModel
        {
            var response = new PagingResponse();
            var commandText = "SELECT {0} FROM {1} WHERE {2}";

            var instance = Activator.CreateInstance<T>();
            var columns = "*";
            var tableName = instance.GetTableName() ?? instance.GetType().Name;
            if (pagingRequest.Columns != null)
            {
                //Xử lý lọc ra cột không hợp lệ
                var columnsFromClient = pagingRequest.Columns.Split(",");
                var validColumns = (await GetValidColumns(tableName)).Select(col => col.ToLower());
                columns = string.Join(",",columnsFromClient.Where(col => validColumns.Contains(col.ToLower().Trim())));

            }
            var type = typeof(T);
            var param = new Dictionary<string, object>();
            var condition = await BuildPagingCommandText(type, pagingRequest, param);
            commandText = string.Format(commandText, new object[] { columns, tableName, condition });
            using(var connection = _dl.GetConnection(""))
            {
                var result = await _dl.QueryMultipleUsingCommandTextAsync(new List<Type>() { typeof(object), typeof(int) }, commandText, param, connection);
                response.PageData = result?.ElementAt(0);
                response.Total = Convert.ToInt32(result?.ElementAt(1).FirstOrDefault()?.ToString() ?? "0");
            }
            return response;
        }

        public async Task<string> BuildPagingCommandText(Type type, PagingRequest pagingRequest, Dictionary<string, object> param, bool disabledLimit = false)
        {
            var builder = new StringBuilder();

            int pageIndex = pagingRequest.PageIndex;
            int pageSize = pagingRequest.PageSize;
            var conditions = new List<string>();
            if (!string.IsNullOrWhiteSpace(pagingRequest.Filter))
            {
                var filter = JsonConvert.DeserializeObject<JArray>(pagingRequest.Filter);
                var condition = BuildCondition(filter, param);
                if (!string.IsNullOrEmpty(condition))
                {
                    conditions.Add(condition);
                }
            }
            if (!string.IsNullOrWhiteSpace(pagingRequest.CustomFilter))
            {
                var filter = JsonConvert.DeserializeObject<JArray>(pagingRequest.CustomFilter);
                var condition = BuildCondition(filter, param);
                if (!string.IsNullOrEmpty(condition))
                {
                    conditions.Add(condition);
                }
            }
            if (pagingRequest.QuickSearch != null)
            {
                var searchObject = pagingRequest.QuickSearch;
                var condition = await BuildSearchCondition(type, searchObject, param);
                if (!string.IsNullOrEmpty(condition))
                {
                    conditions.Add(condition);
                }
            }
            if (conditions.Count > 0)
            {
                builder.Append(string.Join(" AND ", conditions));
            }
            else
            {
                builder.Append("1=1");
            }
            if (pagingRequest.Sort != null)
            {
                var sort = JsonConvert.DeserializeObject<List<PagingSort>>(pagingRequest.Sort ?? "");
                builder.Append(BuildSortOrder(sort));
            }

            if (!disabledLimit)
            {
                builder.Append(BuildLimit(ref pageSize, ref pageIndex));
            }

            builder.Append(BuildSelectCount(type, string.Join(" AND ", conditions)));

            var commandText = builder.ToString();



            return commandText;
        }


        

        private string BuildCondition(JToken filters, Dictionary<string, object> param)
        {
            if (filters != null && filters?.Count() > 0)
            {
                string whereCondition = "";
                int index = 0;
                foreach (var element in filters)
                {
                    if (element == null)
                    {
                        return whereCondition;
                    }
                    if (element.GetType() == typeof(JValue))
                    {
                        if (index == 0)
                        {
                            
                            var column = element.ToString();
                            whereCondition += " " + column + " ";
                            
                        }
                        else
                        if (index == 2)
                        {
                            var stringParam = Utils.GenerateSearchParam();
                            whereCondition = string.Format(whereCondition, stringParam);
                            var conditionValue = GetConditionValue(filters?[1]?.ToString() ?? "", element.ToString());
                            param.TryAdd(stringParam, conditionValue);
                        }
                        else
                        {
                            whereCondition += " " + GetCondition(element.ToString()) + " ";
                        }
                    }
                    if (element.GetType() == typeof(JArray))
                    {
                        whereCondition += this.BuildCondition(element, param);
                    }
                    index++;
                    //Console.WriteLine(element.GetType() == typeof(JArray));
                }
                return "(" + whereCondition + ")";
            }
            return "";
        }

        private string GetCondition(string value)
        {
            switch (value.ToUpper())
            {
                case Operator.Equal:
                    return "= {0}";
                case Operator.StartWith:
                case Operator.EndWith:
                case Operator.Contains:
                    return "LIKE {0}";
                case Operator.NotEqual:
                    return "<> {0}";
                case Operator.LessOrEqual:
                    return "<= {0}";
                case Operator.GreaterOrEqual:
                    return ">= {0}";
                case Operator.LessThan:
                    return "< {0}";
                case Operator.GreaterThan:
                    return "> {0}";
                case Operator.And:
                    return "AND";
                case Operator.Or:
                    return "OR";
                case Operator.IsNotNull:
                    return "IS NOT NULL";
                case Operator.NotNull:
                    return "NOT NULL";
                case Operator.In:
                    return "IN {0}";
                case Operator.NotIn:
                    return "NOT IN {0}";
            }
            return "LIKE {0}";
        }

        private object GetConditionValue(string op, string value)
        {
            switch (op.ToUpper())
            {
                case Operator.Contains:
                    return $"%{SecurityUtils.SafetyCharsForLIKEOperator(value)}%";
                case Operator.StartWith:
                    //return "LIKE";
                    return $"{SecurityUtils.SafetyCharsForLIKEOperator(value)}%";
                case Operator.EndWith:
                    //return "LIKE";
                    return $"%{SecurityUtils.SafetyCharsForLIKEOperator(value)}";
                case Operator.In:
                case Operator.NotIn:
                    return GetMultipleValue(SecurityUtils.SafetyCharsForLIKEOperator(value));

            }
            return $"{SecurityUtils.SafetyCharsForLIKEOperator(value)}";
        }

        private string[] GetMultipleValue(string rawValue)
        {
            try
            {
                if (rawValue != null)
                {
                    return rawValue.Split(",");
                }

            }
            catch (Exception)
            {
                return new string[] { };
            }
            return new string[] { };

        }

        private string BuildSortOrder(IEnumerable<PagingSort> sorts)
        {

            var sortOrder = new StringBuilder();
            if (sorts != null)
            {
                sortOrder.Append(" ORDER BY");
                foreach (var sort in sorts)
                {
                    if (sort.Random)
                    {
                        sortOrder.Append(" RAND(),");
                    }
                    else
                    {
                        sortOrder.Append(" " + sort.Selector + " " + (sort.Desc ? "DESC" : "ASC") + ",");
                    }
                }

                return sortOrder.ToString().Remove(sortOrder.Length - 1);

            }
            return "";
        }

        private async Task<string> BuildSearchCondition(Type typeModel, QuickSearch quickSearch, Dictionary<string, object> parameter)
        {
            var columns = quickSearch.Columns?.Split(",").Select(c => c.Trim()).ToList();
            var instance = Activator.CreateInstance(typeModel) as IBaseModel;
            var tableName = instance.GetTableName();
            var conditions = new List<string>();
            if (quickSearch != null && quickSearch.Columns != null && quickSearch.Columns.Length > 0)
            {
                parameter.Add("@SearchValue", $"%{SecurityUtils.SafetyCharsForLIKEOperator(quickSearch.SearchValue)}%");
                columns = await GetValidColumns(tableName);
                if (columns != null)
                {
                    foreach (var column in columns)
                    {
                        
                        conditions.Add($"( {column} LIKE @SearchValue)");
                        
                    }
                    return "( " + string.Join(" OR ", conditions) + " )";
                }

            }
            return "";
        }

        private string BuildSelectCount(Type typeModel, string condition)
        {
            var instance = Activator.CreateInstance(typeModel) as IBaseModel;
            condition = string.IsNullOrWhiteSpace(condition) ? "1=1" : condition;
            var tableName = instance.GetTableName() ?? typeModel.Name;
            var commandText = $"SELECT COUNT(1) FROM {tableName} WHERE {condition}";
            return commandText;
        }





        public string BuildLimit(ref int pageSize, ref int pageIndex)
        {
            if (pageSize > DatabaseConstant.MaxReturnRecord || pageSize <= 0)
            {
                pageSize = DatabaseConstant.MaxReturnRecord;
            }
            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }
            if (pageIndex > DatabaseConstant.MaxPageIndex)
            {
                pageIndex = DatabaseConstant.MaxPageIndex;
            }
            return $" LIMIT {pageSize} OFFSET {(pageIndex - 1) * pageSize};";
        }






        public async Task<T> InserOne<T>(T baseModel) where T : IBaseModel, ICreationInfo
        {
            var tableName = baseModel.GetTableName();
            var sql = "INSERT INTO {tableName} ({columns}) VALUES ({values}); SELECT LAST_INSERT_ID()";
            
            var props = baseModel.GetType().GetProperties();
            baseModel.CreatedBy = "Current user";//Todo: làm tính năng đăng nhập
            baseModel.CreatedDate = DateTime.Now;
            var param = new Dictionary<string, object>();
            var columns = new List<string>();
            foreach ( var prop in props )
            {
                if(prop.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault() != null)
                {
                    var value = prop.GetValue(baseModel);
                    if( value != null)
                    {
                        columns.Add(prop.Name);
                        param.Add($"@{prop.Name}", value);
                    }
                }
            }
            sql = sql.Replace("{tableName}", tableName)
                .Replace("{columns}", string.Join(",", columns))
                .Replace("{values}", string.Join(",", columns.Select(col => $"@{col}")));


            using (var conn = _dl.GetConnection(""))
            {
                var lastId = await _dl.ExecuteScalarCommandTextAsync<int>(sql, param, conn);
                baseModel.SetPrimaryKey(lastId);
            }

            return baseModel;


        }

        public async Task<T> UpdateOne<T>(T baseModel) where T : IBaseModel, IModificationInfo
        {
            var tableName = baseModel.GetTableName();
            var sql = "UPDATE {0} SET {1} WHERE {2}";
            var props = baseModel.GetType().GetProperties();
            baseModel.ModifiedBy = "Current user";//Todo: làm tính năng đăng nhập
            baseModel.ModifiedDate = DateTime.Now;
            var param = new Dictionary<string, object>();
            var columns = new List<string>();
            string whereCondition = string.Empty;
            foreach (var prop in props)
            {
                if (prop.Name == "CreatedDate" || prop.Name == "CreatedBy")
                {
                    continue;
                }
                var value = prop.GetValue(baseModel);
                if (value != null)
                { 
                    var propParam = $"@{prop.Name}";

                    if (prop.GetCustomAttributes(typeof(KeyAttribute), true).FirstOrDefault() != null)
                    {
                        whereCondition = $"{prop.Name} = {propParam}";
                        param.Add(propParam, value);
                    }
                    else
                    if (prop.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault() != null)
                    {
                        columns.Add($"{prop.Name} = {propParam}");
                        param.Add(propParam, value);
                    
                    }
                } 
            }
            sql = string.Format(sql, new string[] { tableName, string.Join(",", columns), whereCondition });

            var success = false;
            using (var conn = _dl.GetConnection(""))
            {
                var res = await _dl.ExecuteCommandTextAsync(sql, param, conn);
                success = res > 0;
            }
            if (success)
            {
                return baseModel;

            }
            return default(T);

        }


        public async Task<bool> DeleteOne<T>(int id) where T : IBaseModel
        {
            var sql = "DELETE FROM {0} WHERE {1}";
            var baseModel = Activator.CreateInstance<T>();
            var tableName = baseModel.GetTableName();
            var props = baseModel.GetType().GetProperties();
            var keyProp = props.FirstOrDefault(prop => prop.GetCustomAttributes(typeof(KeyAttribute), true).FirstOrDefault() != null);
            var keyValue = id;
            var whereCondition = string.Empty;
            var param = new Dictionary<string, object>();
            var keyParam = $"@{keyProp.Name}";
            whereCondition = $"{keyProp.Name} = {keyParam}";
            param.Add(keyParam, keyValue);
            
            sql = string.Format(sql, new string[] { tableName, whereCondition });

            var success = false;
            using (var conn = _dl.GetConnection(""))
            {
                var res = await _dl.ExecuteCommandTextAsync(sql, param, conn);
                success = res > 0;
            }
            
            return success;

        }


    }
}
