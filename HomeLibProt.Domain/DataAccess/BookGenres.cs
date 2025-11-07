using System.Data.Common;
using System.Threading.Tasks;
using Dapper;

namespace HomeLibProt.Domain.DataAccess;

public record BookGenreParam(long BookId, long GenreId);

public static class BookGenres {
    public static async Task InsertBookGenresAsync(DbConnection connection, BookGenreParam[] bookGenreParams) {
        var sql =
            @"
insert into
BookGenres (BookId, GenreId)
values (@BookId, @GenreId)
";

        foreach (var bgp in bookGenreParams) {
            var _ = await connection.ExecuteAsync(sql, bgp);
        }
    }
}
