module HomeLibProt.CollectionImporter.Tests.TestInpLine

open NUnit.Framework

open HomeLibProt.CollectionImporter.InpLine
open HomeLibProt.CollectionImporter.InpLineFields

[<Test>]
let TestWrapRegEx () =
    let expected =
        "^(?<authors>[^\x04]*)\x04(?<genres>[^\x04]*)\x04(?<title>[^\x04]*)\x04(?<series>[^\x04]*)\x04(?<serno>[^\x04]*)\x04(?<file>[^\x04]*)\x04(?<size>[^\x04]*)\x04(?<libid>[^\x04]*)\x04(?<del>[^\x04]*)\x04(?<extension>[^\x04]*)\x04(?<date>[^\x04]*)\x04(?<insno>[^\x04]*)\x04(?<folder>[^\x04]*)\x04(?<lang>[^\x04]*)\x04(?<rate>[^\x04]*)\x04(?<keywords>[^\x04]*)\x04$"

    let regEx =
        "(?<authors>[^\x04]*)\x04(?<genres>[^\x04]*)\x04(?<title>[^\x04]*)\x04(?<series>[^\x04]*)\x04(?<serno>[^\x04]*)\x04(?<file>[^\x04]*)\x04(?<size>[^\x04]*)\x04(?<libid>[^\x04]*)\x04(?<del>[^\x04]*)\x04(?<extension>[^\x04]*)\x04(?<date>[^\x04]*)\x04(?<insno>[^\x04]*)\x04(?<folder>[^\x04]*)\x04(?<lang>[^\x04]*)\x04(?<rate>[^\x04]*)\x04(?<keywords>[^\x04]*)"

    let actual = regEx |> wrapRegEx

    Assert.That(actual, Is.EqualTo expected)

[<Test>]
let TestJoinRegExGroups () =
    let expected =
        "(?<authors>[^\x04]*)\x04(?<genres>[^\x04]*)\x04(?<title>[^\x04]*)\x04(?<series>[^\x04]*)\x04(?<serno>[^\x04]*)\x04(?<file>[^\x04]*)\x04(?<size>[^\x04]*)\x04(?<libid>[^\x04]*)\x04(?<del>[^\x04]*)\x04(?<extension>[^\x04]*)\x04(?<date>[^\x04]*)\x04(?<insno>[^\x04]*)\x04(?<folder>[^\x04]*)\x04(?<lang>[^\x04]*)\x04(?<rate>[^\x04]*)\x04(?<keywords>[^\x04]*)"

    let regExGroups =
        [| "(?<authors>[^\x04]*)"
           "(?<genres>[^\x04]*)"
           "(?<title>[^\x04]*)"
           "(?<series>[^\x04]*)"
           "(?<serno>[^\x04]*)"
           "(?<file>[^\x04]*)"
           "(?<size>[^\x04]*)"
           "(?<libid>[^\x04]*)"
           "(?<del>[^\x04]*)"
           "(?<extension>[^\x04]*)"
           "(?<date>[^\x04]*)"
           "(?<insno>[^\x04]*)"
           "(?<folder>[^\x04]*)"
           "(?<lang>[^\x04]*)"
           "(?<rate>[^\x04]*)"
           "(?<keywords>[^\x04]*)" |]

    let actual = regExGroups |> joinRegExGroups

    Assert.That(actual, Is.EqualTo(expected))

[<Test>]
let TestMapInpFieldToRegExGroup () =
    let expected =
        [| "(?<authors>[^\x04]*)"
           "(?<genres>[^\x04]*)"
           "(?<title>[^\x04]*)"
           "(?<series>[^\x04]*)"
           "(?<serno>[^\x04]*)"
           "(?<file>[^\x04]*)"
           "(?<size>[^\x04]*)"
           "(?<libid>[^\x04]*)"
           "(?<del>[^\x04]*)"
           "(?<extension>[^\x04]*)"
           "(?<date>[^\x04]*)"
           "(?<insno>[^\x04]*)"
           "(?<folder>[^\x04]*)"
           "(?<lang>[^\x04]*)"
           "(?<rate>[^\x04]*)"
           "(?<keywords>[^\x04]*)" |]

    let inpFields =
        [| Required authors
           Required genres
           Required title
           Required series
           Required serno
           Required file
           Required size
           Required libid
           Required del
           Required extension
           Required date
           Option insno
           Option folder
           Required lang
           Required rate
           Required keywords |]

    let actual = inpFields |> Array.map mapInpFieldToRegExGroup

    Assert.That(actual, Is.EqualTo(expected).AsCollection)

