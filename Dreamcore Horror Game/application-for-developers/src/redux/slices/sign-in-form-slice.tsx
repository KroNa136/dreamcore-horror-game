import { createSlice, ThunkDispatch } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit";
import type { RootState } from "../store";

interface SignInFormState {
  login: string,
  password: string,
}

const initialState: SignInFormState = {
  login: "",
  password: "",
};

const signInFormSlice = createSlice({
  name: "signInForm",
  initialState: initialState,
  reducers: {
    setLogin: (state, action: PayloadAction<string>) => { state.login = action.payload },
    setPassword: (state, action: PayloadAction<string>) => { state.password = action.payload },
  },
});

export const state = (state: RootState) => state.signInForm;
export const actions = signInFormSlice.actions;

export const resetState = (dispatch: ThunkDispatch<any, any, any>) => {
  dispatch(actions.setLogin(initialState.login));
  dispatch(actions.setPassword(initialState.password));
};

export default signInFormSlice.reducer;
