using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using HomeLibProt.Domain.Tests.Entities;

namespace HomeLibProt.Domain.Tests.Utils;

public static class BookUtils {
    public static async Task<long> Create(DbConnection connection,
                                          string title,
                                          string fileName,
                                          long size,
                                          string libId,
                                          bool deleted,
                                          string extension,
                                          string date,
                                          string folder,
                                          int? libRate,
                                          long languageId) {
        var sql = @"
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
    LibRate,
    LanguageId)
values
    (@Title,
    @FileName,
    @Size,
    @LibId,
    @Deleted,
    @Extension,
    @Date,
    @Folder,
    @LibRate,
    @LanguageId)
returning Id";

        return await connection.QuerySingleAsync<long>(sql, new {
            Title = title,
            FileName = fileName,
            Size = size,
            LibId = libId,
            Deleted = deleted,
            Extension = extension,
            Date = date,
            Folder = folder,
            LibRate = libRate,
            LanguageId = languageId
        });
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
    LibRate,
    LanguageId
from Books
";

        return await TestUtils.GetFromTestDatabase<TestBook>(connection, sql);
    }
}
