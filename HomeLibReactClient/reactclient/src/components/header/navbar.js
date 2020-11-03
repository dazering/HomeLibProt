import React from "react";
import { NavLink, withRouter } from "react-router-dom";

const navbar = () => {
  return (
    <div className="p-0 col-12 col-sm-12 col-md-3 col-lg-3 col-xl-3 mr-auto">
      <nav className="bg-primary nav nav-justified">
        <NavLink
          to="/"
          className="nav-item nav-link text-white"
        >
          HomeLib
        </NavLink>
      </nav>
    </div>
  );
};

export default withRouter(navbar);
