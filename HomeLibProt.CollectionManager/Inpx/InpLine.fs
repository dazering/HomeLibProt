module HomeLibProt.CollectionManager.Inpx.InpLine

type AuthorName =
    { First: string
      Middle: string
      Last: string }

type InpRecord =
    { AuthorNames: AuthorName array
      Genres: string array
      Title: string
      Series: string
      SeriesNumber: string
      FileName: string
      Size: string
      LibId: string
      Del: string
      Extension: string
      Date: string
      Lang: string
      LibRate: string
      Keywords: string array }

[<Literal>]
let InpSeparator = "\x04"

[<Literal>]
let private EnumerationSeparator = ":"

let private appendEnumerationSeparator (value: string) : string =
    sprintf $"{value}{EnumerationSeparator}"

let private makeEnumeration (separator: string) (values: string array) : string = values |> String.concat separator

let private makeAuthorsEnumeration (authorNames: AuthorName array) : string =
    authorNames
    |> Array.map (fun an -> sprintf $"{an.Last},{an.First},{an.Middle}")
    |> makeEnumeration EnumerationSeparator

let makeLine (record: InpRecord) : string =
    let values =
        [| record.AuthorNames |> makeAuthorsEnumeration |> appendEnumerationSeparator
           record.Genres
           |> makeEnumeration EnumerationSeparator
           |> appendEnumerationSeparator
           record.Title
           record.Series
           record.SeriesNumber
           record.FileName
           record.Size
           record.LibId
           record.Del
           record.Extension
           record.Date
           record.Lang
           record.LibRate
           record.Keywords |> makeEnumeration ", " |]

    let line = values |> String.concat InpSeparator
    sprintf $"{line}{InpSeparator}"
