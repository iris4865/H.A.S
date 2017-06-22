using MySqlDataBase.Core;
using System;

namespace MySqlDataBase
{
    public class MysqlCommand
    {
        DataBaseInventory inventory;
        DataBase database;
        Table table;

        public string[] DatabaseList => inventory.DatabaseList;

        public bool ConnectMysql(string remoteAddress, string id, string Password)
        {
            MySqlAdapter adapter = MySqlAdapter.Instance;
            bool isConnect = adapter.Connect($"Server={remoteAddress};Uid={id};Pwd={Password};");

            if (isConnect)
                inventory = new DataBaseInventory();

            return isConnect;
        }

        public bool OpenDatabase(string dbName, string tableName)
        {
            if (IsObject(OpenDatabase(dbName)))
                return OpenTable(tableName);

            return false;
        }

        public string[] OpenDatabase(string dbName)
        {
            database = inventory.OpenDataBase(dbName);

            if (IsObject(database))
                return database.TableList;
            return null;
        }

        public bool OpenTable(string tableName)
        {
            table = database.GetTable(tableName);

            if (IsObject(table))
                return true;
            return false;
        }

        private bool IsObject(Object o)
        {
            if (o != null)
                return true;

            return false;
        }

        public bool CheckLogin(string id, string password)
        {
            return table.IsField($"id='{id}' AND password='{password}'");
        }

        public bool SignUp(string id, string password, string lastName, string firstName, string mail, string birthDay)
        {
            return table.AddField(id, password, lastName, firstName, mail, birthDay);
        }
    }
}
