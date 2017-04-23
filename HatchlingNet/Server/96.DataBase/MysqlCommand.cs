using System;
using System.Collections.Generic;
using System.Data;

namespace DataBase
{
    public class MysqlCommand
    {
        DataBaseManager database;
        TableManager table;

        public MysqlCommand()
        {
            database = new DataBaseManager();
        }

        public bool ConnectMysql(string remoteAddress, string id, string Password)
        {
            return database.ConnectMysql(remoteAddress, id, Password);
        }

        public bool OpenDatabase(string dbName)
        {
            table = database.OpenDataBase(dbName);
            return false;
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
