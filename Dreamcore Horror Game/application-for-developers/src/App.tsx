import React from "react";
import "./app.css";
import SignIn from "./pages/sign-in";
import MainPage from "./pages/main-page";

import AllAbilities from "./pages/all-abilities";
import SingleAbility from "./pages/single-ability";

import AllAcquiredAbilities from "./pages/all-acquired-abilities";
import SingleAcquiredAbility from "./pages/single-acquired-ability";

import AllArtifacts from "./pages/all-artifacts";
import SingleArtifact from "./pages/single-artifact";

import AllCollectedArtifacts from "./pages/all-collected-artifacts";
import SingleCollectedArtifact from "./pages/single-collected-artifact";

import AllCreatures from "./pages/all-creatures";
import SingleCreature from "./pages/single-creature";

import AllDevelopers from "./pages/all-developers";
import SingleDeveloper from "./pages/single-developer";

import AllDeveloperAccessLevels from "./pages/all-developer-access-levels";
import SingleDeveloperAccessLevel from "./pages/single-developer-access-level";

import AllGameModes from "./pages/all-game-modes";
import SingleGameMode from "./pages/single-game-mode";

import AllGameSessions from "./pages/all-game-sessions";
import SingleGameSession from "./pages/single-game-session";

import AllPlayers from "./pages/all-players";
import SinglePlayer from "./pages/single-player";
import CreatePlayer from "./pages/create-player";
import EditPlayer from "./pages/edit-player";

import AllPlayerSessions from "./pages/all-player-sessions";
import SinglePlayerSession from "./pages/single-player-session";

import AllRarityLevels from "./pages/all-rarity-levels";
import SingleRarityLevel from "./pages/single-rarity-level";

import AllServers from "./pages/all-servers";
import SingleServer from "./pages/single-server";

import AllXpLevels from "./pages/all-xp-levels";
import SingleXpLevel from "./pages/single-xp-level";

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
        <Route path="/ability/:id" element={
          <IfSignedIn>
            <SingleAbility />
          </IfSignedIn>
        } />

        <Route path="/acquiredAbilities" element={
          <IfSignedIn>
            <AllAcquiredAbilities defaultShowBy={defaultShowBy} />
          </IfSignedIn>
        } />
        <Route path="/acquiredAbility/:id" element={
          <IfSignedIn>
            <SingleAcquiredAbility />
          </IfSignedIn>
        } />

        <Route path="/artifacts" element={
          <IfSignedIn>
            <AllArtifacts defaultShowBy={defaultShowBy} />
          </IfSignedIn>
        } />
        <Route path="/artifact/:id" element={
          <IfSignedIn>
            <SingleArtifact />
          </IfSignedIn>
        } />

        <Route path="/collectedArtifacts" element={
          <IfSignedIn>
            <AllCollectedArtifacts defaultShowBy={defaultShowBy} />
          </IfSignedIn>
        } />
        <Route path="/collectedArtifact/:id" element={
          <IfSignedIn>
            <SingleCollectedArtifact />
          </IfSignedIn>
        } />

        <Route path="/creatures" element={
          <IfSignedIn>
            <AllCreatures defaultShowBy={defaultShowBy} />
          </IfSignedIn>
        } />
        <Route path="/creature/:id" element={
          <IfSignedIn>
            <SingleCreature />
          </IfSignedIn>
        } />

        <Route path="/developers" element={
          <IfSignedIn>
            <AllDevelopers defaultShowBy={defaultShowBy} />
          </IfSignedIn>
        } />
        <Route path="/developer/:id" element={
          <IfSignedIn>
            <SingleDeveloper />
          </IfSignedIn>
        } />

        <Route path="/developerAccessLevels" element={
          <IfSignedIn>
            <AllDeveloperAccessLevels defaultShowBy={defaultShowBy} />
          </IfSignedIn>
        } />
        <Route path="/developerAccessLevel/:id" element={
          <IfSignedIn>
            <SingleDeveloperAccessLevel />
          </IfSignedIn>
        } />

        <Route path="/gameModes" element={
          <IfSignedIn>
            <AllGameModes defaultShowBy={defaultShowBy} />
          </IfSignedIn>
        } />
        <Route path="/gameMode/:id" element={
          <IfSignedIn>
            <SingleGameMode />
          </IfSignedIn>
        } />

        <Route path="/gameSessions" element={
          <IfSignedIn>
            <AllGameSessions defaultShowBy={defaultShowBy} />
          </IfSignedIn>
        } />
        <Route path="/gameSession/:id" element={
          <IfSignedIn>
            <SingleGameSession />
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
        <Route path="/playerSession/:id" element={
          <IfSignedIn>
            <SinglePlayerSession />
          </IfSignedIn>
        } />

        <Route path="/rarityLevels" element={
          <IfSignedIn>
            <AllRarityLevels defaultShowBy={defaultShowBy} />
          </IfSignedIn>
        } />
        <Route path="/rarityLevel/:id" element={
          <IfSignedIn>
            <SingleRarityLevel />
          </IfSignedIn>
        } />

        <Route path="/servers" element={
          <IfSignedIn>
            <AllServers defaultShowBy={defaultShowBy} />
          </IfSignedIn>
        } />
        <Route path="/server/:id" element={
          <IfSignedIn>
            <SingleServer />
          </IfSignedIn>
        } />

        <Route path="/xpLevels" element={
          <IfSignedIn>
            <AllXpLevels defaultShowBy={defaultShowBy} />
          </IfSignedIn>
        } />
        <Route path="/xpLevel/:id" element={
          <IfSignedIn>
            <SingleXpLevel />
          </IfSignedIn>
        } />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
