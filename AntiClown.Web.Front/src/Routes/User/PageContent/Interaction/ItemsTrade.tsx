import { useParams } from "react-router-dom";
import React from "react";
import { useStore } from "../../../../Stores";
import { Typography } from "@mui/material";

export default function ItemsTrade() {
  const { authStore } = useStore();
  const currentLoggedInUserId = authStore.loggedInUserId;
  const { userId } = useParams<"userId">();

  return (
    <Typography variant="h5">
      ОБМЕН ПРЕДМЕТАМИ ПОЛЬЗОВАТЕЛЯ {currentLoggedInUserId} С ПОЛЬЗОВАТЕЛЕМ{" "}
      {userId}
    </Typography>
  );
}
