using System.Data;

namespace LondonStockExchange.Sql.Repositories.Models;

public class SqlCommandParameter
{
    public string Name { get; set; }
    public SqlDbType SqlType { get; set; }
    public object Value { get; set; }
}