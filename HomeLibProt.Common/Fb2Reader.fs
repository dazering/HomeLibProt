module HomeLibProt.Common.Fb2Reader

open System
open System.IO
open System.Xml
open System.Xml.XPath

type Fb2Attributes =
    { Annotation: string option
      Coverpage: string option }

type AuthorName =
    { First: string
      Middle: string
      Last: string }

type Series = { Name: string; Number: string }

type Fb2Info =
    { Authors: AuthorName array
      Genres: string array
      Title: string
      Language: string
      Keywords: string array
      Series: Series array }

let private authorsXpath = "/fb:FictionBook/fb:description/fb:title-info/fb:author"

let private genresXpath = "/fb:FictionBook/fb:description/fb:title-info/fb:genre"

let private titleXpath =
    "/fb:FictionBook/fb:description/fb:title-info/fb:book-title"

let private langXpath = "/fb:FictionBook/fb:description/fb:title-info/fb:lang"

let private keywordsXpath =
    "/fb:FictionBook/fb:description/fb:title-info/fb:keywords"

let private seriesXpath = "/fb:FictionBook/fb:description/fb:title-info/fb:sequence"

let private annotationXpath =
    "/fb:FictionBook/fb:description/fb:title-info/fb:annotation"

let private coverpageLinkXpath =
    "/fb:FictionBook/fb:description/fb:title-info/fb:coverpage/fb:image"

let private normalizeString (value: string) =
    value.Replace("\r\n", String.Empty).Replace("\n", String.Empty)

let private makeKeywords (keywords: string) : string array =
    keywords.Split(',', StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries)

let private tryToRemoveNamespace (value: string option) : string option =
    match value with
    | Some v ->
        v.Replace(" xmlns=\"http://www.gribuser.ru/xml/fictionbook/2.0\"", String.Empty)
        |> Some
    | None -> None

let private tryToReplaceIdSing (id: string option) : string option =
    match id with
    | Some i -> i.Replace("#", String.Empty) |> Some
    | None -> None

let private tryToGetCoverpageXpath (id: string option) : string option =
    match id with
    | Some i -> Some $"/fb:FictionBook/fb:binary[@id='{i}']"
    | None -> None

let private tryToSelectValue (navigator: XPathNavigator option) : string option =
    match navigator with
    | Some n -> Some n.Value
    | None -> None

let private tryToSelectInnerXml (navigator: XPathNavigator option) : string option =
    match navigator with
    | Some n -> Some n.InnerXml
    | None -> None

let private tryToSelectSingleNode
    (navigator: XPathNavigator)
    (namespaceManager: XmlNamespaceManager)
    (xpath: string)
    : XPathNavigator option =
    match navigator.SelectSingleNode(xpath, namespaceManager) with
    | null -> None
    | xPathNavigator -> Some xPathNavigator

let private tryToGetValue (navigator: XPathNavigator option) : string =
    match navigator with
    | Some n -> n.Value
    | None -> String.Empty

let private tryToSelect
    (navigator: XPathNavigator)
    (namespaceManager: XmlNamespaceManager)
    (xpath: string)
    : XPathNodeIterator option =
    match navigator.Select(xpath, namespaceManager) with
    | null -> None
    | iterator -> Some iterator

let private tryToSelectAttributeNode
    (xpath: string)
    (namespaceManager: XmlNamespaceManager)
    (navigator: XPathNavigator option)
    : XPathNavigator option =
    match navigator with
    | Some n -> xpath |> tryToSelectSingleNode n namespaceManager
    | None -> None

let private tryToSelectSeries
    (namespaceManager: XmlNamespaceManager)
    (iterator: XPathNodeIterator option)
    : Series seq option =
    match iterator with
    | Some i ->
        seq {
            while i.MoveNext() do
                let name =
                    tryToSelectAttributeNode "@name" namespaceManager (Some i.Current)
                    |> tryToSelectValue
                    |> Option.defaultValue String.Empty
                    |> normalizeString

                let number =
                    tryToSelectAttributeNode "@number" namespaceManager (Some i.Current)
                    |> tryToSelectValue
                    |> Option.defaultValue String.Empty
                    |> normalizeString

                match name |> String.IsNullOrEmpty, number |> String.IsNullOrEmpty with
                | false, true -> yield { Name = name; Number = "0" }
                | _, _ -> yield { Name = name; Number = number }
        }
        |> Some
    | None -> None

