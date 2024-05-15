import { createSlice, ThunkDispatch } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit";
import type { RootState } from "../store";

interface RarityLevelFormState {
  id: string,
  assetName: string,
  probability: number,
}

const initialState: RarityLevelFormState = {
  id: "",
  assetName: "",
  probability: 0
};

const rarityLevelFormSlice = createSlice({
  name: "rarityLevelForm",
  initialState: initialState,
  reducers: {
    setId: (state, action: PayloadAction<string>) => { state.id = action.payload },
    setAssetName: (state, action: PayloadAction<string>) => { state.assetName = action.payload },
    setProbability: (state, action: PayloadAction<number>) => { state.probability = action.payload },
  },
});

export const state = (state: RootState) => state.rarityLevelForm;
export const actions = rarityLevelFormSlice.actions;

export const resetState = (dispatch: ThunkDispatch<any, any, any>) => {
  dispatch(actions.setId(initialState.id));
  dispatch(actions.setAssetName(initialState.assetName));
  dispatch(actions.setProbability(initialState.probability));
};

export default rarityLevelFormSlice.reducer;
