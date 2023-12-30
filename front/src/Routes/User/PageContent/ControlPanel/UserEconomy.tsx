import { useParams } from "react-router-dom";
import React from "react";

export default function UserEconomy() {
  const { userId } = useParams<"userId">();

  return <div>МОЯ БУДУЩАЯ ЭКОНОМИКА {userId}</div>;
}