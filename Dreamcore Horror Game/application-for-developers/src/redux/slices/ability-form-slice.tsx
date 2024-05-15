import { createSlice, ThunkDispatch } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit";
import type { RootState } from "../store";

interface AbilityFormState {
  id: string,
  assetName: string,
}

const initialState: AbilityFormState = {
  id: "",
  assetName: "",
};

const abilityFormSlice = createSlice({
  name: "abilityForm",
  initialState: initialState,
  reducers: {
    setId: (state, action: PayloadAction<string>) => { state.id = action.payload },
    setAssetName: (state, action: PayloadAction<string>) => { state.assetName = action.payload },
  },
});

export const state = (state: RootState) => state.abilityForm;
export const actions = abilityFormSlice.actions;

export const resetState = (dispatch: ThunkDispatch<any, any, any>) => {
  dispatch(actions.setId(initialState.id));
  dispatch(actions.setAssetName(initialState.assetName));
};

export default abilityFormSlice.reducer;
