import "./App.css";
import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import UserMainPage from "./Routes/User/MainPage/UserMainPage";
import LoginPage from "./Routes/Auth/LoginPage";
import { createTheme, CssBaseline, ThemeProvider } from "@mui/material";
import React, {useEffect} from "react";

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
    document.title = 'Clown City Web';
  }, []);
  return (
    <ThemeProvider theme={darkTheme}>
      <CssBaseline />
      <BrowserRouter basename="/">
        <Routes>
          <Route path="/" element={<Navigate to={"/auth"} />} />
          <Route path="/auth" element={<LoginPage />} />
          <Route path="/user/:userId/*" element={<UserMainPage />} />
        </Routes>
      </BrowserRouter>
    </ThemeProvider>
  );
}

export default App;
