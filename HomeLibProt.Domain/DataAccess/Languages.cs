using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace HomeLibProt.Domain.DataAccess;

public record Language(long Id, string Name);

public static class Languages {
    public static async Task<Language[]> GetLanguagesByNameAsync(DbConnection connection, string[] languageNames) {
        var sql =
            @"
select Id, Name from Languages
where Name in @Names
";

        var languages = await connection.QueryAsync<Language>(sql, new { Names = languageNames });

        return languages.ToArray();
    }

    public static async Task InsertLanguagesAsync(DbConnection connection, string[] languages) {
        var sql =
            @"
insert into
Languages (Name, Include)
values (@Name, 1)
";

        foreach (var l in languages) {
            var _ = await connection.ExecuteAsync(sql, new { Name = l });
        }
    }
}
