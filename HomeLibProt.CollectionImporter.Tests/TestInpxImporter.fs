module HomeLibProt.CollectionImporter.Tests.TestInpxImporter

open NUnit.Framework
open System
open System.IO

open HomeLibProt.CollectionImporter.InpxImporter
open HomeLibProt.CollectionImporter.Tests.Utils
open HomeLibProt.Domain.DataAccess
open HomeLibProt.Domain.Tests.Entities
open HomeLibProt.Domain.Tests.Utils

let getInpxPath (testCaseName: string) =
    Path.Combine(TestCaseUtils.getTestCasesBasePath "TestInpxImporter", testCaseName, "inpx.inpx")

[<Test>]
let TestInsertBookAsync () =
    task {
        let expectedBooks =
            [| TestBook(
                   Id = 1,
                   Title = "Title 1",
                   FileName = "1",
                   Size = 100500,
                   LibId = "1",
                   Deleted = 0,
                   Extension = "fb2",
                   Date = "2025-05-28",
                   Folder = "000001-000002.zip",
                   LibRate = Nullable(0L)
               )
               TestBook(
                   Id = 2,
                   Title = "Title 2",
                   FileName = "2",
                   Size = 100500,
                   LibId = "2",
                   Deleted = 0,
                   Extension = "fb2",
                   Date = "2025-05-28",
                   Folder = "000001-000002.zip",
                   LibRate = Nullable(1L)
               ) |]

        let expectedAuthors =
            [| TestAuthor(Id = 1, FullName = "B B B", FirstName = "B", MiddleName = "B", LastName = "B") |]

        let expectedAuthorships =
            [| TestAuthorship(BookId = 1, AuthorId = 1)
               TestAuthorship(BookId = 2, AuthorId = 1) |]

        let expectedGenres = [| TestGenre(Id = 1, Key = "Genre 1", Name = null) |]

        let expectedBookGenres =
            [| TestBookGenre(BookId = 1, GenreId = 1)
               TestBookGenre(BookId = 2, GenreId = 1) |]

        let expectedKeywords = [| TestKeyword(Id = 1, Name = "Keyword 1") |]

        let expectedBookKeywords =
            [| TestBookKeyword(BookId = 1, KeywordId = 1)
               TestBookKeyword(BookId = 2, KeywordId = 1) |]

        let expectedSeries = [| TestSeriesEntity(Id = 1, Name = "Series 1") |]

        let expectedBookSeries =
            [| TestBookSeriesEntity(BookId = 1, SeriesId = 1, SeriesNumber = 1)
               TestBookSeriesEntity(BookId = 2, SeriesId = 1, SeriesNumber = 2) |]

        let inpxImporterParameters =
            { PathToInpx = getInpxPath "Simple"
              BatchSize = 2
              ProgressReport = ignore
              DoInTransactionAsync = ConnectionUtils.DoInTransactionAsync }

        let! books, authors, authorships, genres, bookGenres, keywords, bookKeywords, series, bookSeries =
            TestUtils.UseTestDatabase(fun connection ->
                task {

                    do! importInpxToDb inpxImporterParameters connection

                    let! books = ConnectionUtils.DoInTransactionAsync(connection, BookUtils.GetTestData)
                    let! authors = ConnectionUtils.DoInTransactionAsync(connection, AuthorUtils.GetTestData)
                    let! authorships = ConnectionUtils.DoInTransactionAsync(connection, AuthorshipUtils.GetTestData)
                    let! genres = ConnectionUtils.DoInTransactionAsync(connection, GenreUtils.GetTestData)
                    let! bookGenres = ConnectionUtils.DoInTransactionAsync(connection, BookGenreUtils.GetTestData)
                    let! keywords = ConnectionUtils.DoInTransactionAsync(connection, KeywordUtils.GetTestData)
                    let! bookKeywords = ConnectionUtils.DoInTransactionAsync(connection, BookKeywordUtils.GetTestData)
                    let! series = ConnectionUtils.DoInTransactionAsync(connection, SeriesUtils.GetTestData)
                    let! bookSeries = ConnectionUtils.DoInTransactionAsync(connection, BookSeriesUtils.GetTestData)

                    return books, authors, authorships, genres, bookGenres, keywords, bookKeywords, series, bookSeries
                })

        Assert.That(books, Is.EqualTo expectedBooks)
        Assert.That(authors, Is.EqualTo expectedAuthors)
        Assert.That(authorships, Is.EqualTo(expectedAuthorships).AsCollection)
        Assert.That(genres, Is.EqualTo(expectedGenres).AsCollection)
        Assert.That(bookGenres, Is.EqualTo(expectedBookGenres).AsCollection)
        Assert.That(keywords, Is.EqualTo(expectedKeywords).AsCollection)
        Assert.That(bookKeywords, Is.EqualTo(expectedBookKeywords).AsCollection)
        Assert.That(bookSeries, Is.EqualTo(expectedBookSeries).AsCollection)
        Assert.That(series, Is.EqualTo(expectedSeries).AsCollection)

    }
