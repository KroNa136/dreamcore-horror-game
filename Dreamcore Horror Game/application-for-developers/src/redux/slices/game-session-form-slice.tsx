import { createSlice, ThunkDispatch } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit";
import type { RootState } from "../store";
import { GameMode, Server } from "../../database";

interface GameSessionFormState {
  id: string,
  serverId: string | null,
  gameModeId: string,
  startTimestamp: string,
  endTimestamp: string | null,
  servers: Server[],
  gameModes: GameMode[],
}

const initialState: GameSessionFormState = {
  id: "",
  serverId: null,
  gameModeId: "",
  startTimestamp: "1970-01-01T00:00:00.000Z",
  endTimestamp: null,
  servers: [],
  gameModes: [],
};

const gameSessionFormSlice = createSlice({
  name: "gameSessionForm",
  initialState: initialState,
  reducers: {
    setId: (state, action: PayloadAction<string>) => { state.id = action.payload },
    setServerId: (state, action: PayloadAction<string | null>) => { state.serverId = action.payload },
    setGameModeId: (state, action: PayloadAction<string>) => { state.gameModeId = action.payload },
    setStartTimestamp: (state, action: PayloadAction<string>) => { state.startTimestamp = action.payload },
    setEndTimestamp: (state, action: PayloadAction<string | null>) => { state.endTimestamp = action.payload },
    setServers: (state, action: PayloadAction<Server[]>) => { state.servers = action.payload },
    setGameModes: (state, action: PayloadAction<GameMode[]>) => { state.gameModes = action.payload },
  },
});

export const state = (state: RootState) => state.gameSessionForm;
export const actions = gameSessionFormSlice.actions;

export const resetState = (dispatch: ThunkDispatch<any, any, any>) => {
  dispatch(actions.setId(initialState.id));
  dispatch(actions.setServerId(initialState.serverId));
  dispatch(actions.setGameModeId(initialState.gameModeId));
  dispatch(actions.setStartTimestamp(initialState.startTimestamp));
  dispatch(actions.setEndTimestamp(initialState.endTimestamp));
  dispatch(actions.setServers(initialState.servers));
  dispatch(actions.setGameModes(initialState.gameModes));
};

export default gameSessionFormSlice.reducer;
