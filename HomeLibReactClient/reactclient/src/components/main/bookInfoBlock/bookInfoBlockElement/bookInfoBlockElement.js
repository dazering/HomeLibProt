import React from "react";
import HttpApiClient from "../../../api/httpApiClient";

const BookInfoBlockElement = (props) => {
  return (
    <div className="text-white">
      <div>
        {props.book.authorships
          .map((authorship) => {
            let a = authorship.author;
            return `${a.lastName} ${a.firstName[0]}.`;
          })
          .join(", ")}{" "}
        - {props.book.title}
      </div>
      <div>
        {props.book.year ? props.book.year : "Год издания неизвестен"} -
        {props.book.isbn ? props.book.isbn : "ISBN неизвестен"}
      </div>
      <br />
      <div>
        {props.book.annotation ? props.book.annotation : "Нет аннотации"}
      </div>
      <div>
        <a
          className="btn btn-primary"
          href={`${process.env.REACT_APP_URL_API}/books/download/${props.book.bookId}`}
        >
          Скачать книгу
        </a>
      </div>
    </div>
  );
};

export default BookInfoBlockElement;
