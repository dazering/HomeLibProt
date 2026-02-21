module HomeLibProt.CollectionImporter.Tests.TestBookImporter

open NUnit.Framework
open System
open System.Data.Common
open System.Threading.Tasks

open HomeLibProt.CollectionImporter.Book
open HomeLibProt.CollectionImporter.BookImporter
open HomeLibProt.Domain.DataAccess
open HomeLibProt.Domain.Tests.Entities
open HomeLibProt.Domain.Tests.Utils

let setUpData (connection: DbConnection) : Task<unit> =
    task {
        let! _ = AuthorUtils.Create(connection, fullName = "A A A", lastName = "A", firstName = "A", middleName = "A")
        let! _ = AuthorUtils.Create(connection, fullName = "B B B", lastName = "B", firstName = "B", middleName = "B")
        let! _ = GenreUtils.Create(connection, key = "genre1", name = "Genre 1")
        let! _ = GenreUtils.Create(connection, key = "genre2", name = "Genre 2")
        let! _ = SeriesUtils.Create(connection, name = "Series 1")
        let! _ = SeriesUtils.Create(connection, name = "Series 2")
        let! _ = KeywordUtils.Create(connection, name = "Keyword 1")
        let! _ = KeywordUtils.Create(connection, name = "Keyword 2")
        let! _ = LanguageUtils.Create(connection, name = "Lang 1")
        let! _ = LanguageUtils.Create(connection, name = "Lang 2")
        do ()
    }

[<Test>]
let TestInsertBookAsync () =
    task {
        let expectedBooks =
            [| TestBook(
                   Id = 1,
                   Title = "Title 1",
                   FileName = "File1",
                   Size = 100500,
                   LibId = "File1",
                   Deleted = 0,
                   Extension = "fb2",
                   Date = "2025-11-07",
                   Folder = "archive.zip",
                   LibRate = Nullable(2L),
                   LanguageId = 1L
               ) |]

        let expectedAuthorships = [| TestAuthorship(BookId = 1, AuthorId = 1) |]

        let expectedBookGenres = [| TestBookGenre(BookId = 1, GenreId = 1) |]

        let expectedBookKeywords = [| TestBookKeyword(BookId = 1, KeywordId = 1) |]

        let expectedBookSeries =
            [| TestBookSeriesEntity(BookId = 1, SeriesId = 1, SeriesNumber = 1) |]

        let authors = [| "A A A", 1L |] |> Map
        let genres = [| "Genre 1", 1L |] |> Map
        let keywords = [| "Keyword 1", 1L |] |> Map
        let series = [| "Series 1", 1L |] |> Map
        let languages = [| "Lang 1", 1L |] |> Map

        let book =
            { Authors =
                [| { FullName = "A A A"
                     LastName = "A"
                     FirstName = "A"
                     MiddleName = "A" } |]
              Genres = [| "Genre 1" |]
              Title = "Title 1"
              Series = Some { Name = "Series 1"; Number = 1 }
              FileName = "File1"
              Size = 100500
              LibId = "File1"
              Deleted = false
              Extension = "fb2"
              Date = "2025-11-07"
              Folder = "archive.zip"
              Lang = "Lang 1"
              LibRate = Nullable 2
              Keywords = [| "Keyword 1" |] }

        let! books, authorships, bookGenres, bookKeywords, bookSeries =
            TestUtils.UseTestDatabase(fun connection ->
                task {
                    do! ConnectionUtils.DoInTransactionAsync(connection, setUpData)

                    do!
                        ConnectionUtils.DoInTransactionAsync(
                            connection,
                            fun connection ->
                                task {
                                    do! book |> insertBookAsync authors genres series keywords languages connection
                                }
                        )

                    let! books = ConnectionUtils.DoInTransactionAsync(connection, BookUtils.GetTestData)
                    let! authorships = ConnectionUtils.DoInTransactionAsync(connection, AuthorshipUtils.GetTestData)
                    let! bookGenres = ConnectionUtils.DoInTransactionAsync(connection, BookGenreUtils.GetTestData)
                    let! bookKeywords = ConnectionUtils.DoInTransactionAsync(connection, BookKeywordUtils.GetTestData)
                    let! bookSeries = ConnectionUtils.DoInTransactionAsync(connection, BookSeriesUtils.GetTestData)

                    return books, authorships, bookGenres, bookKeywords, bookSeries
                })

        Assert.That(books, Is.EqualTo expectedBooks)
        Assert.That(authorships, Is.EqualTo(expectedAuthorships).AsCollection)
        Assert.That(bookGenres, Is.EqualTo(expectedBookGenres).AsCollection)
        Assert.That(bookKeywords, Is.EqualTo(expectedBookKeywords).AsCollection)
        Assert.That(bookSeries, Is.EqualTo(expectedBookSeries).AsCollection)

    }

