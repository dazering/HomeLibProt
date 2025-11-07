using System.Data.Common;
using System.Threading.Tasks;
using HomeLibProt.Domain.Tests.Entities;

namespace HomeLibProt.Domain.Tests.Utils;

public static class SeriesUtils {
    public static async Task SetUpTestData(DbConnection connection) {
        var sql = @"
insert into
Series (Id, Name)
values
(1, 'Series 1'),
(2, 'Series 2')
";

        await TestUtils.InsertIntoTestDatabase(connection, sql);
    }

    public static async Task<TestSeriesEntity[]> GetTestData(DbConnection connection) {
        var sql = @"
select Id, Name from Series
";

        return await TestUtils.GetFromTestDatabase<TestSeriesEntity>(connection, sql);
    }
}
