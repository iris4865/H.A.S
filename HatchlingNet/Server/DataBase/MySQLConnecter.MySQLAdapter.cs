using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace DataBase
{
    partial class MySQLConnecter
    {
        private class MySQLAdapter
        {
            MySqlConnection connection;
            public bool connectDatabase { get; private set; }

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
                    this.connectDatabase = false;
                    return false;
                }

                this.connectDatabase = true;
                return true;
            }

            public List<string> ShowTables()
            {
                return SendQuery("SHOW TABLES;");
            }

            public bool CreateTable(string primaryKeyId)
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

            public bool addColumns(MySQLDataType type, string columnId, int size, bool defaultNull)
            {
                string queryString = $"alter table {tableName} add {columnId} {type.ToString()}({size.ToString()}) DEFALUT ";
                if (defaultNull)
                    queryString += "NULL;";
                else
                    queryString += "NOT NULL;";

                SendNoQuery(queryString);
                return true;
            }

            public List<string> showColumns()
            {
                return SendQuery($"SHOW COLUMNS FROM {tableName};");
            }

            public List<string> showColumn(string field)
            {
                return SendQuery("SELECT ");
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
                    {
                        string row = "";

                        for (int i = 0; i < reader.FieldCount; i++)
                            row += reader.GetString(i) + "\t";

                        resultList.Add(row);
                    }
                }
                return resultList;
            }
        }
    }

}