import React, { useEffect } from "react";
import apiCLient from "../../api/httpApiClient";
import AlphabetSearchElement from "./alphabetSearchElement/alpabeltSearchElement";
import Loader from "../../loader/loader";
import ErrorMessage from "../errorMesssage/errorMessage";
import { useLocation, withRouter } from "react-router-dom";
import queryString from "query-string";

const AlpabeltSearchBlock = (props) => {
  const [alphbetsElems, setAlphabetsElems] = React.useState({});
  const [cleanUp, setCleanUp] = React.useState(false);
  const { literals } = queryString.parse(useLocation().search);

  useEffect(() => {
    const getAlphabets = (literals = "") => {
      apiCLient
        .SearchFirstLiteralsOfAuthorsName(literals)
        .then((res) => {
          if (res.ok) {
            res.json().then((data) => {
              if (data["data"].length > 0) {
                setAlphabetsElems({
                  elems: data["data"].map((elem, i) => (
                    <AlphabetSearchElement
                      key={i}
                      firstLiterals={elem.alphabets}
                      count={elem.count}
                      chooseLiterals={() => getAlphabets(elem.alphabets)}
                    />
                  )),
                });
              }
              else{
                setAlphabetsElems({
                  elems: <ErrorMessage key={0} message={'Ничего не найдено...'} />,
                });
              }
            });
          } else
            return res.json().then((data) => {
              setAlphabetsElems({
                elems: <ErrorMessage key={0} message={data["data"]} />,
              });
            });
        })
        .catch((err) => {
          console.log(err);
          setAlphabetsElems({
            elems: <ErrorMessage key={0} message={"Что-то не работает..."} />,
          });
        });
    };

    if (!cleanUp) {
      getAlphabets(literals);
    }
    return () => {
      setCleanUp(true);
      setAlphabetsElems({});
    };
  }, [cleanUp, literals]);

  return <div>{alphbetsElems.elems ? alphbetsElems.elems : <Loader />}</div>;
};

export default withRouter(AlpabeltSearchBlock);
