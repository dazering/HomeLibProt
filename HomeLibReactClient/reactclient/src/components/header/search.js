import React, { useRef, useState } from "react";
import { NavLink } from "react-router-dom";

const SearchBar = () => {
  const [searchType, setSearchType] = useState("title");
  const [searchTerm, setSearchTerm] = useState("");
  const inputTxtRef = useRef();
  return (
    <div className="p-0 col-12 col-sm-12 col-md-8 col-lg-6 col-xl-5 mr-0 ml-auto">
      <form className=" form-inline justify-content-center">
        <div className="row w-100  align-items-center">
          <div className="p-0 col-12 col-sm-5 col-md-5 col-lg-5 col-xl-5">
            <input
              className="form-control w-100"
              name="term"
              type="text"
              placeholder="Search..."
              ref={inputTxtRef}
              onChange={() => setSearchTerm(inputTxtRef.current.value)}
            />
          </div>
          <div className="p-0 col-8 col-sm-5 col-md-5 col-lg-5 col-xl-5 mx-auto">
            <div>
              <div className="form-check-inline">
                <input
                  id="radio-term-title"
                  defaultChecked
                  type="radio"
                  name="name"
                  value="Title"
                  className="mx-1"
                  onClick={() => setSearchType("title")}
                />
                <label htmlFor="radio-term-title">By Title</label>
              </div>
              <div className="form-check-inline">
                <input
                  id="radio-term-name"
                  type="radio"
                  name="name"
                  className="mx-1"
                  onClick={() => setSearchType("name")}
                />
                <label htmlFor="radio-term-name">By Name</label>
              </div>
            </div>
          </div>
          <div className="p-0 col-4 col-sm-2 col-md-2 col-lg-2 col-xl-2">
            <NavLink
              to={`/search?type=${searchType}&searchTerm=${searchTerm}`}
              className="btn btn-primary w-100"
              onClick={() => {
                setSearchTerm("");
                inputTxtRef.current.value = "";
              }}
            >
              Search!
            </NavLink>
          </div>
        </div>
      </form>
    </div>
  );
};

export default SearchBar;
