class HttpApiClient {
  static GetBookById(id) {
    return fetch(`${process.env.REACT_APP_URL_API}/books/book/${id}`);
  }

  static SearchBookByTitle(title) {
    return fetch(`${process.env.REACT_APP_URL_API}/books/search/${title}`);
  }

  static DownloadBook(id) {
    return fetch(`${process.env.REACT_APP_URL_API}/books/download/${id}`);
  }

  static GetBooksByAuthorId(id) {
    return fetch(`${process.env.REACT_APP_URL_API}/authors/author/${id}`);
  }

  static SearchFirstLiteralsOfAuthorsName(literals = "") {
    return fetch(`${process.env.REACT_APP_URL_API}/authors/${literals}`);
  }

  static GetAuthorsByFirstLiterals(name) {
    return fetch(`${process.env.REACT_APP_URL_API}/authors/getauthors/${name}`);
  }

  static SearchAuthorByName(name) {
    return fetch(`${process.env.REACT_APP_URL_API}/authors/search/${name}`);
  }
}

export default HttpApiClient;
