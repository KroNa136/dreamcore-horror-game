import { createSlice, ThunkDispatch } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit";
import type { RootState } from "../store";
import { DeveloperAccessLevel } from "../../database";

interface DeveloperFormState {
  id: string,
  login: string,
  password: string,
  refreshToken: string | null,
  developerAccessLevelId: string,
  developerAccessLevels: DeveloperAccessLevel[],
}

const initialState: DeveloperFormState = {
  id: "",
  login: "",
  password: "",
  refreshToken: null,
  developerAccessLevelId: "",
  developerAccessLevels: [],
};

const developerFormSlice = createSlice({
  name: "developerForm",
  initialState: initialState,
  reducers: {
    setId: (state, action: PayloadAction<string>) => { state.id = action.payload },
    setLogin: (state, action: PayloadAction<string>) => { state.login = action.payload },
    setPassword: (state, action: PayloadAction<string>) => { state.password = action.payload },
    setRefreshToken: (state, action: PayloadAction<string | null>) => { state.refreshToken = action.payload },
    setDeveloperAccessLevelId: (state, action: PayloadAction<string>) => { state.developerAccessLevelId = action.payload },
    setDeveloperAccessLevels: (state, action: PayloadAction<DeveloperAccessLevel[]>) => { state.developerAccessLevels = action.payload },
  },
});

export const state = (state: RootState) => state.developerForm;
export const actions = developerFormSlice.actions;

export const resetState = (dispatch: ThunkDispatch<any, any, any>) => {
  dispatch(actions.setId(initialState.id));
  dispatch(actions.setLogin(initialState.login));
  dispatch(actions.setPassword(initialState.password));
  dispatch(actions.setRefreshToken(initialState.refreshToken));
  dispatch(actions.setDeveloperAccessLevelId(initialState.developerAccessLevelId));
  dispatch(actions.setDeveloperAccessLevels(initialState.developerAccessLevels));
};

export default developerFormSlice.reducer;
