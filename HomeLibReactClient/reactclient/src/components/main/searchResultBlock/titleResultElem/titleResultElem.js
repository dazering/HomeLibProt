import React from "react";
import { NavLink } from "react-router-dom";

const TitleResultElem = (props) => {
  return (
    <NavLink
      to={`/book/${props.book.bookId}`}
      className="list-group-item list-group-item-action text-center"
    >
      {props.book.title}
    </NavLink>
  );
};

export default TitleResultElem;
