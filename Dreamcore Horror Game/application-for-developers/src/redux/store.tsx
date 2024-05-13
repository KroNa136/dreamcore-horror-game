import { configureStore } from "@reduxjs/toolkit";
import playerFormReducer from "./slices/player-form-slice";

export const store = configureStore({
  reducer: {
    playerForm: playerFormReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