let private tryToSelectKeywords (iterator: XPathNodeIterator option) : string seq option =
    match iterator with
    | Some i ->
        seq {
            while i.MoveNext() do
                let keywords = $"{i.Current}"
                yield keywords |> normalizeString
        }
        |> Some
    | None -> None

let private tryToSelectGenres (iterator: XPathNodeIterator option) : string seq option =
    match iterator with
    | Some i ->
        seq {
            while i.MoveNext() do
                let genre = $"{i.Current}"
                yield genre |> normalizeString
        }
        |> Some
    | None -> None

let private tryToSelectAuthors
    (namespaceManager: XmlNamespaceManager)
    (iterator: XPathNodeIterator option)
    : AuthorName seq option =
    match iterator with
    | Some i ->
        seq {
            while i.MoveNext() do
                let firstName =
                    "fb:first-name"
                    |> tryToSelectSingleNode i.Current namespaceManager
                    |> tryToGetValue
                    |> normalizeString

                let middleName =
                    "fb:middle-name"
                    |> tryToSelectSingleNode i.Current namespaceManager
                    |> tryToGetValue
                    |> normalizeString

                let lastName =
                    "fb:last-name"
                    |> tryToSelectSingleNode i.Current namespaceManager
                    |> tryToGetValue
                    |> normalizeString

                yield
                    { First = firstName
                      Middle = middleName
                      Last = lastName }
        }
        |> Some
    | None -> None

let private tryToGetCoverpageNode
    (navigator: XPathNavigator)
    (namespaceManager: XmlNamespaceManager)
    (xpath: string option)
    : XPathNavigator option =
    match xpath with
    | Some x -> x |> tryToSelectSingleNode navigator namespaceManager
    | None -> None

let private readFb2 (fb2: Stream) (action: XPathNavigator -> XmlNamespaceManager -> 'T) : 'T option =
    try
        let doc = new XPathDocument(fb2)

        let navigator = doc.CreateNavigator()

        let namespaceManager = new XmlNamespaceManager(navigator.NameTable)
        namespaceManager.AddNamespace("fb", "http://www.gribuser.ru/xml/fictionbook/2.0")
        namespaceManager.AddNamespace("l", "http://www.w3.org/1999/xlink")

        action navigator namespaceManager |> Some
    with :? XmlException ->
        None

let getFb2Attributes (fb2: Stream) : Fb2Attributes option =
    readFb2 fb2 (fun navigator namespaceManager ->
        let annotation =
            annotationXpath
            |> tryToSelectSingleNode navigator namespaceManager
            |> tryToSelectInnerXml
            |> tryToRemoveNamespace

        let coverpage =
            coverpageLinkXpath
            |> tryToSelectSingleNode navigator namespaceManager
            |> tryToSelectAttributeNode "@l:href" namespaceManager
            |> tryToSelectValue
            |> tryToReplaceIdSing
            |> tryToGetCoverpageXpath
            |> tryToGetCoverpageNode navigator namespaceManager
            |> tryToSelectValue

        { Annotation = annotation
          Coverpage = coverpage })

let getFb2Info (fb2: Stream) : Fb2Info option =
    readFb2 fb2 (fun navigator namespaceManager ->
        let authors =
            authorsXpath
            |> tryToSelect navigator namespaceManager
            |> tryToSelectAuthors namespaceManager
            |> Option.defaultValue Seq.empty
            |> Seq.toArray

        let genres =
            genresXpath
            |> tryToSelect navigator namespaceManager
            |> tryToSelectGenres
            |> Option.defaultValue Seq.empty
            |> Seq.toArray

        let title =
            titleXpath
            |> tryToSelectSingleNode navigator namespaceManager
            |> tryToGetValue
            |> normalizeString

        let language =
            langXpath
            |> tryToSelectSingleNode navigator namespaceManager
            |> tryToGetValue
            |> normalizeString

        let keywords =
            keywordsXpath
            |> tryToSelect navigator namespaceManager
            |> tryToSelectKeywords
            |> Option.defaultValue Seq.empty
            |> Seq.collect makeKeywords
            |> Seq.toArray

        let series =
            seriesXpath
            |> tryToSelect navigator namespaceManager
            |> tryToSelectSeries namespaceManager
            |> Option.defaultValue (
                seq {
                    yield
                        { Name = String.Empty
                          Number = String.Empty }
                }
            )
            |> Seq.toArray

        { Authors = authors
          Genres = genres
          Title = title
          Language = language
          Keywords = keywords
          Series = series })
