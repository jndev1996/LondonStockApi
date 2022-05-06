using System.Data.SqlClient;
using Dapper;
using LondonStockExchange.Sql.Configuration;
using LondonStockExchange.Sql.Repositories.Models;
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

    protected async Task<List<T>> GetByCommandAsync<T>(string sqlCommand, List<SqlCommandParameter>? sqlCommandParameters = null)
    {
        if (string.IsNullOrWhiteSpace(sqlCommand))
        {
            throw new ArgumentException("Sql Command must not be null, empty or whitespace", nameof(sqlCommand));
        }

        if (sqlCommandParameters != null)
        {
            CheckSqlCommandParameters(sqlCommandParameters);
        }
        

        var builder = BuildSqlConnectionString();

        var result = new List<T>();         
        using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand(sqlCommand, connection))
            {
                if (sqlCommandParameters != null)
                {
                    AddSqlCommandParametersToSqlCommand(command, sqlCommandParameters);
                }

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

    protected async Task ExecuteByCommandAsync(string sqlCommand, List<SqlCommandParameter>? sqlCommandParameters = null)
    {
        if (string.IsNullOrWhiteSpace(sqlCommand))
        {
            throw new ArgumentException("Sql Command must not be null, empty or whitespace", nameof(sqlCommand));
        }
        
        if (sqlCommandParameters != null)
        {
            CheckSqlCommandParameters(sqlCommandParameters);
        }

        var builder = BuildSqlConnectionString();

        using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand(sqlCommand, connection))
            {
                if (sqlCommandParameters != null)
                {
                    AddSqlCommandParametersToSqlCommand(command, sqlCommandParameters);
                }

                await command.ExecuteScalarAsync();
            }
        }
    }

    private static void AddSqlCommandParametersToSqlCommand(SqlCommand sqlCommand, List<SqlCommandParameter> sqlCommandParameters)
    {
        foreach (var sqlCommandParameter in sqlCommandParameters)
        {
            sqlCommand.Parameters.Add(sqlCommandParameter.Name, sqlCommandParameter.SqlType);
            sqlCommand.Parameters[sqlCommandParameter.Name].Value = sqlCommandParameter.Value;
        }    
    }

    private static void CheckSqlCommandParameters(List<SqlCommandParameter> sqlCommandParameters)
    {
        
        if (sqlCommandParameters.Any(sqp => String.IsNullOrWhiteSpace(sqp.Name)))
        {
            throw new ArgumentException("Sql Command Parameters must have a name", nameof(sqlCommandParameters));
        }
        
        if (sqlCommandParameters.Any(sqp => sqp.Value == null))
        {
            throw new ArgumentException("Sql Command Parameters Value cannot be null", nameof(sqlCommandParameters));
        }
    }
}