[<Test>]
let TestGetLanguagesMapAsyncAsync () =
    task {
        let expected = Map [| "Lang 1", 1 |]

        let languages = [| "Lang 1" |] |> Set.ofArray

        let! actual =
            TestUtils.UseTestDatabase(fun connection ->
                task {
                    do! ConnectionUtils.DoInTransactionAsync(connection, setUpData)

                    return!
                        ConnectionUtils.DoInTransactionAsync(
                            connection,
                            fun connection -> task { return! languages |> getLanguagesMapAsync connection }
                        )
                })

        Assert.That(actual, Is.EqualTo(expected).AsCollection)

    }

[<Test>]
let TestInsertUnexistedLanguagesAsync () =
    task {
        let expected =
            [| TestLanguage(Id = 1, Name = "Lang 1", Include = 1)
               TestLanguage(Id = 2, Name = "Lang 2", Include = 1)
               TestLanguage(Id = 3, Name = "Lang 3", Include = 1) |]

        let languages = [| "Lang 2"; "Lang 3" |] |> Set.ofArray

        let! actual =
            TestUtils.UseTestDatabase(fun connection ->
                task {
                    do! ConnectionUtils.DoInTransactionAsync(connection, setUpData)

                    do!
                        ConnectionUtils.DoInTransactionAsync(
                            connection,
                            fun connection -> task { do! languages |> insertUnexistedLanguagesAsync connection }
                        )

                    return! ConnectionUtils.DoInTransactionAsync(connection, LanguageUtils.GetTestData)
                })

        Assert.That(actual, Is.EqualTo(expected).AsCollection)

    }

[<Test>]
let TestGetKeywordsMapAsyncAsync () =
    task {
        let expected = Map [| "Keyword 1", 1 |]

        let keywords = [| "Keyword 1" |] |> Set.ofArray

        let! actual =
            TestUtils.UseTestDatabase(fun connection ->
                task {
                    do! ConnectionUtils.DoInTransactionAsync(connection, setUpData)

                    return!
                        ConnectionUtils.DoInTransactionAsync(
                            connection,
                            fun connection -> task { return! keywords |> getKeywordsMapAsync connection }
                        )
                })

        Assert.That(actual, Is.EqualTo(expected).AsCollection)

    }

[<Test>]
let TestInsertUnexistedKeywordsAsync () =
    task {
        let expected =
            [| TestKeyword(Id = 1, Name = "Keyword 1")
               TestKeyword(Id = 2, Name = "Keyword 2")
               TestKeyword(Id = 3, Name = "Keyword 3") |]

        let keywords = [| "Keyword 2"; "Keyword 3" |] |> Set.ofArray

        let! actual =
            TestUtils.UseTestDatabase(fun connection ->
                task {
                    do! ConnectionUtils.DoInTransactionAsync(connection, setUpData)

                    do!
                        ConnectionUtils.DoInTransactionAsync(
                            connection,
                            fun connection -> task { do! keywords |> insertUnexistedKeywordsAsync connection }
                        )

                    return! ConnectionUtils.DoInTransactionAsync(connection, KeywordUtils.GetTestData)
                })

        Assert.That(actual, Is.EqualTo(expected).AsCollection)

    }

[<Test>]
let TestGetSeriesMapAsyncAsync () =
    task {
        let expected = Map [| "Series 1", 1 |]

        let series = [| "Series 1" |] |> Set.ofArray

        let! actual =
            TestUtils.UseTestDatabase(fun connection ->
                task {
                    do! ConnectionUtils.DoInTransactionAsync(connection, setUpData)

                    return!
                        ConnectionUtils.DoInTransactionAsync(
                            connection,
                            fun connection -> task { return! series |> getSeriesMapAsync connection }
                        )
                })

        Assert.That(actual, Is.EqualTo(expected).AsCollection)

    }

