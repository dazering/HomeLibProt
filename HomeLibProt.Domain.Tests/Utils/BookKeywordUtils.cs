using System.Data.Common;
using System.Threading.Tasks;
using HomeLibProt.Domain.Tests.Entities;

namespace HomeLibProt.Domain.Tests.Utils;

public static class BookKeywordUtils {
    public static async Task<TestBookKeyword[]> GetTestData(DbConnection connection) {
        var sql = @"
select BookId, KeywordId from BookKeywords
";

        return await TestUtils.GetFromTestDatabase<TestBookKeyword>(connection, sql);
    }
}
