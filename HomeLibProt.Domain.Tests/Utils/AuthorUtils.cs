using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using HomeLibProt.Domain.Tests.Entities;

namespace HomeLibProt.Domain.Tests.Utils;

public static class AuthorUtils {
    public static async Task<long> Create(DbConnection connection,
                                          string fullName,
                                          string lastName,
                                          string firstName,
                                          string middleName) {
        var sql = @"
insert into
Authors (FullName, FirstName, MiddleName, LastName)
values (@FullName, @FirstName, @MiddleName, @LastName)
returning Id";

        return await connection.QuerySingleAsync<long>(sql, new {
            FullName = fullName,
            LastName = lastName,
            FirstName = firstName,
            MiddleName = middleName
        });
    }

    public static async Task<TestAuthor[]> GetTestData(DbConnection connection) {
        var sql = @"
select Id, FullName, FirstName, MiddleName, LastName from Authors
";

        return await TestUtils.GetFromTestDatabase<TestAuthor>(connection, sql);
    }
}
