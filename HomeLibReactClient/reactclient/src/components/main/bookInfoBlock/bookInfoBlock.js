import React, { useEffect, useState } from "react";
import HttpApiClient from "../../api/httpApiClient";
import BookInfoBlockElem from "./bookInfoBlockElement/bookInfoBlockElement";
import ErrorMessage from "../errorMesssage/errorMessage";
import Loader from "../../loader/loader";

const BookInfoBlock = (props) => {
  const [bookInfo, setBookInfo] = useState({});
  const [cleanUp, setCleanUp] = useState(false);

  useEffect(() => {
    const getBookInfo = (bookId) => {
      HttpApiClient.GetBookById(bookId)
        .then((res) => {
          if (res.ok) {
            res.json().then((data) => {
              setBookInfo({
                elem: (
                  <BookInfoBlockElem
                    key={0}
                    book={data["data"]}
                  ></BookInfoBlockElem>
                ),
              });
            });
          } else {
            res.json().then((data) => {
              setBookInfo({
                elem: <ErrorMessage key={0} message={data["data"]} />,
              });
            });
          }
        })
        .catch(() => {
          setBookInfo({
            elem: <ErrorMessage key={0} message={"Что-то не работает..."} />,
          });
        });
    };

    if (!cleanUp) {
      getBookInfo(props.match.params.bookId);
    }

    return () => {
      setCleanUp(true);
      setBookInfo({});
    };
  }, [props, cleanUp]);

  return <div>{bookInfo.elem ? bookInfo.elem : <Loader />}</div>;
};

export default BookInfoBlock;
