import { createSlice, ThunkDispatch } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit";
import type { RootState } from "../store";

interface GameModeFormState {
  id: string,
  assetName: string,
  maxPlayers: number | null,
  timeLimit: string | null,
  isActive: boolean,
}

const initialState: GameModeFormState = {
  id: "",
  assetName: "",
  maxPlayers: null,
  timeLimit: null,
  isActive: false
};

const gameModeFormSlice = createSlice({
  name: "gameModeForm",
  initialState: initialState,
  reducers: {
    setId: (state, action: PayloadAction<string>) => { state.id = action.payload },
    setAssetName: (state, action: PayloadAction<string>) => { state.assetName = action.payload },
    setMaxPlayers: (state, action: PayloadAction<number | null>) => { state.maxPlayers = action.payload },
    setTimeLimit: (state, action: PayloadAction<string | null>) => { state.timeLimit = action.payload },
    setIsActive: (state, action: PayloadAction<boolean>) => { state.isActive = action.payload },
  },
});

export const state = (state: RootState) => state.gameModeForm;
export const actions = gameModeFormSlice.actions;

export const resetState = (dispatch: ThunkDispatch<any, any, any>) => {
  dispatch(actions.setId(initialState.id));
  dispatch(actions.setAssetName(initialState.assetName));
  dispatch(actions.setMaxPlayers(initialState.maxPlayers));
  dispatch(actions.setTimeLimit(initialState.timeLimit));
  dispatch(actions.setIsActive(initialState.isActive));
};

export default gameModeFormSlice.reducer;
