using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace HomeLibProt.Domain.DataAccess;

public record SeriesEntity(long Id, string Name);

public static class Series {
    public static async Task<SeriesEntity[]> GetSeriesByNameAsync(DbConnection connection, string[] seriesNames) {
        var sql =
            @"
select Id, Name from Series
where Name in @Names
";

        var keywords = await connection.QueryAsync<SeriesEntity>(sql, new { Names = seriesNames });

        return keywords.ToArray();
    }

    public static async Task InsertSeriesAsync(DbConnection connection, string[] series) {
        var sql =
            @"
insert into
Series (Name)
values (@Name)
";

        foreach (var s in series) {
            var _ = await connection.ExecuteAsync(sql, new { Name = s });
        }
    }
}
