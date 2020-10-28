class HttpApiClient {
  GetBookById(id) {
    return fetch(`${process.env.REACT_APP_URL_API}/books/book/${id}`);
  }

  SearchBookByTitle(title) {
    return fetch(`${process.env.REACT_APP_URL_API}/books/search/${title}`);
  }

  DownloadBook(id) {
    return fetch(`${process.env.REACT_APP_URL_API}/books/download/${id}`);
  }

  GetBooksByAuthorId(id) {
    return fetch(`${process.env.REACT_APP_URL_API}/authors/author/${id}`);
  }

  SearchFirstLiteralsOfAuthorsName(literals = "") {
    return fetch(`${process.env.REACT_APP_URL_API}/authors/${literals}`);
  }

  GetAuthorsByFirstLiterals(name) {
    return fetch(`${process.env.REACT_APP_URL_API}/authors/getauthors/${name}`);
  }

  SearchAuthorByName(name) {
    return fetch(`${process.env.REACT_APP_URL_API}/authors/search/${name}`);
  }
}

export default HttpApiClient;
