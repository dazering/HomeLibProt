using System.Data.Common;
using System.Threading.Tasks;
using HomeLibProt.Domain.Tests.Entities;

namespace HomeLibProt.Domain.Tests.Utils;

public static class BookSeriesUtils {
    public static async Task<TestBookSeriesEntity[]> GetTestData(DbConnection connection) {
        var sql = @"
select BookId, SeriesId, SeriesNumber from BookSeries
";

        return await TestUtils.GetFromTestDatabase<TestBookSeriesEntity>(connection, sql);
    }
}
