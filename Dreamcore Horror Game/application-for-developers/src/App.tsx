import React from "react";
import "./app.css";
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
import AllPlayerSessions from "./pages/all-player-sessions";
import AllRarityLevels from "./pages/all-rarity-levels";
import AllServers from "./pages/all-servers";
import AllXpLevels from "./pages/all-xp-levels";
import CreateAbility from "./pages/create-ability";
import CreateAcquiredAbility from "./pages/create-acquired-ability";
import CreateArtifact from "./pages/create-artifact";
import CreateCollectedArtifact from "./pages/create-collected-artifact";
import CreateCreature from "./pages/create-creature";
import CreateDeveloper from "./pages/create-developer";
import CreateDeveloperAccessLevel from "./pages/create-developer-access-level";
import CreateGameMode from "./pages/create-game-mode";
import CreateGameSession from "./pages/create-game-session";
import CreatePlayer from "./pages/create-player";
import CreatePlayerSession from "./pages/create-player-session";
import CreateRarityLevel from "./pages/create-rarity-level";
import CreateServer from "./pages/create-server";
import CreateXpLevel from "./pages/create-xp-level";
import EditAbility from "./pages/edit-ability";
import EditAcquiredAbility from "./pages/edit-acquired-ability";
import EditArtifact from "./pages/edit-artifact";
import EditCollectedArtifact from "./pages/edit-collected-artifact";
import EditCreature from "./pages/edit-creature";
import EditDeveloper from "./pages/edit-developer";
import EditDeveloperAccessLevel from "./pages/edit-developer-access-level";
import EditGameMode from "./pages/edit-game-mode";
import EditGameSession from "./pages/edit-game-session";
import EditPlayer from "./pages/edit-player";
import EditPlayerSession from "./pages/edit-player-session";
import EditRarityLevel from "./pages/edit-rarity-level";
import EditServer from "./pages/edit-server";
import EditXpLevel from "./pages/edit-xp-level";
import MainPage from "./pages/main-page";
import SignIn from "./pages/sign-in";
import SingleAbility from "./pages/single-ability";
import SingleAcquiredAbility from "./pages/single-acquired-ability";
import SingleArtifact from "./pages/single-artifact";
import SingleCollectedArtifact from "./pages/single-collected-artifact";
import SingleCreature from "./pages/single-creature";
import SingleDeveloper from "./pages/single-developer";
import SingleDeveloperAccessLevel from "./pages/single-developer-access-level";
import SingleGameMode from "./pages/single-game-mode";
import SingleGameSession from "./pages/single-game-session";
import SinglePlayer from "./pages/single-player";
import SinglePlayerSession from "./pages/single-player-session";
import SingleRarityLevel from "./pages/single-rarity-level";
import SingleServer from "./pages/single-server";
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
        <Route path="/createAbility" element={
          <IfSignedIn>
            <CreateAbility />
          </IfSignedIn>
        } />
        <Route path="/editAbility/:id" element={
          <IfSignedIn>
            <EditAbility />
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
        <Route path="/createAcquiredAbility" element={
          <IfSignedIn>
            <CreateAcquiredAbility />
          </IfSignedIn>
        } />
        <Route path="/editAcquiredAbility/:id" element={
          <IfSignedIn>
            <EditAcquiredAbility />
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
        <Route path="/createArtifact" element={
          <IfSignedIn>
            <CreateArtifact />
          </IfSignedIn>
        } />
        <Route path="/editArtifact/:id" element={
          <IfSignedIn>
            <EditArtifact />
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
        <Route path="/createCollectedArtifact" element={
          <IfSignedIn>
            <CreateCollectedArtifact />
          </IfSignedIn>
        } />
        <Route path="/editCollectedArtifact/:id" element={
          <IfSignedIn>
            <EditCollectedArtifact />
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
        <Route path="/createCreature" element={
          <IfSignedIn>
            <CreateCreature />
          </IfSignedIn>
        } />
        <Route path="/editCreature/:id" element={
          <IfSignedIn>
            <EditCreature />
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
        <Route path="/createDeveloper" element={
          <IfSignedIn>
            <CreateDeveloper />
          </IfSignedIn>
        } />
        <Route path="/editDeveloper/:id" element={
          <IfSignedIn>
            <EditDeveloper />
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
        <Route path="/createDeveloperAccessLevel" element={
          <IfSignedIn>
            <CreateDeveloperAccessLevel />
          </IfSignedIn>
        } />
        <Route path="/editDeveloperAccessLevel/:id" element={
          <IfSignedIn>
            <EditDeveloperAccessLevel />
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
        <Route path="/createGameMode" element={
          <IfSignedIn>
            <CreateGameMode />
          </IfSignedIn>
        } />
        <Route path="/editGameMode/:id" element={
          <IfSignedIn>
            <EditGameMode />
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
        <Route path="/createGameSession" element={
          <IfSignedIn>
            <CreateGameSession />
          </IfSignedIn>
        } />
        <Route path="/editGameSession/:id" element={
          <IfSignedIn>
            <EditGameSession />
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
        <Route path="/createPlayerSession" element={
          <IfSignedIn>
            <CreatePlayerSession />
          </IfSignedIn>
        } />
        <Route path="/editPlayerSession/:id" element={
          <IfSignedIn>
            <EditPlayerSession />
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
        <Route path="/createRarityLevel" element={
          <IfSignedIn>
            <CreateRarityLevel />
          </IfSignedIn>
        } />
        <Route path="/editRarityLevel/:id" element={
          <IfSignedIn>
            <EditRarityLevel />
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
        <Route path="/createServer" element={
          <IfSignedIn>
            <CreateServer />
          </IfSignedIn>
        } />
        <Route path="/editServer/:id" element={
          <IfSignedIn>
            <EditServer />
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
        <Route path="/createXpLevel" element={
          <IfSignedIn>
            <CreateXpLevel />
          </IfSignedIn>
        } />
        <Route path="/editXpLevel/:id" element={
          <IfSignedIn>
            <EditXpLevel />
          </IfSignedIn>
        } />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
