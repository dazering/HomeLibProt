import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import HttpApiClient from "../../api/httpApiClient";
import Loader from "../../loader/loader";
import BookBlockElement from "./booksBlockElement/booksBlockElement";
import ErrorMessage from "../errorMesssage/errorMessage";

const BooksBlock = (props) => {
  const [booksBlockElems, setBookBlockElems] = useState({});
  const [cleanUp, setCleanUp] = useState(false);
  const { authorId } = useParams();
  useEffect(() => {
    const getBooks = (authorId) => {
      HttpApiClient.GetBooksByAuthorId(authorId)
        .then((res) => {
          if (res.ok) {
            res.json().then((data) => {
              let books = data["data"].authorships;
              setBookBlockElems({
                elems: books.map((book, i) => (
                  <BookBlockElement
                    key={i}
                    title={book.book.title}
                    bookId={book.bookId}
                  />
                )),
              });
            });
          } else {
            res.json().then((data) => {
              setBookBlockElems({
                elems: <ErrorMessage key={0} message={data["data"]} />,
              });
            });
          }
        })
        .catch((err) => {
          console.log(err);
          setBookBlockElems({
            elems: <ErrorMessage key={0} message={"Что-то не работает..."} />,
          });
        });
    };

    if (!cleanUp) {
      getBooks(authorId);
    }

    return () => {
      setBookBlockElems({});
      setCleanUp(true);
    };
  }, [cleanUp, authorId]);

  return (
    <div>{booksBlockElems.elems ? booksBlockElems.elems : <Loader />}</div>
  );
};

export default BooksBlock;
