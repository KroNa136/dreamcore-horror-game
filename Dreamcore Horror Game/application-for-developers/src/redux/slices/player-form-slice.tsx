import { createSlice, ThunkDispatch } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit";
import type { RootState } from "../store";

interface PlayerFormState {
  id: string,
  username: string,
  email: string,
  password: string,
  refreshToken: string | null,
  registrationTimestamp: string,
  collectOptionalData: boolean,
  xpLevelId: string,
  xp: number,
  abilityPoints: number,
  spiritEnergyPoints: number
}

const initialState: PlayerFormState = {
  id: "",
  username: "",
  email: "",
  password: "",
  refreshToken: null,
  registrationTimestamp: "1970-01-01T00:00:00.000Z",
  collectOptionalData: false,
  xpLevelId: "",
  xp: 0,
  abilityPoints: 0,
  spiritEnergyPoints: 0
};

const playerFormSlice = createSlice({
  name: "playerForm",
  initialState: initialState,
  reducers: {
    setId: (state, action: PayloadAction<string>) => { state.id = action.payload },
    setUsername: (state, action: PayloadAction<string>) => { state.username = action.payload },
    setEmail: (state, action: PayloadAction<string>) => { state.email = action.payload },
    setPassword: (state, action: PayloadAction<string>) => { state.password = action.payload },
    setRefreshToken: (state, action: PayloadAction<string | null>) => { state.refreshToken = action.payload },
    setRegistrationTimestamp: (state, action: PayloadAction<string>) => { state.registrationTimestamp = action.payload },
    setCollectOptionalData: (state, action: PayloadAction<boolean>) => { state.collectOptionalData = action.payload },
    setXpLevelId: (state, action: PayloadAction<string>) => { state.xpLevelId = action.payload },
    setXp: (state, action: PayloadAction<number>) => { state.xp = action.payload },
    setAbilityPoints: (state, action: PayloadAction<number>) => { state.abilityPoints = action.payload },
    setSpiritEnergyPoints: (state, action: PayloadAction<number>) => { state.spiritEnergyPoints = action.payload },
  },
});

export const state = (state: RootState) => state.playerForm;
export const actions = playerFormSlice.actions;

export const resetState = (dispatch: ThunkDispatch<any, any, any>) => {
  dispatch(actions.setId(initialState.id));
  dispatch(actions.setUsername(initialState.username));
  dispatch(actions.setEmail(initialState.email));
  dispatch(actions.setPassword(initialState.password));
  dispatch(actions.setRefreshToken(initialState.refreshToken));
  dispatch(actions.setRegistrationTimestamp(initialState.registrationTimestamp));
  dispatch(actions.setCollectOptionalData(initialState.collectOptionalData));
  dispatch(actions.setXpLevelId(initialState.xpLevelId));
  dispatch(actions.setXp(initialState.xp));
  dispatch(actions.setAbilityPoints(initialState.abilityPoints));
  dispatch(actions.setSpiritEnergyPoints(initialState.spiritEnergyPoints));
}

export default playerFormSlice.reducer;
