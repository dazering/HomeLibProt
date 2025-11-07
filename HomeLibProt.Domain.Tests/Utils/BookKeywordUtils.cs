using System.Data.Common;
using System.Threading.Tasks;
using HomeLibProt.Domain.Tests.Entities;

namespace HomeLibProt.Domain.Tests.Utils;

public static class BookKeywordUtils {
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
Keywords (Id, Name)
values
(1, 'Keyword 1')
";

        await TestUtils.InsertIntoTestDatabase(connection, sql);
    }

    public static async Task<TestBookKeyword[]> GetTestData(DbConnection connection) {
        var sql = @"
select BookId, KeywordId from BookKeywords
";

        return await TestUtils.GetFromTestDatabase<TestBookKeyword>(connection, sql);
    }
}
