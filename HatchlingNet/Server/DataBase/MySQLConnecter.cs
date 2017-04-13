using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DataBase
{
    partial class MySQLConnecter
    {
        MySQLAdapter mySQL;

        public MySQLConnecter(string remoteAddress, string Password)
        {
            mySQL = new MySQLAdapter();
            mySQL.Connect(remoteAddress, Password);
        }

        public bool Open()
        {
            if (!mySQL.Open())
                return false;

            return true;
        }

        public bool CreateDatabase(string databaseName)
        {
            if (mySQL.IsDataBase())
                return false;
            return mySQL.createDataBase(databaseName);
        }

        public bool ConnectDatabase(string databaseName)
        {
            mySQL.databaseName = databaseName;

            if (mySQL.IsDataBase())
                return mySQL.ConnectDataBase();

            mySQL.databaseName = "";
            return false;
        }

        public List<String> ShowTables()
        {
            return mySQL.ShowTables();
        }

        public bool ConnectTable(string tableName)
        {
            mySQL.tableName = tableName;

            if (mySQL.IsTable())
                return true;

            mySQL.tableName = "";
            return false;
        }

        public bool CreateTable(string tableName, string primaryKeyId)
        {
            if (mySQL.IsTable())
                return false;

            return mySQL.CreateTable(tableName, primaryKeyId);
        }

        public List<string> ShowColumnNames()
        {
            if (mySQL.IsTable())
                return mySQL.ShowColumnNames();

            return null;
        }

        public bool AddColumn(MySQLDataType type, string columnId, int size, bool notNull)
        {
            return mySQL.AddColumns(type, columnId, size, notNull);
        }

        public bool Login(string id, string password)
        {
            return mySQL.CheckLogin(id, password);
        }

        public bool SignUp(string id, string password)
        {
            if (mySQL.IsField("id", id))
                return false;

            return mySQL.AddField(id, password);
        }

        /*
         * Query test method
         */
        public void ExcuteQuery(string query)
        {
            List<string> result;
            try
            {
                result = mySQL.SendQuery(query);
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                return;
            }

            foreach (string line in result)
                Console.WriteLine(line);
        }
        /*
        public bool useTable(string tableName)
        {
            if (!mySQL.IsTable(tableName))
                return false;

            mySQL.tableName = tableName;
            return true;
        }

        public bool removeTable(string tableName)
        {
            if (mySQL.IsTable(tableName))
                return mySQL.removeTable();

            return false;
        }

        public List<string> ReadAllUser()
        {
            return mySQL.showColumn("id");
        }
        */
    }

}