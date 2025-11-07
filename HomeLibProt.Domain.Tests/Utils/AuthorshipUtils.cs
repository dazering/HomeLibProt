using System.Data.Common;
using System.Threading.Tasks;
using HomeLibProt.Domain.Tests.Entities;

namespace HomeLibProt.Domain.Tests.Utils;

public static class AuthorshipUtils {
    public static async Task SetUpTestData(DbConnection connection) {
        var sql = @"
insert into
Authors (Id, FullName, FirstName, MiddleName, LastName)
values
(1, 'A A A', 'A', 'A', 'A');

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
    0)

";

        await TestUtils.InsertIntoTestDatabase(connection, sql);
    }

    public static async Task<TestAuthorship[]> GetTestData(DbConnection connection) {
        var sql = @"
select BookId, AuthorId from Authorships
";

        return await TestUtils.GetFromTestDatabase<TestAuthorship>(connection, sql);
    }
}
