import { configureStore } from "@reduxjs/toolkit";
import abilityFormReducer from "./slices/ability-form-slice";
import acquiredAbilityFormReducer from "./slices/acquired-ability-form-slice";
import artifactFormReducer from "./slices/artifact-form-slice";
import collectedArtifactFormReducer from "./slices/collected-artifact-form-slice";
import creatureFormReducer from "./slices/creature-form-slice";
import developerFormReducer from "./slices/developer-form-slice";
import developerAccessLevelFormReducer from "./slices/developer-access-level-form-slice";
import gameModeFormReducer from "./slices/game-mode-form-slice";
import gameSessionFormReducer from "./slices/game-session-form-slice";
import playerFormReducer from "./slices/player-form-slice";
import playerSessionFormReducer from "./slices/player-session-form-slice";
import rarityLevelFormReducer from "./slices/rarity-level-form-slice";
import serverFormReducer from "./slices/server-form-slice";
import xpLevelFormReducer from "./slices/xp-level-form-slice";

export const store = configureStore({
  reducer: {
    abilityForm: abilityFormReducer,
    acquiredAbilityForm: acquiredAbilityFormReducer,
    artifactForm: artifactFormReducer,
    collectedArtifactForm: collectedArtifactFormReducer,
    creatureForm: creatureFormReducer,
    developerForm: developerFormReducer,
    developerAccessLevelForm: developerAccessLevelFormReducer,
    gameModeForm: gameModeFormReducer,
    gameSessionForm: gameSessionFormReducer,
    playerForm: playerFormReducer,
    playerSessionForm: playerSessionFormReducer,
    rarityLevelForm: rarityLevelFormReducer,
    serverForm: serverFormReducer,
    xpLevelForm: xpLevelFormReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
