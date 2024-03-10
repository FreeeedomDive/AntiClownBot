import { useParams } from "react-router-dom";
import React from "react";
import {Typography} from "@mui/material";

export default function UserShop() {
  const { userId } = useParams<"userId">();

  return (<Typography variant={"h5"}>МОЙ БУДУЩИЙ МАГАЗИН {userId}</Typography>)
}