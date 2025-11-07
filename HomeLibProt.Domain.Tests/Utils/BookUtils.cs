using System.Data.Common;
using System.Threading.Tasks;
using HomeLibProt.Domain.Tests.Entities;

namespace HomeLibProt.Domain.Tests.Utils;

public static class BookUtils {
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
