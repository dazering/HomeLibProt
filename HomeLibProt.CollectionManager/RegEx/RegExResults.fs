module HomeLibProt.CollectionManager.RegEx.RegExResults

open System
open System.Text.RegularExpressions
open System.Globalization

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

let formatDate (dateTime: string) =
    DateTime.ParseExact(dateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd")

let private makeKeywords (keywords: string) : string array =
    keywords.Split(',', StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries)

let private convertStringToBool (value: string) : bool =
    match value with
    | "0" -> false
    | _ -> true

let private extractGroupValue (group: Group) : string = group.Value

let getBookGenreResult (groups: GroupCollection) : BookGenreResult =
    { BookId = groups.[RegExGroups.BookGenres.bookId] |> extractGroupValue |> int64
      GenreId = groups.[RegExGroups.BookGenres.genreId] |> extractGroupValue |> int64 }

let getGenreResult (groups: GroupCollection) : GenreResult =
    { Id = groups.[RegExGroups.Genres.genreId] |> extractGroupValue |> int64
      Key = groups.[RegExGroups.Genres.key] |> extractGroupValue
      Name = groups.[RegExGroups.Genres.name] |> extractGroupValue }

let getAuthorshipsResult (groups: GroupCollection) : AuthorshipsResult =
    { BookId = groups.[RegExGroups.Authorships.bookId] |> extractGroupValue |> int64
      AuthorId = groups.[RegExGroups.Authorships.authorId] |> extractGroupValue |> int64 }

let getBookResult (groups: GroupCollection) : BookResult =
    { Id = groups.[RegExGroups.Books.bookId] |> extractGroupValue |> int64
      FileSize = groups.[RegExGroups.Books.fileSize] |> extractGroupValue |> int64
      Date = groups.[RegExGroups.Books.time] |> extractGroupValue |> formatDate
      Title = groups.[RegExGroups.Books.title] |> extractGroupValue
      Lang = groups.[RegExGroups.Languages.lang] |> extractGroupValue
      Extension = groups.[RegExGroups.Books.fileType] |> extractGroupValue
      Deleted = groups.[RegExGroups.Books.deleted] |> extractGroupValue |> convertStringToBool
      Keywords = groups.[RegExGroups.Keywords.keywords] |> extractGroupValue |> makeKeywords }

let getAuthorResult (groups: GroupCollection) : AuthorResult =
    { Id = groups.[RegExGroups.Authors.authorId] |> extractGroupValue |> int64
      FirstName = groups.[RegExGroups.Authors.firstName] |> extractGroupValue
      MiddleName = groups.[RegExGroups.Authors.middleName] |> extractGroupValue
      LastName = groups.[RegExGroups.Authors.lastName] |> extractGroupValue
      FullName = String.Empty }
