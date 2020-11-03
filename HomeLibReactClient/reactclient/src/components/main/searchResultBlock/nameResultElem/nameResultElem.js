import React from "react";
import { NavLink } from "react-router-dom";

const NameResultElem = (props) => {
    console.log(props);
  return (
    <NavLink
      to={`/books/${props.author.authorId}`}
      className="list-group-item list-group-item-action text-center"
    >
      {props.author.fullName}
    </NavLink>
  );
};

export default NameResultElem;
