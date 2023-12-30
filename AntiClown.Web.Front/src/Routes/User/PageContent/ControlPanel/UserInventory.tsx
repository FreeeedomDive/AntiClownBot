import {useParams} from "react-router-dom";
import React from "react";

export default function UserInventory(){
  const { userId } = useParams<"userId">();

  return <div>МОЙ БУДУЩИЙ ИНВЕНТАРЬ {userId}</div>;
}