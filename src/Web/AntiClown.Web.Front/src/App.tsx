import "./App.css";
import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import UserMainPage from "./Routes/User/MainPage/UserMainPage";
import { createTheme, CssBaseline, ThemeProvider } from "@mui/material";
import React, { useEffect } from "react";
import { useStore } from "./Stores";
import MobileMainPage from "./Mobile/MobileMainPage";
import LoginPage from "./Routes/Auth/LoginPage";
import AutoLoginPage from "./Routes/Auth/AutoLoginPage";
import { MAIN_COLOR } from "./Helpers/Colors";

const darkTheme = createTheme({
  palette: {
    mode: "dark",
    background: {
      default: MAIN_COLOR,
    },
  },
});

function App() {
  useEffect(() => {
    document.title = "Clown City";
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
          <Route path="/auth/auto" element={<AutoLoginPage />} />
          <Route path="/user/:userId/*" element={<UserMainPage />} />
          <Route
            path="/f1Predictions/standings"
            element={
              <Navigate to={`/user/${userId}/f1Predictions/standings`} />
            }
          />
          <Route
            path="/f1Predictions/rulebook"
            element={
              <Navigate to={`/user/${userId}/f1Predictions/rulebook`} />
            }
          />
          <Route path="/mobile" element={<MobileMainPage />} />
        </Routes>
      </BrowserRouter>
    </ThemeProvider>
  );
}

export default App;
