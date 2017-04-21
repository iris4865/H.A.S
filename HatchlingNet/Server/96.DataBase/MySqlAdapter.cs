using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace DataBase
{
    sealed class MySqlAdapter : IDisposable, IMySQLAdapter
    {
        private static readonly Lazy<MySqlAdapter> instance = new Lazy<MySqlAdapter>(() => new MySqlAdapter());
        MySqlConnection connection;

        public static MySqlAdapter GetInstance()
        {
            return instance.Value;
        }

        public bool Connect(string remoteAddress, string Password)
        {
            string connectArguments = $"Server={remoteAddress};Uid=root;Pwd={Password};";
            connection = new MySqlConnection(connectArguments);
            
            try
            {
                connection.Open();
            }
            catch(MySqlException)
            {
                return false;
            }
            return true;
        }

        public void Dispose()
        {
            connection.Dispose();
        }

        public DataSet SendQuery(string sqlQuery)
        {
            return MySqlHelper.ExecuteDataset(connection, sqlQuery);
        }
        
        public bool SendQueryNoData(string sqlQuery)
        {
            if (MySqlHelper.ExecuteNonQuery(connection, sqlQuery) == -1)
                return false;

            return true;
        }
    }

}
