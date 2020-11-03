import React from "react";
import { NavLink, withRouter } from "react-router-dom";

const alphabetSearchElement = (props) => {
  const pathParam = encodeURIComponent(`${props.firstLiterals}`);
  return (
    <NavLink
      to={
        props.count > 0 && props.count < 50
          ? `/authors?name=${pathParam}`
          : `/?literals=${pathParam}`
      }
      activeClassName={""}
      className="list-group-item list-group-item-action d-flex justify-content-between align-items-center"
      onClick={props.chooseLiterals}
    >
      {props.firstLiterals}{" "}
      <span className="badge badge-primary">{props.count}</span>
    </NavLink>
  );
};

export default withRouter(alphabetSearchElement);
