import React from "react";
import "./app.css";
import SignIn from "./pages/sign-in";
import MainPage from "./pages/main-page";

import AllAbilities from "./pages/all-abilities";

import AllAcquiredAbilities from "./pages/all-acquired-abilities";

import AllArtifacts from "./pages/all-artifacts";

import AllCollectedArtifacts from "./pages/all-collected-artifacts";

import AllCreatures from "./pages/all-creatures";

import AllDevelopers from "./pages/all-developers";

import AllDeveloperAccessLevels from "./pages/all-developer-access-levels";

import AllGameModes from "./pages/all-game-modes";

import AllGameSessions from "./pages/all-game-sessions";

import AllPlayers from "./pages/all-players";
import SinglePlayer from "./pages/single-player";
import CreatePlayer from "./pages/create-player";
import EditPlayer from "./pages/edit-player";

import AllPlayerSessions from "./pages/all-player-sessions";

import AllRarityLevels from "./pages/all-rarity-levels";

import AllServers from "./pages/all-servers";

import AllXpLevels from "./pages/all-xp-levels";

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

        <Route path="/abilities" element={
          <IfSignedIn>
            <AllAbilities defaultShowBy={defaultShowBy} />
          </IfSignedIn>
        } />

        <Route path="/acquiredAbilities" element={
          <IfSignedIn>
            <AllAcquiredAbilities defaultShowBy={defaultShowBy} />
          </IfSignedIn>
        } />

        <Route path="/artifacts" element={
          <IfSignedIn>
            <AllArtifacts defaultShowBy={defaultShowBy} />
          </IfSignedIn>
        } />

        <Route path="/collectedArtifacts" element={
          <IfSignedIn>
            <AllCollectedArtifacts defaultShowBy={defaultShowBy} />
          </IfSignedIn>
        } />

        <Route path="/creatures" element={
          <IfSignedIn>
            <AllCreatures defaultShowBy={defaultShowBy} />
          </IfSignedIn>
        } />

        <Route path="/developers" element={
          <IfSignedIn>
            <AllDevelopers defaultShowBy={defaultShowBy} />
          </IfSignedIn>
        } />

        <Route path="/developerAccessLevels" element={
          <IfSignedIn>
            <AllDeveloperAccessLevels defaultShowBy={defaultShowBy} />
          </IfSignedIn>
        } />

        <Route path="/gameModes" element={
          <IfSignedIn>
            <AllGameModes defaultShowBy={defaultShowBy} />
          </IfSignedIn>
        } />

        <Route path="/gameSessions" element={
          <IfSignedIn>
            <AllGameSessions defaultShowBy={defaultShowBy} />
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

        <Route path="/playerSessions" element={
          <IfSignedIn>
            <AllPlayerSessions defaultShowBy={defaultShowBy} />
          </IfSignedIn>
        } />

        <Route path="/rarityLevels" element={
          <IfSignedIn>
            <AllRarityLevels defaultShowBy={defaultShowBy} />
          </IfSignedIn>
        } />

        <Route path="/servers" element={
          <IfSignedIn>
            <AllServers defaultShowBy={defaultShowBy} />
          </IfSignedIn>
        } />

        <Route path="/xpLevels" element={
          <IfSignedIn>
            <AllXpLevels defaultShowBy={defaultShowBy} />
          </IfSignedIn>
        } />

      </Routes>
    </BrowserRouter>
  );
}

export default App;
