import {useParams} from "react-router-dom";
import React from "react";
import {useStore} from "../../../../Stores";

export default function ItemsTrade() {
  const {authStore} = useStore();
  const currentLoggedInUserId = authStore.userId;
  const {userId} = useParams<"userId">();

  return <div>ОБМЕН ПРЕДМЕТАМИ ПОЛЬЗОВАТЕЛЯ {currentLoggedInUserId} С ПОЛЬЗОВАТЕЛЕМ {userId}</div>;
}