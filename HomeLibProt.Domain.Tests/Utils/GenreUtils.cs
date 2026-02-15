using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using HomeLibProt.Domain.Tests.Entities;

namespace HomeLibProt.Domain.Tests.Utils;

public static class GenreUtils {
    public static async Task<long> Create(DbConnection connection,
                                          string key,
                                          string name) {
        var sql = @"
insert into
Genres (Key, Name)
values (@Key, @Name)
returning Id";

        return await connection.QuerySingleAsync<long>(sql, new {
            Key = key,
            Name = name
        });
    }

    public static async Task<TestGenre[]> GetTestData(DbConnection connection) {
        var sql = @"
select Id, Key, Name from Genres
";

        return await TestUtils.GetFromTestDatabase<TestGenre>(connection, sql);
    }
}
