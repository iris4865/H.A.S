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

        public void SelectDatabase(string databaseName)
        {
            mySQL.databaseName = databaseName;
        }

        public bool CreateDatabase(string databaseName)
        {
            if (mySQL.IsDataBase(databaseName))
                return false;
            return mySQL.createDataBase(databaseName);
        }

        public bool ConnectDatabase()
        {
            if (mySQL.IsDataBase())
                return mySQL.ConnectDataBase();
            return false;
        }

        public List<String> ShowTables()
        {
            return mySQL.ShowTables();
        }

        public void SelectTable(string tableName)
        {
            mySQL.tableName = tableName;
        }

        public bool CreateTable(string primaryKeyId)
        {
            if (mySQL.IsTable())
                return false;

            return mySQL.CreateTable(primaryKeyId);
        }

        public List<string> ShowColumns()
        {
            if (mySQL.IsTable())
                return mySQL.showColumns();

            return null;
        }

        public bool addColumn(MySQLDataType type, string columnId, int size, bool notNull)
        {
            return true;
        }

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

        public bool Login(string id, string password)
        {
            return true;
        }

        public bool SignUp(string id, string password)
        {

            return true;
        }
        */
    }

}