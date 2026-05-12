module HomeLibProt.CollectionManager.RegEx.Flibusta

open RegExGroups

let authors =
    $@"\((?<{Authors.authorId}>\d*),'(?<{Authors.firstName}>.*)','(?<{Authors.middleName}>.*)','(?<{Authors.lastName}>.*)','(?<nickname>.*)',(?<uid>\d*),'(?<email>.*)','(?<homepage>.*)','(?<gender>.*)',(?<masterId>\d*)\)"

let authorships =
    $@"\((?<{Authorships.bookId}>\d*),(?<{Authorships.authorId}>\d*),(?<pos>\d*)\)"

let bookGenres =
    $@"\((?<id>\d*),(?<{BookGenres.bookId}>\d*),(?<{BookGenres.genreId}>\d*)\)"

let books =
    $@"\((?<{Books.bookId}>\d*),(?<{Books.fileSize}>\d*),'(?<{Books.time}>\d\d\d\d-\d\d-\d\d) \d\d:\d\d:\d\d','(?<{Books.title}>.*)','(?<title1>.*)','(?<{Languages.lang}>.*)',(?<langEx>\d*),'(?<srcLang>.*)','(?<{Books.fileType}>.*)','(?<encoding>.*)',(?<year>\d*),'(?<{Books.deleted}>.*)','(?<ver>.*)','(?<fileAuthor>.*)',(?<n>\d*),'(?<{Keywords.keywords}>.*)','(?<md5>.*)','(?<modified>.*)','(?<pmd5>.*)',(?<infoCode>\d*),(?<pages>\d*),(?<chars>\d*)\)"

let bookSeries =
    $@"\((?<{BookSeries.bookId}>\d*),(?<{BookSeries.seriesId}>\d*),(?<{BookSeries.seriesNumber}>\d*),(?<level>\d*),(?<type>\d*)\)"

let genres =
    $@"\((?<{Genres.genreId}>\d*),'(?<{Genres.key}>.*)','(?<{Genres.name}>.*)','(?<meta>.*)'\)"

let series = $@"\((?<{Series.seriesId}>\d*),'(?<{Series.name}>.*)'\)"

let rates =
    $@"\((?<id>\d*),(?<{Rates.bookId}>\d*),(?<userId>\d*),'(?<{Rates.rate}>[^']*)'\)"