[<Test>]
let TestInsertUnexistedSeriesAsync () =
    task {
        let expected =
            [| TestSeriesEntity(Id = 1, Name = "Series 1")
               TestSeriesEntity(Id = 2, Name = "Series 2")
               TestSeriesEntity(Id = 3, Name = "Series 3") |]

        let series = [| "Series 2"; "Series 3" |] |> Set.ofArray

        let! actual =
            TestUtils.UseTestDatabase(fun connection ->
                task {
                    do! ConnectionUtils.DoInTransactionAsync(connection, setUpData)

                    do!
                        ConnectionUtils.DoInTransactionAsync(
                            connection,
                            fun connection -> task { do! series |> insertUnexistedSeriesAsync connection }
                        )

                    return! ConnectionUtils.DoInTransactionAsync(connection, SeriesUtils.GetTestData)
                })

        Assert.That(actual, Is.EqualTo(expected).AsCollection)

    }

[<Test>]
let TestGetGenresMapAsyncAsync () =
    task {
        let expected = Map [| "genre1", 1 |]

        let genres = [| "genre1" |] |> Set.ofArray

        let! actual =
            TestUtils.UseTestDatabase(fun connection ->
                task {
                    do! ConnectionUtils.DoInTransactionAsync(connection, setUpData)

                    return!
                        ConnectionUtils.DoInTransactionAsync(
                            connection,
                            fun connection -> task { return! genres |> getGenresMapAsync connection }
                        )
                })

        Assert.That(actual, Is.EqualTo(expected).AsCollection)

    }

[<Test>]
let TestInsertUnexistedGenresAsync () =
    task {
        let expected =
            [| TestGenre(Id = 1, Key = "genre1", Name = "Genre 1")
               TestGenre(Id = 2, Key = "genre2", Name = "Genre 2")
               TestGenre(Id = 3, Key = "genre3", Name = null) |]

        let genres = [| "genre2"; "genre3" |] |> Set.ofArray

        let! actual =
            TestUtils.UseTestDatabase(fun connection ->
                task {
                    do! ConnectionUtils.DoInTransactionAsync(connection, setUpData)

                    do!
                        ConnectionUtils.DoInTransactionAsync(
                            connection,
                            fun connection -> task { do! genres |> insertUnexistedGenresAsync connection }
                        )

                    return! ConnectionUtils.DoInTransactionAsync(connection, GenreUtils.GetTestData)
                })

        Assert.That(actual, Is.EqualTo(expected).AsCollection)

    }

[<Test>]
let TestGetAuthorsMapAsyncAsync () =
    task {
        let expected = Map [| "A A A", 1 |]

        let authors =
            [| { FullName = "A A A"
                 LastName = "A"
                 FirstName = "A"
                 MiddleName = "A" } |]
            |> Set.ofArray

        let! actual =
            TestUtils.UseTestDatabase(fun connection ->
                task {
                    do! ConnectionUtils.DoInTransactionAsync(connection, setUpData)

                    return!
                        ConnectionUtils.DoInTransactionAsync(
                            connection,
                            fun connection -> task { return! authors |> getAuthorsMapAsync connection }
                        )
                })

        Assert.That(actual, Is.EqualTo(expected).AsCollection)

    }

[<Test>]
let TestInsertUnexistedAuthorsAsync () =
    task {
        let expected =
            [| TestAuthor(Id = 1, FullName = "A A A", LastName = "A", FirstName = "A", MiddleName = "A")
               TestAuthor(Id = 2, FullName = "B B B", LastName = "B", FirstName = "B", MiddleName = "B")
               TestAuthor(Id = 3, FullName = "C C C", LastName = "C", FirstName = "C", MiddleName = "C") |]

        let authors =
            [| { FullName = "B B B"
                 LastName = "B"
                 FirstName = "B"
                 MiddleName = "B" }
               { FullName = "C C C"
                 LastName = "C"
                 FirstName = "C"
                 MiddleName = "C" } |]
            |> Set.ofArray

        let! actual =
            TestUtils.UseTestDatabase(fun connection ->
                task {
                    do! ConnectionUtils.DoInTransactionAsync(connection, setUpData)

                    do!
                        ConnectionUtils.DoInTransactionAsync(
                            connection,
                            fun connection -> task { do! authors |> insertUnexistedAuthorsAsync connection }
                        )

                    return! ConnectionUtils.DoInTransactionAsync(connection, AuthorUtils.GetTestData)
                })

        Assert.That(actual, Is.EqualTo(expected).AsCollection)

    }
