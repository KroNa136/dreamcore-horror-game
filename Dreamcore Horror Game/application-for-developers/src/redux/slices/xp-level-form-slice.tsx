import { createSlice, ThunkDispatch } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit";
import type { RootState } from "../store";

interface XpLevelFormState {
  id: string,
  number: number,
  requiredXp: number,
}

const initialState: XpLevelFormState = {
  id: "",
  number: 0,
  requiredXp: 0,
};

const xpLevelFormSlice = createSlice({
  name: "xpLevelForm",
  initialState: initialState,
  reducers: {
    setId: (state, action: PayloadAction<string>) => { state.id = action.payload },
    setNumber: (state, action: PayloadAction<number>) => { state.number = action.payload },
    setRequiredXp: (state, action: PayloadAction<number>) => { state.requiredXp = action.payload },
  },
});

export const state = (state: RootState) => state.xpLevelForm;
export const actions = xpLevelFormSlice.actions;

export const resetState = (dispatch: ThunkDispatch<any, any, any>) => {
  dispatch(actions.setId(initialState.id));
  dispatch(actions.setNumber(initialState.number));
  dispatch(actions.setRequiredXp(initialState.requiredXp));
};

export default xpLevelFormSlice.reducer;
