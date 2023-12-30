import { useParams } from "react-router-dom";
import React from "react";

export default function UserShop() {
  const { userId } = useParams<"userId">();

  return <div>МОЙ БУДУЩИЙ МАГАЗИН {userId}</div>;
}