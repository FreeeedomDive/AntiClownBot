import {useParams} from "react-router-dom";
import React from "react";
import {Typography} from "@mui/material";

export default function UserInventory(){
  const { userId } = useParams<"userId">();

  return (<Typography variant={"h5"}>МОЙ БУДУЩИЙ ИНВЕНТАРЬ {userId}</Typography>)
}