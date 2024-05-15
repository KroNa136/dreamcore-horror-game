import { createSlice, ThunkDispatch } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit";
import type { RootState } from "../store";
import { Artifact, Player } from "../../database";

interface CollectedArtifactFormState {
  id: string,
  playerId: string,
  artifactId: string,
  collectionTimestamp: string,
  players: Player[],
  artifacts: Artifact[],
}

const initialState: CollectedArtifactFormState = {
  id: "",
  playerId: "",
  artifactId: "",
  collectionTimestamp: "1970-01-01T00:00:00.000Z",
  players: [],
  artifacts: []
};

const collectedArtifactFormSlice = createSlice({
  name: "collectedArtifactForm",
  initialState: initialState,
  reducers: {
    setId: (state, action: PayloadAction<string>) => { state.id = action.payload },
    setPlayerId: (state, action: PayloadAction<string>) => { state.playerId = action.payload },
    setArtifactId: (state, action: PayloadAction<string>) => { state.artifactId = action.payload },
    setCollectionTimestamp: (state, action: PayloadAction<string>) => { state.collectionTimestamp = action.payload },
    setPlayers: (state, action: PayloadAction<Player[]>) => { state.players = action.payload },
    setArtifacts: (state, action: PayloadAction<Artifact[]>) => { state.artifacts = action.payload },
  },
});

export const state = (state: RootState) => state.collectedArtifactForm;
export const actions = collectedArtifactFormSlice.actions;

export const resetState = (dispatch: ThunkDispatch<any, any, any>) => {
  dispatch(actions.setId(initialState.id));
  dispatch(actions.setPlayerId(initialState.playerId));
  dispatch(actions.setArtifactId(initialState.artifactId));
  dispatch(actions.setCollectionTimestamp(initialState.collectionTimestamp));
  dispatch(actions.setPlayers(initialState.players));
  dispatch(actions.setArtifacts(initialState.artifacts));
};

export default collectedArtifactFormSlice.reducer;
