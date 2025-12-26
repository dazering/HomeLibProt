module HomeLibProt.CollectionImporter.InpLine

open System
open System.Text.RegularExpressions

open HomeLibProt.CollectionImporter.InpLineFields

type InpLine =
    { Authors: string
      Genres: string
      Title: string
      Series: string
      SeriesNumber: string
      FileName: string
      Size: string
      LibId: string
      Deleted: string
      Extension: string
      Date: string
      InsNumber: string option
      Folder: string option
      Lang: string
      LibRate: string
      Keywords: string }

type InpField =
    | Required of string
    | Option of string

let defaultStructure =
    "AUTHOR;GENRE;TITLE;SERIES;SERNO;FILE;SIZE;LIBID;DEL;EXT;DATE;LANG;LIBRATE;KEYWORDS"

let internal wrapRegEx (regEx: string) = $"^{regEx}\x04$"

let internal joinRegExGroups (regExGroups: string array) = String.Join("\x04", regExGroups)

let internal mapInpFieldToRegExGroup (inpField: InpField) =
    match inpField with
    | Required f
    | Option f -> $"(?<{f}>[^\x04]*)"

let private requiredInpFields =
    Set
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
           Required lang
           Required rate
           Required keywords |]

let internal validateInpFieldsOnRequired (inpFields: InpField array) =
    let presentedRequiredInpFieldsSet =
        inpFields
        |> Array.choose (fun i ->
            match i with
            | Required f -> f |> Required |> Some
            | Option _ -> None)
        |> Set

    let difference = Set.difference requiredInpFields presentedRequiredInpFieldsSet

    if difference |> Set.isEmpty |> not then
        let message = String.Join(", ", difference |> Set.map (fun f -> f))
        failwith $"Required Inp Fields are absent: {message}"

    inpFields

let internal mapStructureSectionToInpField (section: string) : InpField =
    match section with
    | "author" -> Required authors
    | "genre" -> Required genres
    | "title" -> Required title
    | "series" -> Required series
    | "serno" -> Required serno
    | "file" -> Required file
    | "size" -> Required size
    | "libid" -> Required libid
    | "del" -> Required del
    | "ext" -> Required extension
    | "date" -> Required date
    | "insno" -> Option insno
    | "folder" -> Option folder
    | "lang" -> Required lang
    | "librate" -> Required rate
    | "keywords" -> Required keywords
    | s -> failwith $"Unsupported Inp Field: {s}"

let internal getStructureSections (structure: string) =
    structure.Split(';', StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries)

let getRegEx (structure: string) =
    let regExPattern =
        structure.ToLower()
        |> getStructureSections
        |> Array.map mapStructureSectionToInpField
        |> validateInpFieldsOnRequired
        |> Array.map mapInpFieldToRegExGroup
        |> joinRegExGroups
        |> wrapRegEx

    new Regex(regExPattern, RegexOptions.Compiled)

let parseInpLine (regEx: Regex) (line: string) : GroupCollection =
    let regExMatch = regEx.Match(line)

    if regExMatch.Success then
        regExMatch.Groups
    else
        failwith $"Unsupported inpx format: \"{line}\""

let private extractGroupValue (group: Group) : string = group.Value

let private tryToExtractGroupValue (group: Group) : string option =
    match group.Success with
    | true -> group |> extractGroupValue |> Some
    | _ -> None

let getInpLine (groups: GroupCollection) : InpLine =
    { Authors = groups.[authors] |> extractGroupValue
      Genres = groups.[genres] |> extractGroupValue
      Title = groups.[title] |> extractGroupValue
      Series = groups.[series] |> extractGroupValue
      SeriesNumber = groups.[serno] |> extractGroupValue
      FileName = groups.[file] |> extractGroupValue
      Size = groups.[size] |> extractGroupValue
      LibId = groups.[libid] |> extractGroupValue
      Deleted = groups.[del] |> extractGroupValue
      Extension = groups.[extension] |> extractGroupValue
      Date = groups.[date] |> extractGroupValue
      InsNumber = groups.[insno] |> tryToExtractGroupValue
      Folder = groups.[folder] |> tryToExtractGroupValue
      Lang = groups.[lang] |> extractGroupValue
      LibRate = groups.[rate] |> extractGroupValue
      Keywords = groups.[keywords] |> extractGroupValue }
