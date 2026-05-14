using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using HomeLibProt.Domain.Tests.Entities;

namespace HomeLibProt.Domain.Tests.Utils;

public static class BookGenreUtils {
    public static async Task<long> Create(DbConnection connection,
                                          long bookId,
                                          long genreId) {
        var sql = @"
insert into
BookGenres (BookId, GenreId)
values (@BookId, @GenreId)
returning Id";

        return await connection.QuerySingleAsync<long>(sql, new {
            BookId = bookId,
            GenreId = genreId
        });
    }

    public static async Task<TestBookGenre[]> GetTestData(DbConnection connection) {
        var sql = @"
select BookId, GenreId from BookGenres
";

        return await TestUtils.GetFromTestDatabase<TestBookGenre>(connection, sql);
    }
}
