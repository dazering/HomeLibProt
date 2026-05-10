module HomeLibProt.CollectionManager.RegEx.RegExResults

open System
open System.Text.RegularExpressions

type AuthorResult =
    { Id: int64
      FirstName: string
      MiddleName: string
      LastName: string
      FullName: string }

let private extractGroupValue (group: Group) : string = group.Value

let getAuthorResult (groups: GroupCollection) : AuthorResult =
    { Id = groups.[RegExGroups.Authors.authorId] |> extractGroupValue |> int64
      FirstName = groups.[RegExGroups.Authors.firstName] |> extractGroupValue
      MiddleName = groups.[RegExGroups.Authors.middleName] |> extractGroupValue
      LastName = groups.[RegExGroups.Authors.lastName] |> extractGroupValue
      FullName = String.Empty }
