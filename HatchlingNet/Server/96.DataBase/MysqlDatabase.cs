using System.Collections.Generic;

namespace DataBase
{
    class MysqlDatabaseList
    {
        IMySqlAdapter adapter;

        string name;
        public bool Isconnect { get; private set; }

        public MysqlDatabaseList(IMySqlAdapter adapter, string name)
        {
            this.adapter = adapter;
            this.name = name;
            Isconnect = false;
        }
    }
}