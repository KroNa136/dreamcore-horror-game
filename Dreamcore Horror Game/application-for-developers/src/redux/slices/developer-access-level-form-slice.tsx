import { createSlice, ThunkDispatch } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit";
import type { RootState } from "../store";

interface DeveloperAccessLevelFormState {
  id: string,
  name: string,
}

const initialState: DeveloperAccessLevelFormState = {
  id: "",
  name: "",
};

const developerAccessLevelFormSlice = createSlice({
  name: "developerAccessLevelForm",
  initialState: initialState,
  reducers: {
    setId: (state, action: PayloadAction<string>) => { state.id = action.payload },
    setName: (state, action: PayloadAction<string>) => { state.name = action.payload },
  },
});

export const state = (state: RootState) => state.developerAccessLevelForm;
export const actions = developerAccessLevelFormSlice.actions;

export const resetState = (dispatch: ThunkDispatch<any, any, any>) => {
  dispatch(actions.setId(initialState.id));
  dispatch(actions.setName(initialState.name));
};

export default developerAccessLevelFormSlice.reducer;
