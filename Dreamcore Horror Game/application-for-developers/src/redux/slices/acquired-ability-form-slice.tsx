import { createSlice, ThunkDispatch } from "@reduxjs/toolkit";
import type { PayloadAction } from "@reduxjs/toolkit";
import type { RootState } from "../store";
import { Ability, Player } from "../../database";

interface AcquiredAbilityFormState {
  id: string,
  playerId: string,
  abilityId: string,
  acquirementTimestamp: string,
  players: Player[],
  abilities: Ability[],
}

const initialState: AcquiredAbilityFormState = {
  id: "",
  playerId: "",
  abilityId: "",
  acquirementTimestamp: "1970-01-01T00:00:00.000Z",
  players: [],
  abilities: []
};

const acquiredAbilityFormSlice = createSlice({
  name: "acquiredAbilityForm",
  initialState: initialState,
  reducers: {
    setId: (state, action: PayloadAction<string>) => { state.id = action.payload },
    setPlayerId: (state, action: PayloadAction<string>) => { state.playerId = action.payload },
    setAbilityId: (state, action: PayloadAction<string>) => { state.abilityId = action.payload },
    setAcquirementTimestamp: (state, action: PayloadAction<string>) => { state.acquirementTimestamp = action.payload },
    setPlayers: (state, action: PayloadAction<Player[]>) => { state.players = action.payload },
    setAbilities: (state, action: PayloadAction<Ability[]>) => { state.abilities = action.payload },
  },
});

export const state = (state: RootState) => state.acquiredAbilityForm;
export const actions = acquiredAbilityFormSlice.actions;

export const resetState = (dispatch: ThunkDispatch<any, any, any>) => {
  dispatch(actions.setId(initialState.id));
  dispatch(actions.setPlayerId(initialState.playerId));
  dispatch(actions.setAbilityId(initialState.abilityId));
  dispatch(actions.setAcquirementTimestamp(initialState.acquirementTimestamp));
  dispatch(actions.setPlayers(initialState.players));
  dispatch(actions.setAbilities(initialState.abilities));
};

export default acquiredAbilityFormSlice.reducer;
