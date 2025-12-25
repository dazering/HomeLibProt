using System.Data.Common;
using System.Threading.Tasks;
using HomeLibProt.Domain.Tests.Entities;

namespace HomeLibProt.Domain.Tests.Utils;

public static class BookUtils {
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
    0),
    (2,
    'Title2',
    'File2',
    1,
    'File2',
    0,
    'fb2',
    '2025-11-7',
    'archive2.zip',
    0);
";

        await TestUtils.InsertIntoTestDatabase(connection, sql);
    }

    public static async Task<TestBook[]> GetTestData(DbConnection connection) {
        var sql = @"
select
    Id,
    Title,
    FileName,
    Size,
    LibId,
    Deleted,
    Extension,
    Date,
    Folder,
    LibRate
from Books
";

        return await TestUtils.GetFromTestDatabase<TestBook>(connection, sql);
    }
}
