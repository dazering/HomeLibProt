module HomeLibProt.CollectionManager.RegEx.Librusec

open RegExGroups

let authors =
    $@"\((?<{Authors.authorId}>\d*),'(?<{Authors.firstName}>.*)','(?<{Authors.middleName}>.*)','(?<{Authors.lastName}>.*)','(?<nickname>.*)','(?<noDonate>.*)',(?<uid>\d*),'(?<email>.*)','(?<homepage>.*)','(?<blocked>.*)','(?<public>.*)','(?<pna>.*)','(?<pnb>.*)','(?<pnc>.*)','(?<pnd>.*)','(?<pnf>.*)','(?<png>.*)','(?<lang>.*)',(?<main>\d*),'(?<gender>.*)',(?<books>\d*)\)"

let authorships =
    $@"\((?<{Authorships.bookId}>\d*),(?<{Authorships.authorId}>\d*),'(?<role>.*)'\)"

let bookGenres = $@"\((?<{BookGenres.bookId}>\d*),(?<{BookGenres.genreId}>\d*)\)"

let books =
    $@"\((?<{Books.bookId}>\d*),(?<{Books.fileSize}>\d*),'(?<{Books.time}>\d\d\d\d-\d\d-\d\d) \d\d:\d\d:\d\d','(?<{Books.title}>.*)','(?<title1>.*)','(?<{Languages.lang}>.*)','(?<srcLang>.*)','(?<{Books.fileType}>.*)',(?<year>\d*),(?<year1>\d*),'(?<{Books.deleted}>.*)','(?<ver>.*)','(?<fileAuthor>.*)','(?<{Keywords.keywords}>.*)','(?<blocked>.*)','(?<md5>.*)','(?<broken>.*)','(?<modified>.*)',(?<authors>\d*),(?<replacedBy>\d*),(?<pages>\d*),'(?<metaphone>.*)'\)"

let bookSeries =
    $@"\((?<{BookSeries.bookId}>\d*),(?<{BookSeries.seriesId}>\d*),(?<{BookSeries.seriesNumber}>\d*)\.\d*,(?<sort>\d*)\)"

let genres =
    $@"\((?<{Genres.genreId}>\d*),'(?<{Genres.key}>.*)','(?<{Genres.name}>.*)','(?<edesc>.*)',(?<gidm>\d*)\)"

let series =
    $@"\((?<{Series.seriesId}>\d*),'(?<{Series.name}>.*)',(?<parentId>\d*),(?<parentNumber>\d*),(?<good>\d*),'(?<lang>.*)','(?<type>.*)',(?<publisherId>\d*)\)"

let rates =
    $@"\((?<{Rates.bookId}>\d*),(?<userId>\d*),'(?<{Rates.rate}>[^']*)','(?<time>[^']*)'\)"
