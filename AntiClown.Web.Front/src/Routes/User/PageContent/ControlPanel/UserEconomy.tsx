import { useParams } from "react-router-dom";
import React from "react";
import {Typography} from "@mui/material";

export default function UserEconomy() {
  const { userId } = useParams<"userId">();

  return (<Typography variant={"h5"}>МОЯ БУДУЩАЯ ЭКОНОМИКА {userId}</Typography>)
}