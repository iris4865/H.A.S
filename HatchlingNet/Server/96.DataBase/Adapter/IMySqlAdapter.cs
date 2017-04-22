using System.Collections.Generic;
using System.Data;

namespace DataBase
{
    interface IMySqlAdapter
    {
        bool Connect(string connectionString);
        bool SelectDatabase(string dbName);
        bool IsExist(string sqlQuery);
        bool SendQueryNoData(string sqlQuery);
        DataSet SendQueryDataSet(string sqlQuery);
        List<string> SendQueryList(string sqlQuery);
    }
}
