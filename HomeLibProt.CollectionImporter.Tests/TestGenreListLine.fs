module HomeLibProt.CollectionImporter.Tests.TestGenreListLine

open NUnit.Framework

open HomeLibProt.CollectionImporter.GenreListLine

[<Test>]
let TestGetInpLine () =
    let expected = { Key = "genre1"; Name = "Genre 1" }

    let genreListLine = "genre1\x04Genre 1"

    let actual = (getRegEx (), genreListLine) ||> parseGenreListLine |> getGenreLine

    Assert.That(actual, Is.EqualTo expected)
