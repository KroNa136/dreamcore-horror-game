import { createSlice, ThunkDispatch } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit";
import type { RootState } from "../store";
import { XpLevel } from "../../database";

interface CreatureFormState {
  id: string,
  assetName: string,
  requiredXpLevelId: string,
  health: number,
  movementSpeed: number,
  xpLevels: XpLevel[]
}

const initialState: CreatureFormState = {
  id: "",
  assetName: "",
  requiredXpLevelId: "",
  health: 0,
  movementSpeed: 0,
  xpLevels: []
};

const creatureFormSlice = createSlice({
  name: "creatureForm",
  initialState: initialState,
  reducers: {
    setId: (state, action: PayloadAction<string>) => { state.id = action.payload },
    setAssetName: (state, action: PayloadAction<string>) => { state.assetName = action.payload },
    setRequiredXpLevelId: (state, action: PayloadAction<string>) => { state.requiredXpLevelId = action.payload },
    setHealth: (state, action: PayloadAction<number>) => { state.health = action.payload },
    setMovementSpeed: (state, action: PayloadAction<number>) => { state.movementSpeed = action.payload },
    setXpLevels: (state, action: PayloadAction<XpLevel[]>) => { state.xpLevels = action.payload },
  },
});

export const state = (state: RootState) => state.creatureForm;
export const actions = creatureFormSlice.actions;

export const resetState = (dispatch: ThunkDispatch<any, any, any>) => {
  dispatch(actions.setId(initialState.id));
  dispatch(actions.setAssetName(initialState.assetName));
  dispatch(actions.setRequiredXpLevelId(initialState.requiredXpLevelId));
  dispatch(actions.setHealth(initialState.health));
  dispatch(actions.setMovementSpeed(initialState.movementSpeed));
  dispatch(actions.setXpLevels(initialState.xpLevels));
};

export default creatureFormSlice.reducer;
