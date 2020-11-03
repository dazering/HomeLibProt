import React from "react";
import SearchBar from "./search";
import Navbar from './navbar'
const header = () => {
  return (
    <header className="sticky-top mb-3 bg-secondary rounded-bottom">
      <div className="container-fluid">
        <div className="row align-items-center justify-content-center">
          <Navbar/>
          <SearchBar />
        </div>
      </div>
    </header>
  );
};
export default header;
