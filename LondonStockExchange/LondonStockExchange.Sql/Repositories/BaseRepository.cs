using System.Data.SqlClient;
using Dapper;
using LondonStockExchange.Sql.Configuration;
using Microsoft.Extensions.Options;


namespace LondonStockExchange.Sql.Repositories;
using System;


public class BaseRepository
{
    private readonly SqlDatabaseConfiguration _sqlDatabaseConfiguration;

    protected BaseRepository(IOptions<SqlDatabaseConfiguration> sqlDatabaseConfiguration)
    {
        _sqlDatabaseConfiguration = sqlDatabaseConfiguration.Value;
    }

    private SqlConnectionStringBuilder BuildSqlConnectionString()
    {
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

        builder.DataSource = _sqlDatabaseConfiguration.DataSource; 
        builder.UserID = _sqlDatabaseConfiguration.UserId;  
        builder.Password = _sqlDatabaseConfiguration.Password;
        builder.InitialCatalog = _sqlDatabaseConfiguration.InitialCatalog;

        return builder;
    }

    protected async Task<List<T>> GetByCommandAsync<T>(string sqlCommand)
    {
        if (string.IsNullOrWhiteSpace(sqlCommand))
        {
            throw new ArgumentException("Sql Command must not be null, empty or whitespace", nameof(sqlCommand));
        }

        var builder = BuildSqlConnectionString();

        var result = new List<T>();         
        using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand(sqlCommand, connection))
            {
                await using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    var parser = reader.GetRowParser<T>(typeof(T));

                    while (reader.Read())
                    {
                        result.Add(parser(reader));
                    }
                }
            }
        }

        return result;
    }

    protected async Task ExecuteByCommandAsync(string sqlCommand)
    {
        if (string.IsNullOrWhiteSpace(sqlCommand))
        {
            throw new ArgumentException("Sql Command must not be null, empty or whitespace", nameof(sqlCommand));
        }

        var builder = BuildSqlConnectionString();

        using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand(sqlCommand, connection))
            {
                await connection.ExecuteAsync(sqlCommand);
            }
        }
    }
}