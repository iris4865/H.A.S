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

        //오류나면 밑의 코드로 교체
        //set nocount on이면 항상 -1로 반환하기때문에 문제발생가능
        public bool SendQueryNoData(string sqlQuery)
        {
            if (MySqlHelper.ExecuteNonQuery(connection, sqlQuery) == -1)
                return false;

            return true;
            /*
            MySqlCommand command = new MySqlCommand(sqlQuery, connection);

            if (command.ExecuteNonQuery() == -1)
                return false;

            return true;
            */
        }


        //sendQuery 이전 코드
        /*
        public List<string> SendQuery2(string sqlQuery)
        {
            List<string> resultList = new List<string>();

            MySqlCommand command = new MySqlCommand(sqlQuery, connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                    resultList.Add(GetLineString(reader));
            }
            return resultList;
        }

        private string GetLineString(MySqlDataReader reader)
        {
            string line = "";

            for (int i = 0; i < reader.FieldCount; i++)
            {
                try
                {
                    line += reader.GetString(i);
                }
                catch (SqlNullValueException)
                {
                    line += "Null";
                }
                line += "\t";
            }

            return line;
        }
        */
    }

}
