using System;
using System.Collections.Generic;

namespace DataBase
{
    class DataBaseManager
    {
        IMySqlAdapter adapter;
        public bool Isconnect { get; private set; }
        private List<string> databaseList;

        public string[] DatabaseList => databaseList.ToArray();

        public DataBaseManager()
        {
            adapter = MySqlAdapter.Instance;
            Isconnect = false;
        }

        public bool ConnectMysql(string remoteAddress, string id, string Password)
        {
            Isconnect = adapter.Connect($"Server={remoteAddress};Uid={id};Pwd={Password};");

            if (Isconnect)
                databaseList = adapter.SendQueryList("SHOW DATABASES");

            return Isconnect;
        }

        public bool CreateDataBase(string dbName)
        {
            if (adapter.SendQueryNoData($"CREATE DATABASE {dbName}"))
            {
                databaseList.Add(dbName);
                return true;
            }

            return false;
        }

        public bool DropDataBase(string dbName)
        {
            if (adapter.SendQueryNoData($"DROP DATABASE {dbName}"))
            {
                databaseList.Remove(dbName);
                return true;
            }

            return false;
        }

        public TableManager OpenDataBase(string name)
        {
            if (databaseList.Contains(name))
            {
                adapter.SelectDatabase(name);
                return new TableManager(adapter.SendQueryList("SHOW TABLES"));
            }

            return null;
        }
    }
}