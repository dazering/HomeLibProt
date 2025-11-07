using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace HomeLibProt.Domain.DataAccess;

public record Genre(long Id, string Key);

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
}
