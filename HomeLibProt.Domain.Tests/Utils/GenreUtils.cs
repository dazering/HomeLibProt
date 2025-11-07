using System.Data.Common;
using System.Threading.Tasks;
using HomeLibProt.Domain.Tests.Entities;

namespace HomeLibProt.Domain.Tests.Utils;

public static class GenreUtils {
    public static async Task SetUpTestData(DbConnection connection) {
        var sql = @"
insert into
Genres (Id, Key, Name)
values
(1, 'genre1', 'Genre 1'),
(2, 'genre2', 'Genre 2')
";

        await TestUtils.InsertIntoTestDatabase(connection, sql);
    }

    public static async Task<TestGenre[]> GetTestData(DbConnection connection) {
        var sql = @"
select Id, Key, Name from Genres
";

        return await TestUtils.GetFromTestDatabase<TestGenre>(connection, sql);
    }
}
