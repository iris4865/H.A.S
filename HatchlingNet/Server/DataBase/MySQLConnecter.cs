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

        public void excuteQuery(string query)
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
            {
                Console.WriteLine(line);
            }
        }
        /*
        public bool CreateTable(string tableName, string primaryKeyId)
        {
            if (mySQL.IsTable(tableName))
                return false;

            return mySQL.CreateTable(tableName, primaryKeyId);
        }

        public bool useTable(string tableName)
        {
            if (!mySQL.IsTable(tableName))
                return false;

            mySQL.tableName = tableName;
            return true;
        }

        public List<String> showTables()
        {
            return mySQL.showTables();
        }

        public bool removeTable(string tableName)
        {
            if (mySQL.IsTable(tableName))
                return mySQL.removeTable();

            return false;
        }

        public bool addColumn(MySQLDataType type, string columnId, int size, bool notNull)
        {
            return true;
        }

        public List<string> showColumns()
        {
            if (mySQL.IsTable(mySQL.tableName))
                return mySQL.showColumns();

            return null;
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