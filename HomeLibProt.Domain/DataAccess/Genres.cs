using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace HomeLibProt.Domain.DataAccess;

public record Genre(long Id, string Key);

public record GenreEntityParam(long Id, string Key, string Name);

public static class Genres {
    public static async Task<Genre[]> GetGenresByKeyAsync(DbConnection connection, string[] genreKeys) {
        var sql =
            @"
select Id, Key from Genres
where Key in @Keys
";

        var genres = await connection.QueryAsync<Genre>(sql, new { Keys = genreKeys });

        return genres.ToArray();
    }

    public static async Task InsertGenresAsync(DbConnection connection, string[] genresKeys) {
        var sql =
            @"
insert into
Genres (Key)
values (@Key)
";

        foreach (var k in genresKeys) {
            var _ = await connection.ExecuteAsync(sql, new { Key = k });
        }
    }

    public static async Task InsertGenreEntityAsync(DbConnection connection, GenreEntityParam genre) {
        var sql =
            @"
insert into
Genres (Id, Key, Name)
values (@Id, @Key, @Name)
";

        await connection.ExecuteAsync(sql, genre);
    }

    public static async Task<string[]> GetGenreKeysByBookIdAsync(DbConnection connection, long bookId) {
        var sql =
            @"
select
  g.Key
from Genres g
inner join BookGenres bg on bg.GenreId = g.Id
where bg.BookId = @BookId
";

        var genreKeys = await connection.QueryAsync<string>(sql, new { BookId = bookId });

        return genreKeys.ToArray();
    }
}
