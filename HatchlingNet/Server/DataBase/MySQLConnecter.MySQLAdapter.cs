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
            string databaseName;

            public bool connectDatabase { get; private set; }
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
                return this.IsExist($"SHOW DATABASES LIKE '{databaseName}'");
            }

            public bool ConnectDataBase(string databaseName)
            {
                this.databaseName = databaseName;
                connection.ChangeDatabase(databaseName);
                this.connectDatabase = true;
                return true;
            }

            public bool createDataBase()
            {
                SendNoQuery($"CREATE DATABASE {databaseName};");
                return true;
            }

            private bool IsExist(string sqlQuery)
            {
                MySqlCommand command = new MySqlCommand(sqlQuery, connection);
                MySqlDataReader reader = command.ExecuteReader();
                bool exist = reader.HasRows;
                reader.Close();

                return exist;
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

            public List<string> showTables()
            {
                return SendQuery("SHOW TABLES;");
            }

            public bool removeTable()
            {
                SendNoQuery($"DROP TABLE {tableName};");
                return true;
            }

            public bool addColumns(MySQLDataType type, string columnId, int size, bool defaultNull)
            {
                string queryString = "alter table " + this.tableName + " add " + columnId + " " + type.ToString() + "(" + size.ToString() + ") DEFALUT ";
                if (defaultNull)
                    queryString += "NULL;";
                else
                    queryString += "NOT NULL;";
                SendNoQuery(queryString);
                return true;
            }

            public List<string> showColumns()
            {
                return SendQuery($"SHOW COLUMNS from '{tableName}';");
            }

            public List<string> showColumn(string field)
            {
                return SendQuery("SELECT ");
            }

            public bool IsTable(string tableName)
            {
                return IsExist($"SHOW TABLES LIKE '{tableName}'");
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

                        for(int i=0; i< reader.FieldCount; i++)
                            row += reader.GetString(i)+"\t";

                        resultList.Add(row);
                    }
                }
                return resultList;
            }
        }
    }

}