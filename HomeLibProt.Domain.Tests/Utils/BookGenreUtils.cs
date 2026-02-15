using System.Data.Common;
using System.Threading.Tasks;
using HomeLibProt.Domain.Tests.Entities;

namespace HomeLibProt.Domain.Tests.Utils;

public static class BookGenreUtils {
    public static async Task<TestBookGenre[]> GetTestData(DbConnection connection) {
        var sql = @"
select BookId, GenreId from BookGenres
";

        return await TestUtils.GetFromTestDatabase<TestBookGenre>(connection, sql);
    }
}
