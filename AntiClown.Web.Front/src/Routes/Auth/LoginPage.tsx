import {Alert, Button, CircularProgress, Snackbar, Stack, TextField} from "@mui/material";
import React, {useState} from "react";
import "./LoginPage.css";
import {Navigate, useNavigate} from "react-router-dom";
import {useStore} from "../../Stores";
import UsersApi from "../../Api/UsersApi";
import TokensApi from "../../Api/TokensApi";

const LoginPage = () => {
  const {authStore} = useStore();
  const [userId, setUserId] = useState("");
  const [token, setToken] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const currentLoggedInUserId = authStore.userId;
  return (
    <div className="background">
      <Snackbar open={!!error} autoHideDuration={5000} onClose={() => setError("")}>
        <Alert severity="error">
          {error}
        </Alert>
      </Snackbar>
      {currentLoggedInUserId && <Navigate to={`/user/${currentLoggedInUserId}`}/>}
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
        <Button
          fullWidth
          className="loginButton"
          color="primary"
          variant="contained"
          onClick={async () => {
            if (!userId || !token) {
              return;
            }
            setLoading(true);
            const user = await UsersApi.get(userId);
            setLoading(false);
            if (!user) {
              setError(`Пользователь ${userId} не найден!`)
              return;
            }
            const isValidToken = await TokensApi.isTokenValid(userId, token);
            if (!isValidToken){
              setError(`Невалидный токен`)
              return;
            }
            authStore.setUserId(userId);
            navigate(`/user/${userId}`);
          }}
        >
          {loading ? <CircularProgress color="inherit" size={25}/> : <>Login</>}
        </Button>
      </Stack>
    </div>
  );
};

export default LoginPage;