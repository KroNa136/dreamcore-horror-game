import { createSlice, ThunkDispatch } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit";
import type { RootState } from "../store";
import { RarityLevel } from "../../database";

interface ArtifactFormState {
  id: string,
  assetName: string,
  rarityLevelId: string,
  rarityLevels: RarityLevel[],
}

const initialState: ArtifactFormState = {
  id: "",
  assetName: "",
  rarityLevelId: "",
  rarityLevels: []
};

const artifactFormSlice = createSlice({
  name: "artifactForm",
  initialState: initialState,
  reducers: {
    setId: (state, action: PayloadAction<string>) => { state.id = action.payload },
    setAssetName: (state, action: PayloadAction<string>) => { state.assetName = action.payload },
    setRarityLevelId: (state, action: PayloadAction<string>) => { state.rarityLevelId = action.payload },
    setRarityLevels: (state, action: PayloadAction<RarityLevel[]>) => { state.rarityLevels = action.payload },
  },
});

export const state = (state: RootState) => state.artifactForm;
export const actions = artifactFormSlice.actions;

export const resetState = (dispatch: ThunkDispatch<any, any, any>) => {
  dispatch(actions.setId(initialState.id));
  dispatch(actions.setAssetName(initialState.assetName));
  dispatch(actions.setRarityLevelId(initialState.rarityLevelId));
  dispatch(actions.setRarityLevels(initialState.rarityLevels));
};

export default artifactFormSlice.reducer;
