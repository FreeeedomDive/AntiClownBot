import { Box, CircularProgress, Typography } from "@mui/material";
import { useEffect } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { useStore } from "../../Stores";
import TokensApi from "../../Api/TokensApi";

export default function AutoLoginPage() {
  const [searchParams] = useSearchParams();
  const { authStore } = useStore();
  const navigate = useNavigate();

  useEffect(() => {
    document.title = "Авторизация - Clown City";
    const userId = searchParams.get("userId");
    const token = searchParams.get("token");

    if (!userId || !token) {
      navigate("/auth");
      return;
    }

    TokensApi.isTokenValid(userId, token).then((isValid) => {
      if (isValid) {
        authStore.logIn(userId, token);
        navigate(`/user/${userId}`);
      } else {
        navigate("/auth");
      }
    });
  }, []);

  return (
    <Box
      sx={{
        display: "flex",
        flexDirection: "column",
        justifyContent: "center",
        alignItems: "center",
        gap: 2,
        height: "100vh",
      }}
    >
      <CircularProgress />
      <Typography color="text.secondary">Выполняется вход...</Typography>
    </Box>
  );
}
