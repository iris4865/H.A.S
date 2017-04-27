using System.Collections.Generic;
using System.Data;

namespace DataBase
{
    class Table
    {
        DataTable dataTable;

        string name;
        public string Name => name;

        List<string> columnNameList;
        public string[] ColumnNames => columnNameList.ToArray();

        public Table(string name)
        {
            this.name = name;
            columnNameList = new List<string>();

            RefreshData();
        }

        private void RefreshData()
        {
            DataSet dataSet = MySqlAdapter.Instance.SendQueryDataSet($"SHOW COLUMNS FROM {name};");
            dataTable = dataSet.Tables[0];

            columnNameList.Clear();
            foreach (DataColumn column in dataTable.Columns)
                columnNameList.Add(column.ColumnName);
        }

        public bool AddColumn(string columnName, MySQLDataType type, int size, bool defaultNull)
        {
            string sqlQuery = $"ALTER TABLE {name} add {columnName} {type.ToString()}({size.ToString()}) ";
            if (!defaultNull)
                sqlQuery += "NOT NULL;";

            return SendQueryNoData(sqlQuery);
        }

        public bool ChangeColumn(string originColumnName, string newColumnName, MySQLDataType newType, int size, bool defaultNull)
        {
            string sqlQuery = $"ALTER TABLE {name} change {originColumnName} {newColumnName} {newType.ToString()}({size.ToString()}) ";
            if (!defaultNull)
                sqlQuery += "NOT NULL;";

            return SendQueryNoData(sqlQuery);
        }

        public bool RemoveColumn(string columnName)
        {
            return SendQueryNoData($"alter table {name} drop {columnName}");
        }

        private bool SendQueryNoData(string sqlQuery)
        {
            if (MySqlAdapter.Instance.SendQueryNoData(sqlQuery))
            {
                RefreshData();
                return true;
            }

            return false;
        }

        /*
         * 고칠것
         */
        public bool IsField(string tableName, string columnName, string fieldValue)
        {
            return MySqlAdapter.Instance.IsExist($"SELECT * FROM {tableName} WHERE {columnName}='{fieldValue}';");
        }

        public bool CheckLogin(string tableName, string id, string password)
        {
            return MySqlAdapter.Instance.IsExist($"SELECT * FROM {tableName} WHERE id='{id}' AND password='{password}'");
        }

        public bool AddField(string tableName, string id, string password)
        {
            MySqlAdapter.Instance.SendQueryNoData($"INSERT INTO {tableName} (id, password) VALUES ('{id}','{password}');");
            return true;
        }
    }
}