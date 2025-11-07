using System.Data.Common;
using System.Threading.Tasks;
using HomeLibProt.Domain.Tests.Entities;

namespace HomeLibProt.Domain.Tests.Utils;

public static class BookSeriesUtils {
    public static async Task SetUpTestData(DbConnection connection) {
        var sql = @"
insert into
Books
    (Id,
    Title,
    FileName,
    Size,
    LibId,
    Deleted,
    Extension,
    Date,
    Folder,
    LibRate)
values
    (1,
    'Title1',
    'File1',
    1,
    'File1',
    0,
    'fb2',
    '2025-11-7',
    'archive1.zip',
    0);

insert into
Series (Id, Name)
values
(1, 'Series 1')
";

        await TestUtils.InsertIntoTestDatabase(connection, sql);
    }

    public static async Task<TestBookSeriesEntity[]> GetTestData(DbConnection connection) {
        var sql = @"
select BookId, SeriesId, SeriesNumber from BookSeries
";

        return await TestUtils.GetFromTestDatabase<TestBookSeriesEntity>(connection, sql);
    }
}
