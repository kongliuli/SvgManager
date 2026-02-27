using System.Data;

namespace SvgManagerMvp.Data
{
    public class DbHelper
    {
        private string _connectionString;

        public DbHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection GetConnection()
        {
            // 这里需要根据实际的达梦数据库驱动进行修改
            // 例如：return new DmConnection(_connectionString);
            // 暂时使用SQL Server连接作为占位符
            var assembly = System.Reflection.Assembly.LoadFrom("DmProvider.dll");
            var connectionType = assembly.GetType("Dm.DmConnection");
            var connection = (IDbConnection)Activator.CreateInstance(connectionType, _connectionString);
            return connection;
        }

        public DataTable ExecuteQuery(string sql, params IDbDataParameter[] parameters)
        {
            using (var connection = GetConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }

                    connection.Open();
                    var adapter = CreateDataAdapter(command);
                    var dataSet = new DataSet();
                    adapter.Fill(dataSet);
                    return dataSet.Tables[0];
                }
            }
        }

        public int ExecuteNonQuery(string sql, params IDbDataParameter[] parameters)
        {
            using (var connection = GetConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public object ExecuteScalar(string sql, params IDbDataParameter[] parameters)
        {
            using (var connection = GetConnection())
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    foreach (var parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }

                    connection.Open();
                    return command.ExecuteScalar();
                }
            }
        }

        private IDbDataAdapter CreateDataAdapter(IDbCommand command)
        {
            // 这里需要根据实际的达梦数据库驱动进行修改
            // 例如：return new DmDataAdapter((DmCommand)command);
            // 暂时使用SQL Server数据适配器作为占位符
            var assembly = System.Reflection.Assembly.LoadFrom("DmProvider.dll");
            var adapterType = assembly.GetType("Dm.DmDataAdapter");
            var adapter = (IDbDataAdapter)Activator.CreateInstance(adapterType, command);
            return adapter;
        }

        public IDbDataParameter CreateParameter(string name, object value)
        {
            // 这里需要根据实际的达梦数据库驱动进行修改
            // 例如：return new DmParameter(name, value);
            // 暂时使用SQL Server参数作为占位符
            var assembly = System.Reflection.Assembly.LoadFrom("DmProvider.dll");
            var parameterType = assembly.GetType("Dm.DmParameter");
            var parameter = (IDbDataParameter)Activator.CreateInstance(parameterType, name, value);
            return parameter;
        }
    }
}