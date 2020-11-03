import React, { useEffect, useState } from "react";
import query from "query-string";
import { useLocation } from "react-router-dom";
import HttpApiClient from "../../api/httpApiClient";
import TitleResultElem from "./titleResultElem/titleResultElem";
import Loader from "../../loader/loader";
import ErrorMessage from "../errorMesssage/errorMessage";
import NameResultElem from "./nameResultElem/nameResultElem";

const SearchResultBlock = () => {
  const [searchResultElems, setSearchResultElems] = useState({});
  const { type, searchTerm } = query.parse(useLocation().search);
  const [cleanUp, setCleanUp] = useState(false);

  useEffect(() => {
    const searchByTitle = (title) => {
      HttpApiClient.SearchBookByTitle(title)
        .then((res) => {
          if (res.ok) {
            res.json().then((data) => {
              let books = data["data"];
              setSearchResultElems({
                elems: books.map((book) => {
                  return <TitleResultElem book={book} />;
                }),
              });
            });
          } else {
            setSearchResultElems({
              elems: <ErrorMessage key={0} message={"Никого не найдено..."} />,
            });
          }
        })
        .catch(() => {
          setSearchResultElems({
            elems: <ErrorMessage key={0} message={"Что-то не работает..."} />,
          });
        });
    };

    const searchByAuthorName = (name) => {
      HttpApiClient.SearchAuthorByName(name)
        .then((res) => {
          if (res.ok) {
            res.json().then((data) => {
              let authors = data["data"];
              setSearchResultElems({
                elems: authors.map((a) => {
                  return <NameResultElem author={a} />;
                }),
              });
            });
          } else {
            setSearchResultElems({
              elems: <ErrorMessage key={0} message={"Никого не найдено..."} />,
            });
          }
        })
        .catch(() => {
          setSearchResultElems({
            elems: <ErrorMessage key={0} message={"Что-то не работает..."} />,
          });
        });
    };
    console.log(searchTerm + " " + type);
    if (!cleanUp && searchTerm) {
      if (type === "title") {
        searchByTitle(searchTerm);
      } else if (type === "name") {
        searchByAuthorName(searchTerm);
      }
    }
    return () => {
      setCleanUp(true);
      setSearchResultElems({});
    };
  }, [cleanUp, searchTerm, type]);

  return (
    <div>{searchResultElems.elems ? searchResultElems.elems : <Loader />}</div>
  );
};

export default SearchResultBlock;
