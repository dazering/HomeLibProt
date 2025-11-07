using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace HomeLibProt.Domain.DataAccess;

public record Keyword(long Id, string Name);

public static class Keywords {
    public static async Task<Keyword[]> GetKeywordsByNameAsync(DbConnection connection, string[] keywordNames) {
        var sql =
            @"
select Id, Name from Keywords
where Name in @Names
";

        var keywords = await connection.QueryAsync<Keyword>(sql, new { Names = keywordNames });

        return keywords.ToArray();
    }

    public static async Task InsertKeywordsAsync(DbConnection connection, string[] keywords) {
        var sql =
            @"
insert into
Keywords (Name)
values (@Name)
";

        foreach (var k in keywords) {
            var _ = await connection.ExecuteAsync(sql, new { Name = k });
        }
    }
}
