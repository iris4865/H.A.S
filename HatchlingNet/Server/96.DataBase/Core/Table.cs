using System.Collections.Generic;
using System.Data;

namespace MySqlDataBase.Core
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
            foreach (DataRow row in dataTable.Rows)
                columnNameList.Add((string)row[0]);
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

        public bool AddField(string id, string password, string lastName, string firstName, string mail, string birthDay)
        {
            return SendQueryNoData($"INSERT INTO {name} (id, password, lastName, firstName, mail, birthDay) VALUES ('{id}','{password}','{lastName}','{firstName}','{mail}','{birthDay}');");
        }

        public bool IsField(string whereQuery)
        {
            return MySqlAdapter.Instance.IsExist($"SELECT * FROM {name} WHERE {whereQuery}");
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
    }
}