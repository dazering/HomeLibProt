import React, { useEffect } from "react";
import HttpApiClient from "../../api/httpApiClient";
import Loader from "../../loader/loader";
import AuthorsBlockElement from "./authorsBlockElement/authorsBlockElement";
import ErrorMessage from "../errorMesssage/errorMessage";
import { useLocation } from "react-router-dom";
import queryString from "query-string";

const AuthorsBlock = (props) => {
  const [authorsElems, setAuthorsElems] = React.useState({});
  const [cleanUp, setCleanUp] = React.useState(false);
  const { name } = queryString.parse(useLocation().search);
  useEffect(() => {
    const getAuthors = (name) => {
      HttpApiClient.GetAuthorsByFirstLiterals(encodeURIComponent(name))
        .then((res) => {
          if (res.ok) {
            res.json().then((data) => {
              let authorships = data["data"];
              if (authorships.length > 0) {
                setAuthorsElems({
                  elems: data["data"].map((auth, i) => (
                    <AuthorsBlockElement
                      key={i}
                      authorId={auth.authorId}
                      name={auth.fullName}
                      count={auth.authorships.length}
                    />
                  )),
                });
              }
              else{
                setAuthorsElems({
                  elems: <ErrorMessage key={0} message={'Никого не найдено...'} />,
                });
              }
            });
          } else {
            res.json().then((data) => {
              setAuthorsElems({
                elems: <ErrorMessage key={0} message={data["data"]} />,
              });
            });
          }
        })
        .catch(() => {
          setAuthorsElems({
            elems: <ErrorMessage key={0} message={"Что-то не работает..."} />,
          });
        });
    };

    if (!cleanUp) {
      getAuthors(decodeURIComponent(name));
    }

    return () => {
      setCleanUp(true);
      setAuthorsElems({});
    };
  }, [props, cleanUp, name]);
  return <div>{authorsElems.elems ? authorsElems.elems : <Loader />}</div>;
};

export default AuthorsBlock;
