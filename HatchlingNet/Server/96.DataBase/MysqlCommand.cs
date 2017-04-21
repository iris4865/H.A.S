using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace DataBase
{
    public class MysqlCommand
    {
        IMySqlAdapter adapter;

        public MysqlCommand()
        {
            adapter = MySqlAdapter.GetInstance();
        }

        public void Connect(string remoteAddress, string Password)
        {
            string connectString = $"Server={remoteAddress};Uid=root;Pwd={Password};";
            adapter.Connect(connectString);
        }
        /*
        public bool IsDataBase()
        {
            if (string.IsNullOrEmpty(databaseName))
                return false;

            return this.IsExist($"SHOW DATABASES LIKE '{databaseName}'");
        }

        public bool IsDataBase(string databaseName)
        {
            this.databaseName = databaseName;
            return this.IsExist($"SHOW DATABASES LIKE '{databaseName}'");
        }

        private bool IsExist(string sqlQuery)
        {
            MySqlCommand command = new MySqlCommand(sqlQuery, connection);
            MySqlDataReader reader = command.ExecuteReader();
            bool exist = reader.HasRows;
            reader.Close();

            return exist;
        }

        public bool createDataBase(string databaseName)
        {
            adapter.SendQueryNoData($"CREATE DATABASE {databaseName};");
            return true;
        }

        public bool ConnectDataBase()
        {
            try
            {
                connection.ChangeDatabase(databaseName);
            }
            catch (MySqlException)
            {
                return false;
            }

            return true;
        }

        public List<string> ShowTables()
        {
            return adapter.SendQuery("SHOW TABLES;");
        }

        public bool CreateTable(string tableName, string primaryKeyId)
        {
            adapter.SendQueryNoData(
                $"CREATE TABLE {tableName} (" +
                $"{primaryKeyId} INT NOT NULL AUTO_INCREMENT, " +
                $"PRIMARY KEY({primaryKeyId}) " +
                $") DEFAULT CHARSET=utf16;");

            return true;
        }

        public bool RemoveTable()
        {
            adapter.SendQueryNoData($"DROP TABLE {tableName};");
            return true;
        }

        public bool IsTable()
        {
            return IsExist($"SHOW TABLES LIKE '{tableName}'");
        }

        public bool IsTable(string tableName)
        {
            return IsExist($"SHOW TABLES LIKE '{tableName}'");
        }

        public bool AddColumns(MySQLDataType type, string columnId, int size, bool defaultNull)
        {
            string queryString = $"ALTER TABLE {tableName} add {columnId} {type.ToString()}({size.ToString()}) ";
            if (!defaultNull)
                queryString += "NOT NULL;";

            adapter.SendQueryNoData(queryString);
            return true;
        }

        public List<string> ShowColumnNames()
        {
            return adapter.SendQuery($"SHOW COLUMNS FROM {tableName};");
        }

        public List<string> ShowColumn(string columnName)
        {
            return adapter.SendQuery($"SELECT {columnName} FROM {tableName}");
        }

        public bool IsField(string columnName, string findName)
        {
            return IsExist($"SELECT * FROM {tableName} WHERE {columnName}='{findName}';");
        }

        public bool CheckLogin(string id, string password)
        {
            return IsExist($"SELECT * FROM {tableName} WHERE id='{id}' AND password='{password}'");
        }

        public bool AddField(string id, string password)
        {
            adapter.SendQueryNoData($"INSERT INTO {tableName} (id, password) VALUES ('{id}','{password}');");
            return true;
        }
        */
    }
}
