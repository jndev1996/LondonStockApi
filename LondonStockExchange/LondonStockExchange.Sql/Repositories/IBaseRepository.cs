namespace LondonStockExchange.Sql.Repositories;

public interface IBaseRepository
{
    public List<T> GetByCommand<T>(string sqlCommand);
}