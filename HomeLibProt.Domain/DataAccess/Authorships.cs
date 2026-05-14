using System.Data.Common;
using System.Threading.Tasks;
using Dapper;

namespace HomeLibProt.Domain.DataAccess;

public record AuthorshipParam(long BookId, long AuthorId);

public enum AuthorshipsCheckResult {
    Duplicate = 0,
    AllExsists,
    OnlyBook,
    OnlyAuthor,
    NoRecords
}

public static class Authorships {
    public static async Task InsertAuthorshipsAsync(DbConnection connection, AuthorshipParam[] authorshipParams) {
        var sql =
            @"
insert into
Authorships (BookId, AuthorId)
values (@BookId, @AuthorId)
";

        foreach (var asp in authorshipParams) {
            var _ = await connection.ExecuteAsync(sql, asp);
        }
    }

    public static async Task<AuthorshipsCheckResult> CheckIfAuthorshipsExistsAsync(DbConnection connection, AuthorshipParam authorshipParam) {
        var sql =
            @"
with
    Authorship as (select max(1) as AuthorshipExsists from Authorships where BookId = @BookId and AuthorId = @AuthorId),
    Book as (select max(1) as BookExsists from Books where Id = @BookId),
    Author as (select max(1) as AuthorExsists from Authors where Id = @AuthorId)
select
    case
        when Authorship.AuthorshipExsists then 0
        when Book.BookExsists and Author.AuthorExsists then 1
        when Book.BookExsists then 2
        when Author.AuthorExsists then 3
        else 4
    end as Result
from Book, Author, Authorship
";

        return await connection.QuerySingleAsync<AuthorshipsCheckResult>(sql, new { BookId = authorshipParam.BookId, AuthorId = authorshipParam.AuthorId });
    }
}
