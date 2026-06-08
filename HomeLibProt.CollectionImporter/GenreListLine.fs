module HomeLibProt.CollectionImporter.GenreListLine

open System.Text.RegularExpressions

open HomeLibProt.Common.RegEx

module private GenreInpLineRegExFields =
    let public key = "key"
    let public name = "name"

type GenreListLine = { Key: string; Name: string }

let private regExPattern =
    $"(?<{GenreInpLineRegExFields.key}>[^\x04]*)\x04(?<{GenreInpLineRegExFields.name}>[^\x04]*)"

let getRegEx () =
    new Regex(regExPattern, RegexOptions.Compiled)

let parseGenreListLine (regEx: Regex) (line: string) : GroupCollection =
    let regExMatch = regEx.Match(line)

    if regExMatch.Success then
        regExMatch.Groups
    else
        failwith $"Unsupported genre line format: \"{line}\""

let getGenreLine (groups: GroupCollection) : GenreListLine =
    { Key = groups.[GenreInpLineRegExFields.key] |> extractGroupValue
      Name = groups.[GenreInpLineRegExFields.name] |> extractGroupValue }
