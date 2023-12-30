import {useStore} from "../../../Stores";
import {useParams} from "react-router-dom";
import React from "react";

export default function UserOverview(){
  const { authStore } = useStore();
  const currentLoggedInUserId = authStore.userId;
  const { userId } = useParams<"userId">();
  const isMyPage = currentLoggedInUserId === userId;

  return <div>СУКА, ЭТО {isMyPage ? "" : "НЕ "}МОЯ СТРАНИЦА</div>;
}