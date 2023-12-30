import {Button, Stack, TextField} from "@mui/material";
import React, {useState} from "react";
import "./LoginPage.css";
import {Navigate, useNavigate} from "react-router-dom";
import {useStore} from "../../Stores";

const LoginPage = () => {
  const {authStore} = useStore();
  const [userId, setUserId] = useState("");
  const [token, setToken] = useState("");
  const navigate = useNavigate();
  const currentLoggedInUserId = authStore.userId;
  return (
    <div className="background">
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
          onClick={() => {
            if (!userId) {
              return;
            }
            authStore.setUserId(userId);
            navigate(`/user/${userId}`);
          }}
        >
          Login
        </Button>
      </Stack>
    </div>
  );
};

export default LoginPage;