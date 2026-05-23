module HomeLibProt.CollectionManager.RegEx.RegExResults

open System
open System.Text.RegularExpressions

type RateResult = { BookId: int64; Rate: int64 }

type BookSeriesResult =
    { BookId: int64
      SeriesId: int64
      SeriesNumber: int64 }

type SeriesResult = { Id: int64; Name: string }

type BookGenreResult = { BookId: int64; GenreId: int64 }

type GenreResult =
    { Id: int64; Key: string; Name: string }

type AuthorshipsResult = { BookId: int64; AuthorId: int64 }

type AuthorResult =
    { Id: int64
      FirstName: string
      MiddleName: string
      LastName: string
      FullName: string }

type BookResult =
    { Id: int64
      FileSize: int64
      Date: string
      Title: string
      Lang: string
      Extension: string
      Deleted: bool
      Keywords: string array }

let private removeEscapeSymbol (value: string) : string = value.Replace("\\", String.Empty)

let private makeKeywords (keywords: string) : string array =
    keywords.Split(',', StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries)

let private convertStringToBool (value: string) : bool =
    match value with
    | "0" -> false
    | _ -> true

let private extractGroupValue (group: Group) : string = group.Value

let getRateResult (groups: GroupCollection) : RateResult =
    { BookId = groups.[RegExGroups.Rates.bookId] |> extractGroupValue |> int64
      Rate = groups.[RegExGroups.Rates.rate] |> extractGroupValue |> int64 }

let getBookSeriesResult (groups: GroupCollection) : BookSeriesResult =
    { BookId = groups.[RegExGroups.BookSeries.bookId] |> extractGroupValue |> int64
      SeriesId = groups.[RegExGroups.BookSeries.seriesId] |> extractGroupValue |> int64
      SeriesNumber = groups.[RegExGroups.BookSeries.seriesNumber] |> extractGroupValue |> int64 }

let getSeriesResult (groups: GroupCollection) : SeriesResult =
    { Id = groups.[RegExGroups.Series.seriesId] |> extractGroupValue |> int64
      Name = groups.[RegExGroups.Series.name] |> extractGroupValue |> removeEscapeSymbol }

let getBookGenreResult (groups: GroupCollection) : BookGenreResult =
    { BookId = groups.[RegExGroups.BookGenres.bookId] |> extractGroupValue |> int64
      GenreId = groups.[RegExGroups.BookGenres.genreId] |> extractGroupValue |> int64 }

let getGenreResult (groups: GroupCollection) : GenreResult =
    { Id = groups.[RegExGroups.Genres.genreId] |> extractGroupValue |> int64
      Key = groups.[RegExGroups.Genres.key] |> extractGroupValue
      Name = groups.[RegExGroups.Genres.name] |> extractGroupValue |> removeEscapeSymbol }

let getAuthorshipsResult (groups: GroupCollection) : AuthorshipsResult =
    { BookId = groups.[RegExGroups.Authorships.bookId] |> extractGroupValue |> int64
      AuthorId = groups.[RegExGroups.Authorships.authorId] |> extractGroupValue |> int64 }

let getBookResult (groups: GroupCollection) : BookResult =
    { Id = groups.[RegExGroups.Books.bookId] |> extractGroupValue |> int64
      FileSize = groups.[RegExGroups.Books.fileSize] |> extractGroupValue |> int64
      Date = groups.[RegExGroups.Books.time] |> extractGroupValue
      Title = groups.[RegExGroups.Books.title] |> extractGroupValue |> removeEscapeSymbol
      Lang = groups.[RegExGroups.Languages.lang] |> extractGroupValue
      Extension = groups.[RegExGroups.Books.fileType] |> extractGroupValue
      Deleted = groups.[RegExGroups.Books.deleted] |> extractGroupValue |> convertStringToBool
      Keywords =
        groups.[RegExGroups.Keywords.keywords]
        |> extractGroupValue
        |> makeKeywords
        |> Array.map removeEscapeSymbol }

let getAuthorResult (groups: GroupCollection) : AuthorResult =
    { Id = groups.[RegExGroups.Authors.authorId] |> extractGroupValue |> int64
      FirstName =
        groups.[RegExGroups.Authors.firstName]
        |> extractGroupValue
        |> removeEscapeSymbol
      MiddleName =
        groups.[RegExGroups.Authors.middleName]
        |> extractGroupValue
        |> removeEscapeSymbol
      LastName = groups.[RegExGroups.Authors.lastName] |> extractGroupValue |> removeEscapeSymbol
      FullName = String.Empty }