[<Test>]
let TestValidateInpFieldsOnRequired () =
    let expected =
        [| Required authors
           Required genres
           Required title
           Required series
           Required serno
           Required file
           Required size
           Required libid
           Required del
           Required extension
           Required date
           Option insno
           Option folder
           Required lang
           Required rate
           Required keywords |]

    let inpFields =
        [| Required authors
           Required genres
           Required title
           Required series
           Required serno
           Required file
           Required size
           Required libid
           Required del
           Required extension
           Required date
           Option insno
           Option folder
           Required lang
           Required rate
           Required keywords |]

    let actual = inpFields |> validateInpFieldsOnRequired

    Assert.That(actual, Is.EqualTo(expected).AsCollection)

[<Test>]
let TestValidateInpFieldsOnRequiredWithFail () =
    let inpFields =
        [| Required authors
           Required title
           Required serno
           Required file
           Required size
           Required libid
           Required del
           Required extension
           Required date
           Option insno
           Option folder
           Required lang
           Required rate
           Required keywords |]

    Assert.That(
        (fun _ -> inpFields |> validateInpFieldsOnRequired |> ignore),
        Throws.Exception.With.Message.EqualTo "Required Inp Fields are absent: Required \"genres\", Required \"series\""
    )

[<Test>]
let TestMapStructureSectionToInpField () =
    let expected =
        [| Required authors
           Required genres
           Required title
           Required series
           Required serno
           Required file
           Required size
           Required libid
           Required del
           Required extension
           Required date
           Option insno
           Option folder
           Required lang
           Required rate
           Required keywords |]

    let structureSections =
        [| "author"
           "genre"
           "title"
           "series"
           "serno"
           "file"
           "size"
           "libid"
           "del"
           "ext"
           "date"
           "insno"
           "folder"
           "lang"
           "librate"
           "keywords" |]

    let actual = structureSections |> Array.map mapStructureSectionToInpField

    Assert.That(actual, Is.EqualTo(expected).AsCollection)

[<Test>]
let TestMapStructureSectionToInpFieldFail () =
    let structureSections = [| "unknown section" |]

    Assert.That(
        (fun _ -> structureSections |> Array.map mapStructureSectionToInpField |> ignore),
        Throws.Exception.With.Message.EqualTo "Unsupported Inp Field: unknown section"
    )

[<Test>]
let TestGetStructureSections () =
    let expected =
        [| "author"
           "genre"
           "title"
           "series"
           "serno"
           "file"
           "size"
           "libid"
           "del"
           "ext"
           "date"
           "insno"
           "folder"
           "lang"
           "librate"
           "keywords" |]

    let structure =
        "author;genre;title;series;serno;file;size;libid;del;ext;date;insno;folder;lang;librate;keywords"

    let actual = structure |> getStructureSections

    Assert.That(actual, Is.EqualTo(expected).AsCollection)

[<Test>]
let TestParseInpLineDefaultStructureUnsupportedFormat () =
    let inpLine =
        "A,A,A:B,B,B:\x04Genre 1:Genre 2:\x04Title 1\x04Series 1\x041\x041\x04100500\x041\x040\x04fb2\x042025-05-28\x04en\x042\x04Keyword 1, Keyword 2\x040"

    Assert.That(
        (fun _ -> (defaultStructure |> getRegEx, inpLine) ||> parseInpLine |> ignore),
        Throws.Exception.With.Message.EqualTo
            "Unsupported inpx format: \"A,A,A:B,B,B:Genre 1:Genre 2:Title 1Series 11110050010fb22025-05-28en2Keyword 1, Keyword 20\""
    )

[<Test>]
let TestGetInpLineDefaultStructure () =
    let expected =
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
          InsNumber = None
          Folder = None
          Lang = "en"
          LibRate = "2"
          Keywords = "Keyword 1, Keyword 2" }

    let inpLine =
        "A,A,A:B,B,B:\x04Genre 1:Genre 2:\x04Title 1\x04Series 1\x041\x041\x04100500\x041\x040\x04fb2\x042025-05-28\x04en\x042\x04Keyword 1, Keyword 2\x04"

    let actual = (defaultStructure |> getRegEx, inpLine) ||> parseInpLine |> getInpLine

    Assert.That(actual, Is.EqualTo expected)

[<Test>]
let TestGetInpLine () =
    let expected =
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

    let inpLine =
        "A,A,A:B,B,B:\x04Genre 1:Genre 2:\x04Title 1\x04Series 1\x041\x041\x04100500\x041\x040\x04fb2\x042025-05-28\x041\x04archive.zip\x04en\x042\x04Keyword 1, Keyword 2\x04"

    let structure =
        "AUTHOR;GENRE;TITLE;SERIES;SERNO;FILE;SIZE;LIBID;DEL;EXT;DATE;INSNO;FOLDER;LANG;LIBRATE;KEYWORDS"

    let actual = (structure |> getRegEx, inpLine) ||> parseInpLine |> getInpLine

    Assert.That(actual, Is.EqualTo expected)
