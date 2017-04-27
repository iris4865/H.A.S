using System.Collections.Generic;

namespace DataBase
{
    class DataBase
    {
        string name;

        List<Table> tableList;
        public Table[] TableList => tableList.ToArray();

        public DataBase(string name)
        {
            this.name = name;
            tableList = new List<Table>();

            RefreshData();
        }

        private void RefreshData()
        {
            List<string> tablesName = MySqlAdapter.Instance.SendQueryList("SHOW TABLES");

            List<Table> newTableList = new List<Table>() ;

            foreach (string name in tablesName)
            {
                Table table = tableList.Find(item => item.Name.Equals(name));
                if (table != null)
                {
                    newTableList.Add(table);
                    tableList.Remove(table);
                }
                else
                    newTableList.Add(new Table(name));
            }

            tableList = newTableList;
        }

        public bool CreateTable(string tableName, string primaryKeyId)
        {
            return SendQueryNoData(
                $"CREATE TABLE {tableName} (" +
                $"{primaryKeyId} INT NOT NULL AUTO_INCREMENT, " +
                $"PRIMARY KEY({primaryKeyId}) " +
                $") DEFAULT CHARSET=utf16;");
        }

        public bool RemoveTable(string tableName)
        {
            return SendQueryNoData($"DROP TABLE {tableName};");
        }

        private bool SendQueryNoData(string sqlQuery)
        {
            if(MySqlAdapter.Instance.SendQueryNoData(sqlQuery))
            {
                RefreshData();
                return true;
            }
            return false;
        }
    }
}
