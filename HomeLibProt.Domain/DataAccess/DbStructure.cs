using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;

namespace HomeLibProt.Domain.DataAccess;

public static class DbStructure {
    private static readonly string createAuthorsSql = @"
create table Authors(
    Id integer primary key autoincrement,
    FullName text not null,
    LastName text not null,
    FirstName text not null,
    MiddleName text not null,

    unique (FullName)
);
";

    private static readonly string createIndexAuthorsForFullNameSql = @"
create index idx_authors_full_names on Authors (FullName);
";

    private static readonly string dropAuthorsSql = @"
drop table if exists Authors;
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
    ArchiveId integer not null references Archives(Id),
    LibRate integer null,
    LanguageId integer not null references Languages(Id)
);
";

    private static readonly string dropBooksSql = @"
drop table if exists Books;
";

    private static readonly string createAuthorshipsSql = @"
create table Authorships (
    Id integer primary key,
    AuthorId integer not null references Authors(Id),
    BookId integer not null references Books(Id),
    unique(AuthorId, BookId)
);
";

    private static readonly string dropAuthorshipsSql = @"
drop table if exists Authorships;
";

    private static readonly string createSeriesSql = @"
create table Series (
    Id integer primary key autoincrement,
    Name text not null
);
";

    private static readonly string createIndexSeriesForNameSql = @"
create index idx_series_names on Series (Name);
";

    private static readonly string dropSeriesSql = @"
drop table if exists Series;
";

    private static readonly string createBookSeriesSql = @"
create table BookSeries (
    Id integer primary key,
    BookId integer not null references Books(Id),
    SeriesId integer not null references Series(Id),
    SeriesNumber integer not null
);
";

    private static readonly string dropBookSeriesSql = @"
drop table if exists BookSeries;
";

    private static readonly string createGenresSql = @"
create table Genres (
    Id integer primary key autoincrement,
    Key text unique,
    Name text
);
";

    private static readonly string createIndexGenresForKeySql = @"
create index idx_genres_keys on Genres (Key);
";

    private static readonly string dropGenresSql = @"
drop table if exists Genres;
";

    private static readonly string createBookGenresSql = @"
create table BookGenres (
    Id integer primary key,
    BookId integer not null references Books(Id),
    GenreId integer not null references Genres(Id)
);
";

    private static readonly string dropBookGenresSql = @"
drop table if exists BookGenres;
";

    private static readonly string createKeywordsSql = @"
create table Keywords (
    Id integer primary key autoincrement,
    Name text not null
);
";

    private static readonly string createIndexKeywordsForNameSql = @"
create index idx_keywords_names on Keywords (Name);
";

    private static readonly string dropKeywordsSql = @"
drop table if exists Keywords;
";

    private static readonly string createBookKeywordsSql = @"
create table BookKeywords (
    Id integer primary key,
    BookId integer not null references Books(Id),
    KeywordId integer not null References Keywords(Id)
);
";

    private static readonly string dropBookKeywordsSql = @"
drop table if exists BookKeywords;
";

    private static readonly string createAuthorHierarchicalSearchNodesSql = @"
create table AuthorHierarchicalSearchNodes (
    Id integer primary key,
    Letters text not null,
    PreviousId integer null references AuthorHierarchicalSearchNodes(Id),
    AuthorsCount integer not null
);
";

    private static readonly string createIndexAuthorHierarchicalSearchNodesForPreviousIdSql = @"
create index idx_author_hierarchical_search_nodes_previous_ids on AuthorHierarchicalSearchNodes (PreviousId);
";

    private static readonly string dropAuthorHierarchicalSearchNodesSql = @"
drop table if exists AuthorHierarchicalSearchNodes;
";

    private static readonly string createAuthorHierarchicalSearchResultsSql = @"
create table AuthorHierarchicalSearchResults (
    Id integer primary key,
    NodeId integer not null references AuthorHierarchicalSearchNodes(Id),
    AuthorId integer not null references Authors(Id),

    unique (NodeId, AuthorId)
);
";

    private static readonly string createIndexAuthorHierarchicalSearchResultsForNodeIdSql = @"
