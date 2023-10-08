import React from 'react';
import './App.css';
import {BrowserRouter, Route, Routes} from "react-router-dom";
import UserMainPage from "./Routes/User/UserMainPage";

function App() {
  return (
      <BrowserRouter basename="/">
        <Routes>
            <Route path="/" element={<div>Login page</div>}/>
            <Route path="/:userId" element={<UserMainPage/>}/>
        </Routes>
      </BrowserRouter>
  );
}

export default App;
