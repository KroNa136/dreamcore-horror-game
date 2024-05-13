import React from "react";
import "./app.css";
import SignIn from "./pages/sign-in";
import MainPage from "./pages/main-page";

import AllPlayers from "./pages/all-players";
import SinglePlayer from "./pages/single-player";
import CreatePlayer from "./pages/create-player";
import EditPlayer from "./pages/edit-player";

import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import { isSignedIn } from "./auth-manager";

const IfSignedIn = ({ children }: { children: JSX.Element }) => {
  return isSignedIn() ? children : <Navigate to="/signIn"/>
}

const defaultShowBy = 10; // for "All Something" pages

function App() {
  
  return (
    <BrowserRouter>
      <Routes>
        <Route path="*" element={
          <Navigate to="/" />
        } />

        <Route path="signIn" element={
          <SignIn />
        } />
        <Route path="/" element={
          <IfSignedIn>
            <MainPage />
          </IfSignedIn>
        } />



        <Route path="/players" element={
          <IfSignedIn>
            <AllPlayers defaultShowBy={defaultShowBy} />
          </IfSignedIn>
        } />
        <Route path="/player/:id" element={
          <IfSignedIn>
            <SinglePlayer />
          </IfSignedIn>
        } />
        <Route path="/createPlayer" element={
          <IfSignedIn>
            <CreatePlayer />
          </IfSignedIn>
        } />
        <Route path="/editPlayer/:id" element={
          <IfSignedIn>
            <EditPlayer />
          </IfSignedIn>
        } />


      </Routes>
    </BrowserRouter>
  );
}

export default App;
