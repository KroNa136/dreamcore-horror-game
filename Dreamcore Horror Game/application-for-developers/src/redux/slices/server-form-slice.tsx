import { createSlice, ThunkDispatch } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit";
import type { RootState } from "../store";

interface ServerFormState {
  id: string,
  ipAddress: string,
  password: string,
  refreshToken: string | null,
  playerCapacity: number,
  isOnline: boolean,
}

const initialState: ServerFormState = {
  id: "",
  ipAddress: "",
  password: "",
  refreshToken: null,
  playerCapacity: 0,
  isOnline: false,
};

const serverFormSlice = createSlice({
  name: "serverForm",
  initialState: initialState,
  reducers: {
    setId: (state, action: PayloadAction<string>) => { state.id = action.payload },
    setIpAddress: (state, action: PayloadAction<string>) => { state.ipAddress = action.payload },
    setPassword: (state, action: PayloadAction<string>) => { state.password = action.payload },
    setRefreshToken: (state, action: PayloadAction<string | null>) => { state.refreshToken = action.payload },
    setPlayerCapacity: (state, action: PayloadAction<number>) => { state.playerCapacity = action.payload },
    setIsOnline: (state, action: PayloadAction<boolean>) => { state.isOnline = action.payload },
  },
});

export const state = (state: RootState) => state.serverForm;
export const actions = serverFormSlice.actions;

export const resetState = (dispatch: ThunkDispatch<any, any, any>) => {
  dispatch(actions.setId(initialState.id));
  dispatch(actions.setIpAddress(initialState.ipAddress));
  dispatch(actions.setPassword(initialState.password));
  dispatch(actions.setRefreshToken(initialState.refreshToken));
  dispatch(actions.setPlayerCapacity(initialState.playerCapacity));
  dispatch(actions.setIsOnline(initialState.isOnline));
};

export default serverFormSlice.reducer;
