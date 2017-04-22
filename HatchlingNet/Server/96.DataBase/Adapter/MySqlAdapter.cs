using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;

namespace DataBase
{
    sealed class MySqlAdapter : IDisposable, IMySqlAdapter
    {
        private static readonly Lazy<MySqlAdapter> instance = new Lazy<MySqlAdapter>(() => new MySqlAdapter());
        MySqlConnection connection;

        public static MySqlAdapter GetInstance()
        {
            return instance.Value;
        }

        public bool Connect(string connectionString)
        {
            connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();
            }
            catch (MySqlException)
            {
                return false;
            }
            return true;
        }

        public bool SelectDatabase(string dbName)
        {
            try
            {
                connection.ChangeDatabase(dbName);
            }
            catch (MySqlException)
            {
                return false;
            }

            return true;
        }

        public void Dispose()
        {
            connection.Dispose();
        }

        public bool IsExist(string sqlQuery)
        {
            if (MySqlHelper.ExecuteScalar(connection, sqlQuery) == null)
                return false;

            return true;
        }

        public bool SendQueryNoData(string sqlQuery)
        {
            if (MySqlHelper.ExecuteNonQuery(connection, sqlQuery) == -1)
                return false;

            return true;
        }

        public DataSet SendQueryDataSet(string sqlQuery)
        {
            return MySqlHelper.ExecuteDataset(connection, sqlQuery);
        }

        public List<string> SendQueryList(string sqlQuery)
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

        //시험 끝나고 수정
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

    }

}
