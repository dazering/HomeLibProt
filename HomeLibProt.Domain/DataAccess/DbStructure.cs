using System.Data.Common;
using System.Threading.Tasks;
using Dapper;

namespace HomeLibProt.Domain.DataAccess;

public static class DbStructure {
    private static readonly string dropExistingInpxTablesSql = @"
drop table if exists BookGenres;
drop table if exists Genres;
drop table if exists BookSeries;
drop table if exists Series;
drop table if exists BookKeywords;
drop table if exists Keywords;
drop table if exists Authorships;
drop table if exists Authors;
drop table if exists Books;
drop table if exists Languages;
";

    private static readonly string dropExistingAHSTablesSql = @"
drop table if exists AuthorHierarchicalSearchResults;
drop table if exists AuthorHierarchicalSearchNodes;
";

    private static readonly string createAuthorsSql = @"
create table Authors(
    Id integer primary key autoincrement,
    FullName text not null,
    LastName text not null,
    FirstName text not null,
    MiddleName text not null,

    unique (FullName)
);

create index idx_authors_full_names on Authors (FullName);
";

    private static readonly string createBooksSql = @"
create table Books (
    Id integer primary key,
    Title text not null,
    FileName text not null,
    Size integer not null,
    LibId text not null,
    Deleted integer not null,
    Extension text not null,
    Date text not null,
    Folder text not null,
    LibRate integer null,
    LanguageId integer references Languages(Id)
);
";

    private static readonly string createAuthorshipsSql = @"
create table Authorships (
    AuthorId integer References Authors(Id),
    BookId integer references Books(Id),
    unique(AuthorId, BookId)
);
";

    private static readonly string createSeriesSql = @"
create table Series (
    Id integer primary key autoincrement,
    Name text not null
);

create index idx_series_names on Series (Name);
";

    private static readonly string createBookSeriesSql = @"
create table BookSeries (
    BookId integer References Books(Id),
    SeriesId integer References Series(Id),
    SeriesNumber integer not null
);
";

    private static readonly string createGenresSql = @"
create table Genres (
    Id integer primary key autoincrement,
    Key text unique,
    Name text
);

create index idx_genres_keys on Genres (Key);
";

    private static readonly string createBookGenresSql = @"
create table BookGenres (
    BookId integer References Books(Id),
    GenreId integer References Genres(Id)
);
";

    private static readonly string createKeywordsSql = @"
create table Keywords (
    Id integer primary key autoincrement,
    Name text not null
);

create index idx_keywords_names on Keywords (Name);
";

    private static readonly string createBookKeywordsSql = @"
create table BookKeywords (
    BookId integer References Books(Id),
    KeywordId integer References Keywords(Id)
);
";

    private static readonly string createAuthorHierarchicalSearchNodesSql = @"
create table AuthorHierarchicalSearchNodes (
    Id integer primary key,
    Letters text not null,
    PreviousId integer null references AuthorHierarchicalSearchNodes(Id),
    AuthorsCount integer not null
);

create index idx_author_hierarchical_search_nodes_previous_ids on AuthorHierarchicalSearchNodes (PreviousId);
";

    private static readonly string createAuthorHierarchicalSearchResultsSql = @"
create table AuthorHierarchicalSearchResults (
    Id integer primary key,
    NodeId integer not null references AuthorHierarchicalSearchNodes(Id),
    AuthorId integer not null references Authors(Id),

    unique (NodeId, AuthorId)
);

create index idx_author_hierarchical_search_results_node_ids on AuthorHierarchicalSearchResults (NodeId);
";

    private static readonly string createLanguagesSql = @"
create table Languages (
    Id integer primary key,
    Name text not null,
    Include integer not null,

    unique (Name)
);
";

    private static async Task ExecuteSqlAsync(DbConnection connection, string sql) {
        var _ = await connection.ExecuteAsync(sql);
    }

    public static async Task CreateFullStructure(DbConnection connection) {
        await ExecuteSqlAsync(connection, dropExistingAHSTablesSql);
        await ExecuteSqlAsync(connection, dropExistingInpxTablesSql);
        await ExecuteSqlAsync(connection, createAuthorsSql);
        await ExecuteSqlAsync(connection, createLanguagesSql);
        await ExecuteSqlAsync(connection, createBooksSql);
        await ExecuteSqlAsync(connection, createAuthorshipsSql);
        await ExecuteSqlAsync(connection, createSeriesSql);
        await ExecuteSqlAsync(connection, createBookSeriesSql);
        await ExecuteSqlAsync(connection, createGenresSql);
        await ExecuteSqlAsync(connection, createBookGenresSql);
        await ExecuteSqlAsync(connection, createKeywordsSql);
        await ExecuteSqlAsync(connection, createBookKeywordsSql);
        await ExecuteSqlAsync(connection, createAuthorHierarchicalSearchNodesSql);
        await ExecuteSqlAsync(connection, createAuthorHierarchicalSearchResultsSql);
    }

    public static async Task CreateAHSStructure(DbConnection connection) {
        await ExecuteSqlAsync(connection, dropExistingAHSTablesSql);
        await ExecuteSqlAsync(connection, createAuthorHierarchicalSearchNodesSql);
        await ExecuteSqlAsync(connection, createAuthorHierarchicalSearchResultsSql);
    }
}
