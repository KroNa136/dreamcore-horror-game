import * as React from "react";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import { defaultTheme } from "../themes";
import { editAcquiredAbility, getAbilities, getAcquiredAbility, getPlayers } from "../requests";
import { useNavigate, useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { FormControl, InputLabel, MenuItem, Select } from "@mui/material";
import { displayName, AcquiredAbility, Player, Ability } from "../database";
import { DateTimePicker, LocalizationProvider } from "@mui/x-date-pickers";
import dayjs from "dayjs";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { useAppDispatch, useAppSelector } from "../redux/hooks";
import { actions, resetState } from "../redux/slices/acquired-ability-form-slice";
import Footer from "../components/footer";

export default function EditAcquiredAbility() {
  const navigate = useNavigate();
  const params = useParams();

  const dispatch = useAppDispatch();
  const state = useAppSelector(state => state.acquiredAbilityForm);

  const [players, setPlayers] = useState<Player[]>([]);
  const [abilities, setAbilities] = useState<Ability[]>([]);

  useEffect(() => {
    resetState(dispatch);
    getAcquiredAbility(params.id)
      .then(acquiredAbility => {
        dispatch(actions.setId(acquiredAbility.id));
        dispatch(actions.setPlayerId(acquiredAbility.playerId));
        dispatch(actions.setAbilityId(acquiredAbility.abilityId));
        dispatch(actions.setAcquirementTimestamp(acquiredAbility.acquirementTimestamp));
      });
    getPlayers()
      .then(players => setPlayers(players.items));
    getAbilities()
      .then(abilities => setAbilities(abilities.items));
  }, []);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const newAcquiredAbility: AcquiredAbility = new AcquiredAbility();

    newAcquiredAbility.id = state.id;
    newAcquiredAbility.playerId = state.playerId;
    newAcquiredAbility.abilityId = state.abilityId;
    newAcquiredAbility.acquirementTimestamp = state.acquirementTimestamp;

    const editedAcquiredAbility: AcquiredAbility | undefined = await editAcquiredAbility(newAcquiredAbility);

    if (editedAcquiredAbility) {
      navigate(`/acquiredAbility/${editedAcquiredAbility.id}`);
    }
  };

  return (
    <ThemeProvider theme={defaultTheme}>
      <Container component="main" maxWidth="xs">
        <CssBaseline />
        <Box
          sx={{
            marginTop: 10,
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
          }}
        >
          <Typography component="h4" variant="h4">
            Редактирование приобретённой способности
          </Typography>
          <Box component="form" onSubmit={handleSubmit} noValidate sx={{ mt: 1 }}>
            <FormControl sx={{ my: 1, minWidth: 120 }} fullWidth>
              <InputLabel id="playerLabel">Игрок</InputLabel>
              <Select
                labelId="playerLabel"
                id="player"
                label="Игрок"
                name="player"
                value={state.playerId ?? ""}
                onChange={e => dispatch(actions.setPlayerId(e.target.value))}
              >
                <MenuItem value="">
                  <em>Не выбрано</em>
                </MenuItem>
                {players.map(player => (
                  <MenuItem key={player.id} value={player.id}>{displayName(player)}</MenuItem>
                ))}
              </Select>
            </FormControl>
            <FormControl sx={{ my: 1, minWidth: 120 }} fullWidth>
              <InputLabel id="abilityLabel">Способность</InputLabel>
              <Select
                labelId="abilityLabel"
                id="ability"
                label="Способность"
                name="ability"
                value={state.abilityId ?? ""}
                onChange={e => dispatch(actions.setAbilityId(e.target.value))}
              >
                <MenuItem value="">
                  <em>Не выбрано</em>
                </MenuItem>
                {abilities.map(ability => (
                  <MenuItem key={ability.id} value={ability.id}>{displayName(ability)}</MenuItem>
                ))}
              </Select>
            </FormControl>
            <LocalizationProvider dateAdapter={AdapterDayjs}>
              <DateTimePicker
                sx={{ my: 2, display: "flex" }}
                name="acquirementTimestamp"
                label="Дата и время приобретения"
                value={dayjs(state.acquirementTimestamp)}
                onChange={value => dispatch(actions.setAcquirementTimestamp(value?.toJSON() ?? ""))}
              />
            </LocalizationProvider>
            <Button
              type="submit"
              fullWidth
              variant="contained"
              color="primary"
              sx={{ mt: 3, mb: 2 }}
            >
              Сохранить
            </Button>
          </Box>
        </Box>
        <Footer />
      </Container>
    </ThemeProvider>
  );
}
