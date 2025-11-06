using System;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace HomeLibProt.Domain.DataAccess;

public static class ConnectionUtils {
    public static string MakeConnectionString(string dbName) => $"DataSource={dbName}";

    public static DbConnection MakeConnection(string connectionString) => new SqliteConnection(connectionString);

    public static async Task DoInTransactionAsync(DbConnection connection, Func<DbConnection, Task> action) {
        await using var transaction = await connection.BeginTransactionAsync();
        await action(connection);
        await transaction.CommitAsync();
    }

    public static async Task<T> DoInTransactionAsync<T>(DbConnection connection, Func<DbConnection, Task<T>> action) {
        await using var transaction = await connection.BeginTransactionAsync();
        var result = await action(connection);
        await transaction.CommitAsync();
        return result;
    }

    public static async Task WithConnectionAsync(DbConnection connection, Func<DbConnection, Task> action) {
        await connection.OpenAsync();
        await action(connection);
        await connection.CloseAsync();
    }

    public static async Task<T> WithConnectionAsync<T>(DbConnection connection, Func<DbConnection, Task<T>> action) {
        await connection.OpenAsync();
        var result = await action(connection);
        await connection.CloseAsync();
        return result;
    }
}
