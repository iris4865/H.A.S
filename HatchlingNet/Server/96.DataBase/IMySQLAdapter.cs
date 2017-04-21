using System.Data;

namespace DataBase
{
    public interface IMySqlAdapter
    {
        bool Connect(string connectionString);
        bool SendQueryNoData(string sqlQuery);
        DataSet SendQuery(string sqlQuery);
    }
}
