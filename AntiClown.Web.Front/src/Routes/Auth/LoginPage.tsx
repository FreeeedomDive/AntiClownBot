import { Alert, Snackbar, Stack, TextField } from "@mui/material";
import React, { useCallback, useState } from "react";
import "./LoginPage.css";
import { Navigate, useNavigate } from "react-router-dom";
import { useStore } from "../../Stores";
import TokensApi from "../../Api/TokensApi";
import { LoadingButton } from "@mui/lab";
import { Login } from "@mui/icons-material";

export default function LoginPage() {
  const { authStore } = useStore();
  const [userId, setUserId] = useState("");
  const [token, setToken] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const currentLoggedInUserId = authStore.loggedInUserId;
  const hasToken = !!authStore.userToken;

  const login = useCallback(async () => {
    if (!userId || !token) {
      return;
    }
    setLoading(true);
    const isValidToken = await TokensApi.isTokenValid(userId, token);
    if (!isValidToken) {
      setError(`Неправильный UserId или токен`);
      return;
    }
    authStore.logIn(userId, token);
    setLoading(false);
    navigate(`/user/${userId}`);
  }, [userId, token, authStore, navigate]);

  return (
    <div className="background">
      <Snackbar
        open={!!error}
        autoHideDuration={5000}
        onClose={() => setError("")}
      >
        <Alert severity="error">{error}</Alert>
      </Snackbar>
      {currentLoggedInUserId && hasToken && (
        <Navigate to={`/user/${currentLoggedInUserId}`} />
      )}
      <Stack alignItems="center" className="auth" spacing="8px">
        <TextField
          className="input"
          fullWidth
          variant="outlined"
          label="UserId"
          value={userId}
          onChange={(x) => setUserId(x.target.value)}
        />
        <TextField
          className="input"
          fullWidth
          variant="outlined"
          label="Token"
          type="password"
          value={token}
          onChange={(x) => setToken(x.target.value)}
        />
        <LoadingButton
          fullWidth
          loading={loading}
          color="primary"
          size="large"
          variant="contained"
          startIcon={<Login />}
          onClick={login}
        >
          Login
        </LoadingButton>
      </Stack>
    </div>
  );
}
