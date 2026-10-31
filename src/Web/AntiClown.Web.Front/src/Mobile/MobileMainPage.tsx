import React, { useEffect, useState } from "react";
import { Navigate } from "react-router-dom";
import { Box, Typography } from "@mui/material";
import UsersApi from "../Api/UsersApi";
import { useStore } from "../Stores";
import { Loader } from "../Components/Loader/Loader";

export default function MobileMainPage() {
  const { authStore } = useStore();
  const [userId, setUserId] = useState<string>();
  const [error, setError] = useState<string>();
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    async function load() {
      const webApp = window.Telegram?.WebApp;
      if (!webApp) {
        setError("Эта страница доступна только внутри Telegram");
        return;
      }

      webApp.ready();
      const telegramUser = webApp.initDataUnsafe.user;
      if (!telegramUser) {
        setError("Не удалось получить пользователя Telegram");
        return;
      }

      authStore.logInViaTelegram(telegramUser.id);
      const apiUser = await UsersApi.find({ telegramId: telegramUser.id });
      if (!apiUser) {
        setError("Telegram-аккаунт не привязан к пользователю Clown City");
        return;
      }

      authStore.logInViaTelegram(telegramUser.id, apiUser.id);
      setUserId(apiUser.id);
    }

    load()
      .catch((loadError) => {
        console.error(loadError);
        setError("Не удалось выполнить вход через Telegram");
      })
      .finally(() => setIsLoading(false));
  }, [authStore]);

  if (userId) {
    return <Navigate to={`/user/${userId}`} replace />;
  }

  return (
    <Box
      sx={{
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        minHeight: "100dvh",
        p: 3,
      }}
    >
      {isLoading ? (
        <Loader />
      ) : (
        <Typography align="center" color="text.secondary">
          {error ?? "Не удалось выполнить вход через Telegram"}
        </Typography>
      )}
    </Box>
  );
}
