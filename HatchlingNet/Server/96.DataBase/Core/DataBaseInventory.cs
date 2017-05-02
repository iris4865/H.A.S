using System.Collections.Generic;
using System.Linq;

namespace MySQL.Core
{
    class DataBaseInventory
    {
        MySqlAdapter adapter = MySqlAdapter.Instance;
        private List<string> databaseList;

        public string[] DatabaseList => databaseList.ToArray();

        public DataBaseInventory()
        {
            databaseList = adapter.SendQueryList("SHOW DATABASES");
            databaseList = databaseList.Select(item => item = item.Trim()).ToList();
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

        public DataBase OpenDataBase(string name)
        {
            if (databaseList.Contains(name))
            {
                adapter.SelectDatabase(name);
                return new DataBase(name);
            }

            return null;
        }
    }
}