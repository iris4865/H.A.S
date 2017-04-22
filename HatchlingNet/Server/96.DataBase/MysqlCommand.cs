using System;
using System.Collections.Generic;
using System.Data;

namespace DataBase
{
    public class MysqlCommand
    {
        IMySqlAdapter adapter;
        MysqlDatabaseList database;
        MysqlTable table;

        public MysqlCommand()
        {
            adapter = MySqlAdapter.GetInstance();
        }

        public void Connect(string remoteAddress, string Password)
        {
            string connectString = $"Server={remoteAddress};Uid=root;Pwd={Password};";
            adapter.Connect(connectString);
        }

        public bool OpenDatabase(string dbName)
        {
            if (IsDatabase(dbName))
            {
                MysqlDatabase = new MysqlDatabaseList(adapter, dbName);
            }
                

            return false;
        }

        public bool IsDatabase(string dbName)
        {
            return adapter.IsExist($"SHOW DATABASES LIKE '{dbName}'");
        }

        public bool CreateDataBase(string dbName)
        {
            adapter.SendQueryNoData($"CREATE DATABASE {dbName};");
            return true;
        }

        public bool IsTable(string tableName)
        {
            return adapter.IsExist($"SHOW TABLES LIKE '{tableName}'");
        }

        public bool CreateTable(string tableName, string primaryKeyId)
        {
            return adapter.SendQueryNoData(
                $"CREATE TABLE {tableName} (" +
                $"{primaryKeyId} INT NOT NULL AUTO_INCREMENT, " +
                $"PRIMARY KEY({primaryKeyId}) " +
                $") DEFAULT CHARSET=utf16;");
        }

        public List<string> ShowTables()
        {
            return adapter.SendQueryList("SHOW TABLES;");
        }

        public bool RemoveTable(string tableName)
        {
            adapter.SendQueryNoData($"DROP TABLE {tableName};");
            return true;
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

        /*
        public void testcode()
        {
            DataSet set = adapter.SendQueryDataSet("SHOW DATABASES");

            DataTable table = set.Tables[0];

            foreach (DataRow row in table.Rows)
            {
                foreach(DataColumn col in table.Columns)
                {
                    Console.Write($"{row[col]}\t");
                }
                Console.WriteLine();
            }
        }
        */
    }
}
