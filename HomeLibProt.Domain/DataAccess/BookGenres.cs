using System.Data.Common;
using System.Threading.Tasks;
using Dapper;

namespace HomeLibProt.Domain.DataAccess;

public record BookGenreParam(long BookId, long GenreId);

public enum BookGenreCheckResult {
    Duplicate = 0,
    AllExsists,
    OnlyBook,
    OnlyGenre,
    NoRecords
}

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

    public static async Task<BookGenreCheckResult> CheckIfBookGenreExistsAsync(DbConnection connection, BookGenreParam bookGenreParam) {
        var sql =
            @"
with
    BookGenre as (select max(1) as BookGenreExsists from BookGenres where BookId = @BookId and GenreId = @GenreId),
    Book as (select max(1) as BookExsists from Books where Id = @BookId),
    Genre as (select max(1) as GenreExsists from Genres where Id = @GenreId)
select
    case
        when BookGenre.BookGenreExsists then 0
        when Book.BookExsists and Genre.GenreExsists then 1
        when Book.BookExsists then 2
        when Genre.GenreExsists then 3
        else 4
    end as Result
from Book, Genre, BookGenre
";

        return await connection.QuerySingleAsync<BookGenreCheckResult>(sql, new { BookId = bookGenreParam.BookId, GenreId = bookGenreParam.GenreId });
    }
}
