module HomeLibProt.CollectionImporter.BookImporter

open System.Data.Common
open System.Threading.Tasks

open HomeLibProt.Domain.DataAccess
open HomeLibProt.CollectionImporter

let private makeBookSeriesParams
    (seriesMap: Map<string, int64>)
    (bookId: int64)
    (series: Book.Series)
    : BookSeriesParam =
    BookSeriesParam(BookId = bookId, SeriesId = (seriesMap |> Map.find series.Name), SeriesNumber = series.Number)

let tryInsertSeries
    (connection: DbConnection)
    (seriesMap: Map<string, int64>)
    (bookId: int64)
    (series: Book.Series option)
    : Task<unit> =
    task {
        match series with
        | None -> ()
        | Some s ->
            do!
                (connection, s |> makeBookSeriesParams seriesMap bookId)
                |> BookSeries.InsertBookSeriesAsync
    }

let private getKeywordsFromBook (book: Book.Book) : string array = book.Keywords

let private getSeriesFromBook (book: Book.Book) : Book.Series option = book.Series

let private getGenresFromBook (book: Book.Book) : string array = book.Genres

let private getAuthorsFromBook (book: Book.Book) : string array =
    book.Authors |> Array.map (fun a -> a.FullName) |> Array.distinct

let private makeBookGenresParams
    (genresMap: Map<string, int64>)
    (bookId: int64)
    (genres: string array)
    : BookGenreParam array =
    genres
    |> Array.map (fun g -> BookGenreParam(BookId = bookId, GenreId = (genresMap |> Map.find g)))

let private makeAuthorshipParams
    (authorsMap: Map<string, int64>)
    (bookId: int64)
    (authors: string array)
    : AuthorshipParam array =
    authors
    |> Array.map (fun a -> AuthorshipParam(BookId = bookId, AuthorId = (authorsMap |> Map.find a)))

let private makeBookKeywordParams
    (keywordsMap: Map<string, int64>)
    (bookId: int64)
    (keywords: string array)
    : BookKeywordParam array =
    keywords
    |> Array.map (fun k -> BookKeywordParam(BookId = bookId, KeywordId = (keywordsMap |> Map.find k)))

let private getBookParamsFromBook
    (languagesMap: Map<string, int64>)
    (archivesMap: Map<string, int64>)
    (book: Book.Book)
    : BookParam =
    BookParam(
        Title = book.Title,
        FileName = book.FileName,
        Size = book.Size,
        LibId = book.LibId,
        Deleted = book.Deleted,
        Extension = book.Extension,
        Date = book.Date,
        ArchiveId = (archivesMap |> Map.find book.Folder),
        LibRate = book.LibRate,
        LanguageId = (languagesMap |> Map.find book.Lang)
    )

let internal insertBookAsync
    (authorsMap: Map<string, int64>)
    (genresMap: Map<string, int64>)
    (seriesMap: Map<string, int64>)
    (keywordsMap: Map<string, int64>)
    (languagesMap: Map<string, int64>)
    (archivesMap: Map<string, int64>)
    (connection: DbConnection)
    (book: Book.Book)
    : Task<unit> =
    task {
        let! bookId =
            (connection, book |> getBookParamsFromBook languagesMap archivesMap)
            |> Books.InsertBookAsync

        do!
            (connection, book |> getAuthorsFromBook |> makeAuthorshipParams authorsMap bookId)
            |> Authorships.InsertAuthorshipsAsync

        do!
            (connection, book |> getGenresFromBook |> makeBookGenresParams genresMap bookId)
            |> BookGenres.InsertBookGenresAsync


        do! book |> getSeriesFromBook |> tryInsertSeries connection seriesMap bookId

        do!
            (connection, book |> getKeywordsFromBook |> makeBookKeywordParams keywordsMap bookId)
            |> BookKeywords.InsertBookKeywordsAsync
    }

let private mapAuthorToAuthorParam (author: Book.Author) : AuthorParam =
    AuthorParam(
        FullName = author.FullName,
        FirstName = author.FirstName,
        MiddleName = author.MiddleName,
        LastName = author.LastName
    )

