using System.Data.Common;
using System.Threading.Tasks;
using Dapper;

namespace HomeLibProt.Domain.DataAccess;

public record BookKeywordParam(long BookId, long KeywordId);

public static class BookKeywords {
    public static async Task InsertBookKeywordsAsync(DbConnection connection, BookKeywordParam[] bookKeywordParams) {
        var sql =
            @"
insert into
BookKeywords (BookId, KeywordId)
values (@BookId, @KeywordId)
";

        foreach (var bkp in bookKeywordParams) {
            var _ = await connection.ExecuteAsync(sql, bkp);
        }
    }
}
