module HomeLibProt.CollectionImporter.Tests.TestBook

open NUnit.Framework
open System

open HomeLibProt.CollectionImporter.Book
open HomeLibProt.CollectionImporter.InpLine

let mapAuthorsTestCases =
    [| TestCaseData(
           "A,A,A:",
           { FullName = "A A A"
             LastName = "A"
             FirstName = "A"
             MiddleName = "A" }
       )
       TestCaseData(
           "A: A, A,A,A:",
           { FullName = "A: A, A A A"
             LastName = "A: A, A"
             FirstName = "A"
             MiddleName = "A" }
       )
       TestCaseData(
           "A,A: A, A,A:",
           { FullName = "A A: A, A A"
             LastName = "A"
             FirstName = "A: A, A"
             MiddleName = "A" }
       )
       TestCaseData(
           "A,A,A: A, A:",
           { FullName = "A A A: A, A"
             LastName = "A"
             FirstName = "A"
             MiddleName = "A: A, A" }
       )
       TestCaseData(
           "A:",
           { FullName = "A  "
             LastName = "A"
             FirstName = ""
             MiddleName = "" }
       ) |]

[<Test>]
[<TestCaseSource(nameof mapAuthorsTestCases)>]
let TestMapAuthors (authors: string, expected: Author) =
    let actual = authors |> mapAuthor authorName

    Assert.That(actual, Is.EqualTo expected)

let makeAuthorsTestCases =
    [| TestCaseData("A,A,A:B,B,B:", [| "A,A,A:"; "B,B,B:" |])
       TestCaseData("A: A,A,A:B,B: B,B:C,C,C: C:", [| "A: A,A,A:"; "B,B: B,B:"; "C,C,C: C:" |]) |]

[<Test>]
[<TestCaseSource(nameof makeAuthorsTestCases)>]
let TestMakeAuthors (authors: string, expected: string array) =
    let actual = authors |> makeAuthors authorGroups

    Assert.That(actual, Is.EqualTo expected)

[<Test>]
let TestMakeGenres () =
    let expected = [| "Genre 1"; "Genre 2" |]

    let keywords = "Genre 1:Genre 2:"

    let actual = keywords |> makeGenres

    Assert.That(actual, Is.EqualTo expected)

[<Test>]
let TestMakeKeywords () =
    let expected = [| "Keyword 1"; "Keyword 2" |]

    let keywords = "Keyword 1, Keyword 2"

    let actual = keywords |> makeKeywords

    Assert.That(actual, Is.EqualTo expected)

let makeSeriesTestCases =
    [| TestCaseData("Series 1", "1", Some { Name = "Series 1"; Number = 1 })
       TestCaseData("Series 1", "", Some { Name = "Series 1"; Number = 0 })
       TestCaseData("", "1", None)
       TestCaseData("", "", None) |]

[<Test>]
[<TestCaseSource(nameof makeSeriesTestCases)>]
let TestMakeSeries (value: string, number: string, expected: Series option) =
    let actual = (value, number) ||> makeSeries

    Assert.That(actual, Is.EqualTo expected)

let convertStringToBoolTestCases =
    [| TestCaseData("0", false); TestCaseData("1", true); TestCaseData("", true) |]

[<Test>]
[<TestCaseSource(nameof convertStringToBoolTestCases)>]
let TestConvertStringToBool (value: string, expected: bool) =
    let actual = value |> convertStringToBool

    Assert.That(actual, Is.EqualTo expected)

let convertStringToNullableIntTestCases =
    [| TestCaseData("", Nullable()); TestCaseData("1", Nullable 1) |]

[<Test>]
[<TestCaseSource(nameof convertStringToNullableIntTestCases)>]
let TestConvertStringToNullableInt (value: string, expected: Nullable<int>) =
    let actual = value |> convertStringToNullableInt

    Assert.That(actual, Is.EqualTo expected)

[<Test>]
let TestConvertInpLineToBook () =
    let expected =
        { Authors =
            [| { FullName = "A A A"
                 LastName = "A"
                 FirstName = "A"
                 MiddleName = "A" }
               { FullName = "B B B"
                 LastName = "B"
                 FirstName = "B"
                 MiddleName = "B" } |]
          Genres = [| "Genre 1"; "Genre 2" |]
          Title = "Title 1"
          Series = Some { Name = "Series 1"; Number = 1 }
          FileName = "1"
          Size = 100500
          LibId = "1"
          Deleted = false
          Extension = "fb2"
          Date = "2025-05-28"
          Folder = "archive.zip"
          Lang = "en"
          LibRate = Nullable 2
          Keywords = [| "Keyword 1"; "Keyword 2" |] }

    let inpLine =
        { Authors = "A,A,A:B,B,B:"
          Genres = "Genre 1:Genre 2:"
          Title = "Title 1"
          Series = "Series 1"
          SeriesNumber = "1"
          FileName = "1"
          Size = "100500"
          LibId = "1"
          Deleted = "0"
          Extension = "fb2"
          Date = "2025-05-28"
          InsNumber = Some "1"
          Folder = Some "archive.zip"
          Lang = "en"
          LibRate = "2"
          Keywords = "Keyword 1, Keyword 2" }

    let actual = inpLine |> convertInpLineToBook "test.zip"

    Assert.That(actual, Is.EqualTo expected)