let private mapArchiveToNameId (archive: Archive) : string * int64 = archive.Name, archive.Id

let private mapLanguageToNameId (language: Language) : string * int64 = language.Name, language.Id

let private mapKeywordToNameId (keyword: Keyword) : string * int64 = keyword.Name, keyword.Id

let private mapSeriesToNameId (series: SeriesEntity) : string * int64 = series.Name, series.Id

let private mapGenreToNameId (genre: Genre) : string * int64 = genre.Key, genre.Id

let private mapAuthorToNameId (author: Author) : string * int64 = author.Name, author.Id

let private getExistingEntitiesAsync<'T, 'U>
    (connection: DbConnection)
    (getEntitiesAsync: DbConnection * string array -> Task<'U array>)
    (source: string array)
    : Task<'U array> =
    task {
        let! entities = (connection, source) |> getEntitiesAsync
        return entities
    }

let private getUnexisitingEntities<'T when 'T: comparison>
    (source: 'T Set)
    (keyAccessor: 'T -> string)
    (existedEntities: Map<string, int64>)
    : 'T array =
    source
    |> Set.filter (fun s -> existedEntities |> Map.containsKey (keyAccessor s) |> not)
    |> Set.toArray

let private insertUnexistedEntitiesAsync
    (connection: DbConnection)
    (getEntitiesAsync: DbConnection * string array -> Task<'T2 array>)
    (mapEntityToKeyValue: 'T2 -> string * int64)
    (keyAccessor: 'T1 -> string)
    (mapEntityToInsertParams: 'T1 -> 'T3)
    (insertEntities: DbConnection * 'T3 array -> Task)
    (source: 'T1 Set)
    : Task<unit> =
    task {
        let sourceKeys = source |> Set.map keyAccessor

        let! entities =
            sourceKeys
            |> Set.toArray
            |> getExistingEntitiesAsync connection getEntitiesAsync

        do!
            (connection,
             entities
             |> Array.map mapEntityToKeyValue
             |> Map
             |> getUnexisitingEntities source keyAccessor
             |> Array.map mapEntityToInsertParams)
            |> insertEntities
    }

let private getEntitiesMap
    (connection: DbConnection)
    (keyAccessor: 'T1 -> string)
    (getEntitiesAsync: DbConnection * string array -> Task<'T2 array>)
    (mapEntityToKeyValue: 'T2 -> string * int64)
    (source: 'T1 Set)
    : Task<Map<string, int64>> =
    task {
        let! entities = (connection, source |> Set.map keyAccessor |> Set.toArray) |> getEntitiesAsync

        return entities |> Array.map mapEntityToKeyValue |> Map
    }

let private authorKeyAccessor (a: Book.Author) : string = a.FullName

let internal insertUnexistedArchivesAsync (connection: DbConnection) (archives: Set<string>) : Task<unit> =
    task {
        do!
            insertUnexistedEntitiesAsync
                connection
                Archives.GetArchivesByNameAsync
                mapArchiveToNameId
                id
                id
                Archives.InsertArchivesAsync
                archives
    }

let internal getArchivesMapAsync (connection: DbConnection) (archives: Set<string>) : Task<Map<string, int64>> =
    task { return! getEntitiesMap connection id Archives.GetArchivesByNameAsync mapArchiveToNameId archives }

let internal insertUnexistedLanguagesAsync (connection: DbConnection) (languages: Set<string>) : Task<unit> =
    task {
        do!
            insertUnexistedEntitiesAsync
                connection
                Languages.GetLanguagesByNameAsync
                mapLanguageToNameId
                id
                id
                Languages.InsertLanguagesAsync
                languages
    }

let internal getLanguagesMapAsync (connection: DbConnection) (languages: Set<string>) : Task<Map<string, int64>> =
    task { return! getEntitiesMap connection id Languages.GetLanguagesByNameAsync mapLanguageToNameId languages }

let internal insertUnexistedKeywordsAsync (connection: DbConnection) (keywords: Set<string>) : Task<unit> =
    task {
        do!
            insertUnexistedEntitiesAsync
                connection
                Keywords.GetKeywordsByNameAsync
                mapKeywordToNameId
                id
                id
                Keywords.InsertKeywordsAsync
                keywords
    }

let internal getKeywordsMapAsync (connection: DbConnection) (keywords: Set<string>) : Task<Map<string, int64>> =
    task { return! getEntitiesMap connection id Keywords.GetKeywordsByNameAsync mapKeywordToNameId keywords }

let internal insertUnexistedSeriesAsync (connection: DbConnection) (series: Set<string>) : Task<unit> =
    task {
        do!
            insertUnexistedEntitiesAsync
                connection
                Series.GetSeriesByNameAsync
                mapSeriesToNameId
                id
                id
                Series.InsertSeriesAsync
                series
    }

let internal getSeriesMapAsync (connection: DbConnection) (series: Set<string>) : Task<Map<string, int64>> =
    task { return! getEntitiesMap connection id Series.GetSeriesByNameAsync mapSeriesToNameId series }

let internal insertUnexistedGenresAsync (connection: DbConnection) (genres: Set<string>) : Task<unit> =
    task {
        do!
            insertUnexistedEntitiesAsync
                connection
                Genres.GetGenresByKeyAsync
                mapGenreToNameId
                id
                id
                Genres.InsertGenresAsync
                genres
    }

let internal getGenresMapAsync (connection: DbConnection) (genres: Set<string>) : Task<Map<string, int64>> =
    task { return! getEntitiesMap connection id Genres.GetGenresByKeyAsync mapGenreToNameId genres }

let internal insertUnexistedAuthorsAsync (connection: DbConnection) (authors: Set<Book.Author>) : Task<unit> =
    task {
        do!
            insertUnexistedEntitiesAsync
                connection
                Authors.GetAuthorsByNameAsync
                mapAuthorToNameId
                authorKeyAccessor
                mapAuthorToAuthorParam
                Authors.InsertAuthorsAsync
                authors
    }

let internal getAuthorsMapAsync (connection: DbConnection) (authors: Set<Book.Author>) : Task<Map<string, int64>> =
    task { return! getEntitiesMap connection authorKeyAccessor Authors.GetAuthorsByNameAsync mapAuthorToNameId authors }

let processBooks (books: Book.Book array) (connection: DbConnection) : Task =
    task {
        let authors = books |> Array.collect (fun l -> l.Authors) |> Set.ofArray

        do! authors |> insertUnexistedAuthorsAsync connection

        let! authorsMap = getAuthorsMapAsync connection authors

        let genres = books |> Array.collect (fun l -> l.Genres) |> Set.ofArray

        do! genres |> insertUnexistedGenresAsync connection

        let! genresMap = genres |> getGenresMapAsync connection

        let series =
            books
            |> Array.choose (fun l -> l.Series)
            |> Array.map (fun s -> s.Name)
            |> Set.ofArray

        do! series |> insertUnexistedSeriesAsync connection

        let! seriesMap = series |> getSeriesMapAsync connection

        let keywords = books |> Array.collect (fun l -> l.Keywords) |> Set.ofArray

        do! keywords |> insertUnexistedKeywordsAsync connection

        let! keywordsMap = keywords |> getKeywordsMapAsync connection

        let languages = books |> Array.map (fun l -> l.Lang) |> Set.ofArray

        do! languages |> insertUnexistedLanguagesAsync connection

        let! languagesMap = languages |> getLanguagesMapAsync connection

        let archives = books |> Array.map (fun l -> l.Folder) |> Set.ofArray

        do! archives |> insertUnexistedArchivesAsync connection

        let! archivesMap = archives |> getArchivesMapAsync connection

        for book in books do
            do!
                book
                |> insertBookAsync authorsMap genresMap seriesMap keywordsMap languagesMap archivesMap connection
    }
