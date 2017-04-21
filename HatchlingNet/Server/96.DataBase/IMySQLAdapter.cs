using System.Data;

namespace DataBase
{
    interface IMySQLAdapter
    {
        bool Connect(string remoteAddress, string Password);
        bool SendQueryNoData(string sqlQuery);
        DataSet SendQuery(string sqlQuery);
    }
}
