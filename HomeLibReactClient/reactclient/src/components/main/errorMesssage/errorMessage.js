import React from "react";
import './errorMessage.css'
const ErrorMessage = (props) => (
  <div className="bg-warning error text-center">{props.message}</div>
);

export default ErrorMessage;