create index idx_author_hierarchical_search_results_node_ids on AuthorHierarchicalSearchResults (NodeId);
";

    private static readonly string dropAuthorHierarchicalSearchResultsSql = @"
drop table if exists AuthorHierarchicalSearchResults;
";

    private static readonly string createLanguagesSql = @"
create table Languages (
    Id integer primary key,
    Name text not null,
    Include integer not null,

    unique (Name)
);
";

    private static readonly string createIndexLanguagesForNameSql = @"
create index idx_languages_name on Languages (Name);
";

    private static readonly string dropLanguagesSql = @"
drop table if exists Languages;
";

    private static readonly string createArchivesSql = @"
create table Archives (
    Id integer primary key,
    Name text not null,

    unique (Name)
);
";

    private static readonly string dropArchivesSql = @"
drop table if exists Archives;
";

    private static IEnumerable<string> getDropImportInpxCommands(bool fullCreation) {
        yield return dropAuthorHierarchicalSearchResultsSql;
        yield return dropAuthorHierarchicalSearchNodesSql;
        yield return dropBookGenresSql;
        yield return dropGenresSql;
        yield return dropBookSeriesSql;
        yield return dropSeriesSql;
        yield return dropBookKeywordsSql;
        yield return dropKeywordsSql;
        yield return dropAuthorshipsSql;
        yield return dropAuthorsSql;
        yield return dropBooksSql;
        if (fullCreation) {
            yield return dropLanguagesSql;
        }
        yield return dropArchivesSql;
    }

    private static IEnumerable<string> getDropImportAHSCommands() {
        yield return dropAuthorHierarchicalSearchResultsSql;
        yield return dropAuthorHierarchicalSearchNodesSql;
    }

    private static IEnumerable<string> getCreateImportInpxCommands(bool fullCreation) {
        yield return createAuthorsSql;
        yield return createIndexAuthorsForFullNameSql;
        if (fullCreation) {
            yield return createLanguagesSql;
            yield return createIndexLanguagesForNameSql;
        }
        yield return createArchivesSql;
        yield return createBooksSql;
        yield return createAuthorshipsSql;
        yield return createSeriesSql;
        yield return createIndexSeriesForNameSql;
        yield return createBookSeriesSql;
        yield return createGenresSql;
        yield return createIndexGenresForKeySql;
        yield return createBookGenresSql;
        yield return createKeywordsSql;
        yield return createIndexKeywordsForNameSql;
        yield return createBookKeywordsSql;
        yield return createAuthorHierarchicalSearchNodesSql;
        yield return createIndexAuthorHierarchicalSearchNodesForPreviousIdSql;
        yield return createAuthorHierarchicalSearchResultsSql;
        yield return createIndexAuthorHierarchicalSearchResultsForNodeIdSql;
    }

    private static IEnumerable<string> getCreateImportAHSCommands() {
        yield return createAuthorHierarchicalSearchNodesSql;
        yield return createIndexAuthorHierarchicalSearchNodesForPreviousIdSql;
        yield return createAuthorHierarchicalSearchResultsSql;
        yield return createIndexAuthorHierarchicalSearchResultsForNodeIdSql;
    }

    private static async Task ExecuteSqlAsync(DbConnection connection, string sql) {
        var _ = await connection.ExecuteAsync(sql);
    }

    private static async Task ExecuteSqlsAsync(DbConnection connection, IEnumerable<string> sqls) {
        foreach (var sql in sqls) {
            await ExecuteSqlAsync(connection, sql);
        }
    }

    public static async Task CreateImportInpxStructure(DbConnection connection, bool fullCreation) {
        await ExecuteSqlsAsync(connection, getDropImportInpxCommands(fullCreation));
        await ExecuteSqlsAsync(connection, getCreateImportInpxCommands(fullCreation));
    }

    public static async Task CreateAHSStructure(DbConnection connection) {
        await ExecuteSqlsAsync(connection, getDropImportAHSCommands());
        await ExecuteSqlsAsync(connection, getCreateImportAHSCommands());
    }
}
