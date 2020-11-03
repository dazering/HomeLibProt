import React from "react";
import { Redirect, Route, Switch } from "react-router-dom";
import AlphabetSearchBlock from "./alphabetSearchBlock/alphabetSearchBlock";
import AuthorsBlock from "./authorsBlock/authorsBlock";
import BookInfoBlock from "./bookInfoBlock/bookInfoBlock";
import BooksBlock from "./booksBlock/booksBlock";
import SearchResultBlock from "./searchResultBlock/searchResultBlock";

const main = () => {
  return (
    <main className="bg-secondary p-1 rounded h-auto my-3">
      <Switch>
        <Route path="/authors:name?" component={AuthorsBlock}></Route>
        <Route path="/books/:authorId" component={BooksBlock}></Route>
        <Route path="/book/:bookId" component={BookInfoBlock}></Route>
        <Route path="/search" exact component={SearchResultBlock}></Route>
        <Route path="/:literals?" exact component={AlphabetSearchBlock}></Route>

        <Redirect to="/"></Redirect>
      </Switch>
    </main>
  );
};

export default main;
