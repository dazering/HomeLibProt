import React from "react";
import { NavLink } from "react-router-dom";

const AuthorsBlockElement = (props) => (
  <NavLink
    to={"/books/" + props.authorId}
    className="list-group-item list-group-item-action d-flex justify-content-between align-items-center"
    onClick={props.getBooks}
  >
    {props.name} <span className="badge badge-primary">{props.count}</span>
  </NavLink>
);

export default AuthorsBlockElement;
