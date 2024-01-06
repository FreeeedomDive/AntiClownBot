import {useParams} from "react-router-dom";
import React from "react";
import {useStore} from "../../../../Stores";

export default function ItemsTrade() {
  const {authStore} = useStore();
  const currentLoggedInUserId = authStore.loggedInUserId;
  const {userId} = useParams<"userId">();

  return <div>ОБМЕН ПРЕДМЕТАМИ ПОЛЬЗОВАТЕЛЯ {currentLoggedInUserId} С ПОЛЬЗОВАТЕЛЕМ {userId}</div>;
}