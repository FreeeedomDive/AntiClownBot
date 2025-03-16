import "./App.css";
import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import UserMainPage from "./Routes/User/MainPage/UserMainPage";
import LoginPage from "./Routes/Auth/LoginPage";
import { createTheme, CssBaseline, ThemeProvider } from "@mui/material";
import React, { useEffect } from "react";
import { useStore } from "./Stores";

const darkTheme = createTheme({
  palette: {
    mode: "dark",
    background: {
      default: "#000019",
    },
  },
});

function App() {
  useEffect(() => {
    document.title = "Clown City Web";
  }, []);
  const { authStore } = useStore();
  const userId = authStore.loggedInUserId;

  return (
    <ThemeProvider theme={darkTheme}>
      <CssBaseline />
      <BrowserRouter basename="/">
        <Routes>
          <Route path="/" element={<Navigate to={"/auth"} />} />
          <Route path="/auth" element={<LoginPage />} />
          <Route path="/user/:userId/*" element={<UserMainPage />} />
          <Route path="/f1Predictions" element={<Navigate to={`/user/${userId}/f1Predictions/standings`} />} />
        </Routes>
      </BrowserRouter>
    </ThemeProvider>
  );
}

export default App;
