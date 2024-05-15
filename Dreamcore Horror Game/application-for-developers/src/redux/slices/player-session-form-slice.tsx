import { createSlice, ThunkDispatch } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit";
import type { RootState } from "../store";

interface PlayerSessionFormState {
  id: string,
  gameSessionId: string,
  playerId: string,
  startTimestamp: string,
  endTimestamp: string | null,
  isCompleted: boolean | null,
  isWon: boolean | null,
  timeAlive: string | null,
  playedAsCreature: boolean | null,
  usedCreatureId: string | null,
  selfReviveCount: number | null,
  allyReviveCount: number | null,
}

const initialState: PlayerSessionFormState = {
  id: "",
  gameSessionId: "",
  playerId: "",
  startTimestamp: "1970-01-01T00:00:00.000Z",
  endTimestamp: null,
  isCompleted: null,
  isWon: null,
  timeAlive: null,
  playedAsCreature: null,
  usedCreatureId: null,
  selfReviveCount: null,
  allyReviveCount: null,
};

const playerSessionFormSlice = createSlice({
  name: "playerSessionForm",
  initialState: initialState,
  reducers: {
    setId: (state, action: PayloadAction<string>) => { state.id = action.payload },
    setGameSessionId: (state, action: PayloadAction<string>) => { state.gameSessionId = action.payload },
    setPlayerId: (state, action: PayloadAction<string>) => { state.playerId = action.payload },
    setStartTimestamp: (state, action: PayloadAction<string>) => { state.startTimestamp = action.payload },
    setEndTimestamp: (state, action: PayloadAction<string | null>) => { state.endTimestamp = action.payload },
    setIsCompleted: (state, action: PayloadAction<boolean | null>) => { state.isCompleted = action.payload },
    setIsWon: (state, action: PayloadAction<boolean | null>) => { state.isWon = action.payload },
    setTimeAlive: (state, action: PayloadAction<string | null>) => { state.timeAlive = action.payload },
    setPlayedAsCreature: (state, action: PayloadAction<boolean | null>) => { state.playedAsCreature = action.payload },
    setUsedCreatureId: (state, action: PayloadAction<string | null>) => { state.usedCreatureId = action.payload },
    setSelfReviveCount: (state, action: PayloadAction<number | null>) => { state.selfReviveCount = action.payload },
    setAllyReviveCount: (state, action: PayloadAction<number | null>) => { state.allyReviveCount = action.payload },
  },
});

export const state = (state: RootState) => state.playerSessionForm;
export const actions = playerSessionFormSlice.actions;

export const resetState = (dispatch: ThunkDispatch<any, any, any>) => {
  dispatch(actions.setId(initialState.id));
  dispatch(actions.setGameSessionId(initialState.gameSessionId));
  dispatch(actions.setPlayerId(initialState.playerId));
  dispatch(actions.setStartTimestamp(initialState.startTimestamp));
  dispatch(actions.setEndTimestamp(initialState.endTimestamp));
  dispatch(actions.setIsCompleted(initialState.isCompleted));
  dispatch(actions.setIsWon(initialState.isWon));
  dispatch(actions.setTimeAlive(initialState.timeAlive));
  dispatch(actions.setPlayedAsCreature(initialState.playedAsCreature));
  dispatch(actions.setUsedCreatureId(initialState.usedCreatureId));
  dispatch(actions.setSelfReviveCount(initialState.selfReviveCount));
  dispatch(actions.setAllyReviveCount(initialState.allyReviveCount));
};

export default playerSessionFormSlice.reducer;
