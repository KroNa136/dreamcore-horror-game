import * as React from "react";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import { defaultTheme } from "../themes";
import { createPlayerSession, getCreatures, getGameSessions, getPlayers } from "../requests";
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { Checkbox, FormControl, FormControlLabel, InputLabel, MenuItem, Select, TextField } from "@mui/material";
import { Creature, displayName, GameSession, Player, PlayerSession } from "../database";
import { DateTimePicker, LocalizationProvider } from "@mui/x-date-pickers";
import dayjs from "dayjs";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { useAppDispatch, useAppSelector } from "../redux/hooks";
import { actions, resetState } from "../redux/slices/player-session-form-slice";
import Footer from "../components/footer";

export default function CreatePlayerSession() {
  const navigate = useNavigate();

  const dispatch = useAppDispatch();
  const state = useAppSelector(state => state.playerSessionForm);

  const [gameSessions, setGameSessions] = useState<GameSession[]>([]);
  const [players, setPlayers] = useState<Player[]>([]);
  const [creatures, setCreatures] = useState<Creature[]>([]);

  useEffect(() => {
    resetState(dispatch);
    getGameSessions()
      .then(gameSessions => setGameSessions(gameSessions.items));
    getPlayers()
      .then(players => setPlayers(players.items));
    getCreatures()
      .then(creatures => setCreatures(creatures.items));
  }, []);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const newPlayerSession: PlayerSession = new PlayerSession();

    newPlayerSession.gameSessionId = state.gameSessionId;
    newPlayerSession.playerId = state.playerId;
    newPlayerSession.startTimestamp = state.startTimestamp;
    newPlayerSession.endTimestamp = state.endTimestamp;
    newPlayerSession.isCompleted = state.isCompleted;
    newPlayerSession.isWon = state.isWon;
    newPlayerSession.timeAlive = state.timeAlive;
    newPlayerSession.playedAsCreature = state.playedAsCreature;
    newPlayerSession.usedCreatureId = state.usedCreatureId;
    newPlayerSession.selfReviveCount = state.selfReviveCount;
    newPlayerSession.allyReviveCount = state.allyReviveCount;

    const createdPlayerSession: PlayerSession | undefined = await createPlayerSession(newPlayerSession);

    if (createdPlayerSession) {
      navigate(`/playerSession/${createdPlayerSession.id}`);
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
            Создание сеанса игрока
          </Typography>
          <Box component="form" onSubmit={handleSubmit} noValidate sx={{ mt: 1 }}>
            <FormControl sx={{ my: 1, minWidth: 120 }} fullWidth>
              <InputLabel id="gameSessionLabel">Игровой сеанс</InputLabel>
              <Select
                labelId="gameSessionLabel"
                id="gameSession"
                label="Игровой сеанс"
                name="gameSession"
                value={state.gameSessionId ?? ""}
                onChange={e => dispatch(actions.setGameSessionId(e.target.value))}
              >
                <MenuItem value="">
                  <em>Не выбрано</em>
                </MenuItem>
                {gameSessions.map(gameSession => (
                  <MenuItem key={gameSession.id} value={gameSession.id}>{displayName(gameSession)}</MenuItem>
                ))}
              </Select>
            </FormControl>
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
            <LocalizationProvider dateAdapter={AdapterDayjs}>
              <DateTimePicker
                sx={{ my: 2, display: "flex" }}
                name="startTimestamp"
                label="Дата и время начала"
                value={dayjs(state.startTimestamp)}
                onChange={value => dispatch(actions.setStartTimestamp(value?.toJSON() ?? ""))}
              />
            </LocalizationProvider>
            <LocalizationProvider dateAdapter={AdapterDayjs}>
              <DateTimePicker
                sx={{ my: 2, display: "flex" }}
                name="endTimestamp"
                label="Дата и время окончания"
                value={state.endTimestamp ? dayjs(state.endTimestamp) : undefined}
                onChange={value => dispatch(actions.setEndTimestamp(value?.toJSON() ?? ""))}
              />
            </LocalizationProvider>
            <FormControlLabel
              control={<Checkbox id="isCompleted" name="isCompleted" />}
              label="Игра завершена"
              sx={{ mb: 2 }}
              onChange={(_, checked) => dispatch(actions.setIsCompleted(checked))}
            />
            <FormControlLabel
              control={<Checkbox id="isWon" name="isWon" />}
              label="Победа"
              sx={{ mb: 2 }}
              onChange={(_, checked) => dispatch(actions.setIsWon(checked))}
            />
            <TextField
              margin="normal"
              fullWidth
              id="timeAlive"
              name="timeAlive"
              label="Время жизни (чч:мм:сс)"
              onChange={e => dispatch(actions.setTimeAlive(e.target.value !== "" ? e.target.value : null))}
            />
            <FormControlLabel
              control={<Checkbox id="playedAsCreature" name="playedAsCreature" />}
              label="Использовано существо"
              sx={{ mb: 2 }}
              onChange={(_, checked) => dispatch(actions.setPlayedAsCreature(checked))}
            />
            <FormControl sx={{ my: 1, minWidth: 120 }} fullWidth>
              <InputLabel id="usedCreatureLabel">Использованное существо</InputLabel>
              <Select
                labelId="usedCreatureLabel"
                id="usedCreature"
                label="Использованное существо"
                name="usedCreature"
                value={state.usedCreatureId ?? ""}
                onChange={e => dispatch(actions.setUsedCreatureId(e.target.value))}
              >
                <MenuItem value="">
                  <em>Не выбрано</em>
                </MenuItem>
                {creatures.map(creature => (
                  <MenuItem key={creature.id} value={creature.id}>{displayName(creature)}</MenuItem>
                ))}
              </Select>
            </FormControl>
            <TextField
              margin="normal"
              required
              fullWidth
              id="selfReviveCount"
              name="selfReviveCount"
              label="Количество самовозрождений"
              onChange={e => dispatch(actions.setSelfReviveCount(Number(e.target.value)))}
            />
            <TextField
              margin="normal"
              required
              fullWidth
              id="allyReviveCount"
              name="allyReviveCount"
              label="Количество возрождений союзников"
              onChange={e => dispatch(actions.setAllyReviveCount(Number(e.target.value)))}
            />
            <Button
              type="submit"
              fullWidth
              variant="contained"
              color="primary"
              sx={{ mt: 3, mb: 2 }}
            >
              Создать
            </Button>
          </Box>
        </Box>
        <Footer />
      </Container>
    </ThemeProvider>
  );
}
