using System.Data.Common;
using System.Threading.Tasks;
using Dapper;

namespace HomeLibProt.Domain.DataAccess;

public record BookParam(string Title, string FileName, long Size, string LibId, bool Deleted, string Extension, string Date, string Folder, int? LibRate);

public static class Books {
    public static async Task<long> InsertBookAsync(DbConnection connection, BookParam bookParam) {
        var sql =
            @"
insert into
Books
    (Title,
    FileName,
    Size,
    LibId,
    Deleted,
    Extension,
    Date,
    Folder,
    LibRate)
values
    (@Title,
    @FileName,
    @Size,
    @LibId,
    @Deleted,
    @Extension,
    @Date,
    @Folder,
    @LibRate)
returning Id
";

        return await connection.QuerySingleAsync<long>(sql, bookParam);
    }
}
