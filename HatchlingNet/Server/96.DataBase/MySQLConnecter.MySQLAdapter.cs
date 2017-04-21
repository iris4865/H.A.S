using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace DataBase
{
    public partial class MySQLController
    {
        private class MySQLAdapter
        {
            MySqlConnection connection;

            public string databaseName { get; set; }
            public string tableName { get; set; }

            public void Connect(string remoteAddress, string Password)
            {
                string connectArguments = $"Server={remoteAddress};Uid=root;Pwd={Password};";
                connection = new MySqlConnection(connectArguments);
            }

            public bool Open()
            {
                try
                {
                    connection.Open();
                }
                catch (MySqlException)
                {
                    Console.WriteLine("open failed.");
                    return false;
                }
                return true;
            }

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
                SendNoQuery($"CREATE DATABASE {databaseName};");
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
                return SendQuery("SHOW TABLES;");
            }

            public bool CreateTable(string tableName, string primaryKeyId)
            {
                SendNoQuery(
                    $"CREATE TABLE {tableName} (" +
                    $"{primaryKeyId} INT NOT NULL AUTO_INCREMENT, " +
                    $"PRIMARY KEY({primaryKeyId}) " +
                    $") DEFAULT CHARSET=utf16;");

                return true;
            }

            public bool RemoveTable()
            {
                SendNoQuery($"DROP TABLE {tableName};");
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

                SendNoQuery(queryString);
                return true;
            }

            public List<string> ShowColumnNames()
            {
                return SendQuery($"SHOW COLUMNS FROM {tableName};");
            }

            public List<string> ShowColumn(string columnName)
            {
                return SendQuery($"SELECT {columnName} FROM {tableName}");
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
                SendNoQuery($"INSERT INTO {tableName} (id, password) VALUES ('{id}','{password}');");
                return true;
            }

            private int SendNoQuery(string sqlQuery)
            {
                MySqlCommand command = new MySqlCommand(sqlQuery, connection);
                return command.ExecuteNonQuery();
            }

            public List<string> SendQuery(string sqlQuery)
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
        }
    }

}