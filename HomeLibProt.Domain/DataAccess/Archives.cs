using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace HomeLibProt.Domain.DataAccess;

public record Archive(long Id, string Name);

public static class Archives {
    public static async Task<Archive[]> GetArchivesByNameAsync(DbConnection connection, string[] archiveNames) {
        var sql =
            @"
select Id, Name from Archives
where Name in @Names
";

        var archive = await connection.QueryAsync<Archive>(sql, new { Names = archiveNames });

        return archive.ToArray();
    }

    public static async Task InsertArchivesAsync(DbConnection connection, string[] archives) {
        var sql =
            @"
insert into
Archives (Name)
values (@Name)
";

        foreach (var a in archives) {
            var _ = await connection.ExecuteAsync(sql, new { Name = a });
        }
    }
}
