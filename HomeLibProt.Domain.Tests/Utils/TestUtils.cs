using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using HomeLibProt.Domain.DataAccess;
using Microsoft.Data.Sqlite;

namespace HomeLibProt.Domain.Tests.Utils;

public static class TestUtils {
    private static readonly string dataSource = ":memory:";

    public static async Task<T> UseTestDatabase<T>(Func<DbConnection, Task<T>> action) {
        using var connection = new SqliteConnection(ConnectionUtils.MakeConnectionString(dataSource));
        return await ConnectionUtils.WithConnectionAsync(connection, async (connection) => {
            await ConnectionUtils.DoInTransactionAsync(connection, async (c) => await DbStructure.CreateFullStructure(c));
            return await action(connection);
        });
    }

    public static async Task<T[]> GetFromTestDatabase<T>(DbConnection connection, string sql) {
        return (await connection.QueryAsync<T>(sql)).ToArray();
    }
}
