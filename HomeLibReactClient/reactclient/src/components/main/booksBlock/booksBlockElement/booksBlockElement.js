import React from "react";
import { NavLink, withRouter } from "react-router-dom";

const BooksBlockElement = (props) => {
  return (
    <NavLink
      className="list-group-item list-group-item-action text-center"
      to={`/book/${props.bookId}`}
    >
      {props.title}
    </NavLink>
  );
};

export default withRouter(BooksBlockElement);
