using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using HomeLibProt.Domain.Tests.Entities;

namespace HomeLibProt.Domain.Tests.Utils;

public static class BookKeywordUtils {
    public static async Task<long> Create(DbConnection connection,
                                          long bookId,
                                          long keywordId) {
        var sql = @"
insert into
BookKeywords (BookId, KeywordId)
values (@BookId, @KeywordId)
returning Id";

        return await connection.QuerySingleAsync<long>(sql, new {
            BookId = bookId,
            KeywordId = keywordId
        });
    }

    public static async Task<TestBookKeyword[]> GetTestData(DbConnection connection) {
        var sql = @"
select BookId, KeywordId from BookKeywords
";

        return await TestUtils.GetFromTestDatabase<TestBookKeyword>(connection, sql);
    }
}
