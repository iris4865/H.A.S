using System.Collections.Generic;

namespace DataBase
{
    class TableManager
    {
        IMySqlAdapter adapter;
        List<string> tableList;

        public string[] TableList => tableList.ToArray();

        public TableManager(List<string> tableList)
        {
            this.tableList = tableList;
            adapter = MySqlAdapter.Instance;
        }

        public bool CreateTable(string tableName, string primaryKeyId)
        {
            return adapter.SendQueryNoData(
                $"CREATE TABLE {tableName} (" +
                $"{primaryKeyId} INT NOT NULL AUTO_INCREMENT, " +
                $"PRIMARY KEY({primaryKeyId}) " +
                $") DEFAULT CHARSET=utf16;");
        }

        public bool RemoveTable(string tableName)
        {
            return adapter.SendQueryNoData($"DROP TABLE {tableName};");
        }

        public bool AddColumns(string tableName, MySQLDataType type, string columnId, int size, bool defaultNull)
        {
            string queryString = $"ALTER TABLE {tableName} add {columnId} {type.ToString()}({size.ToString()}) ";
            if (!defaultNull)
                queryString += "NOT NULL;";

            return adapter.SendQueryNoData(queryString);
        }

        /*
         * 여기부터 시작할 것
         */
        public List<string> ShowColumnNames(string tableName)
        {
            return adapter.SendQueryList($"SHOW COLUMNS FROM {tableName};");
        }

        public List<string> ShowColumn(string tableName, string columnName)
        {
            return adapter.SendQueryList($"SELECT {columnName} FROM {tableName}");
        }

        public bool IsField(string tableName, string columnName, string findName)
        {
            return adapter.IsExist($"SELECT * FROM {tableName} WHERE {columnName}='{findName}';");
        }

        public bool CheckLogin(string tableName, string id, string password)
        {
            return adapter.IsExist($"SELECT * FROM {tableName} WHERE id='{id}' AND password='{password}'");
        }

        public bool AddField(string tableName, string id, string password)
        {
            adapter.SendQueryNoData($"INSERT INTO {tableName} (id, password) VALUES ('{id}','{password}');");
            return true;
        }
    }
}
