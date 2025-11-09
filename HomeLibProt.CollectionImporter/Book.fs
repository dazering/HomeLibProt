module HomeLibProt.CollectionImporter.Book

open System
open System.Text.RegularExpressions

open InpLine

type Author =
    { FullName: string
      FirstName: string
      MiddleName: string
      LastName: string }

type Series = { Name: string; Number: int64 }

type Book =
    { Authors: Author array
      Genres: string array
      Title: string
      Series: Series option
      FileName: string
      Size: int64
      LibId: string
      Deleted: bool
      Extension: string
      Date: string
      Folder: string
      Lang: string
      LibRate: Nullable<int>
      Keywords: string array }

let internal authorGroups = new Regex("(.+?:(?! ))+?", RegexOptions.Compiled)

let internal authorName =
    new Regex(
        "(?:(?:(?:(?<lastName>.*),(?! )(?<firstName>.*),(?! )(?<middleName>.*))|(?<lastName>.*)):)",
        RegexOptions.Compiled
    )

let private extractGroupValue (group: Group) : string = group.Value

let private tryToExtractGroupValue (group: Group) : string option =
    match group.Success with
    | true -> group |> extractGroupValue |> Some
    | _ -> None

let internal mapAuthor (authorsNamesRegex: Regex) (author: string) : Author =
    let groups = authorsNamesRegex.Match(author).Groups

    let lastName = extractGroupValue groups.["lastName"]

    let firstName =
        tryToExtractGroupValue groups.["firstName"] |> Option.defaultValue String.Empty

    let middleName =
        tryToExtractGroupValue groups.["middleName"] |> Option.defaultValue String.Empty

    let fullName = $"{lastName} {firstName} {middleName}"

    { FullName = fullName
      FirstName = firstName
      MiddleName = middleName
      LastName = lastName }

let internal makeAuthors (authorsGroupsRegex: Regex) (authors: string) : string array =
    authors
    |> authorsGroupsRegex.Matches
    |> Seq.map (fun m -> m.Value)
    |> Seq.toArray

let internal makeGenres (genres: string) : string array =
    genres.Split(':', StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries)

let internal makeKeywords (keywords: string) : string array =
    keywords.Split(',', StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries)

let private tryParseInt64OrDefault (number: string) : int64 =
    match number |> Int64.TryParse with
    | true, v -> v
    | false, _ -> 0L

let internal makeSeries (name: string) (number: string) : Series option =
    match name |> String.IsNullOrWhiteSpace, number |> String.IsNullOrWhiteSpace with
    | false, false ->
        Some
            { Name = name
              Number = number |> tryParseInt64OrDefault }
    | false, true -> Some { Name = name; Number = 0 }
    | true, false
    | true, true -> None

let internal convertStringToBool (value: string) : bool =
    match value with
    | "0" -> false
    | _ -> true

let internal convertStringToNullableInt (value: string) : Nullable<int> =
    match String.IsNullOrWhiteSpace value with
    | true -> Nullable()
    | false -> value |> int |> Nullable

let convertInpLineToBook (folderName: string) (inpLine: InpLine) : Book =
    { Authors = inpLine.Authors |> makeAuthors authorGroups |> Array.map (mapAuthor authorName)
      Genres = inpLine.Genres |> makeGenres
      Title = inpLine.Title
      Series = (inpLine.Series, inpLine.SeriesNumber) ||> makeSeries
      FileName = inpLine.FileName
      Size = inpLine.Size |> int64
      LibId = inpLine.LibId
      Deleted = inpLine.Deleted |> convertStringToBool
      Extension = inpLine.Extension
      Date = inpLine.Date
      Folder = inpLine.Folder |> Option.defaultValue folderName
      Lang = inpLine.Lang
      LibRate = inpLine.LibRate |> convertStringToNullableInt
      Keywords = inpLine.Keywords |> makeKeywords }
