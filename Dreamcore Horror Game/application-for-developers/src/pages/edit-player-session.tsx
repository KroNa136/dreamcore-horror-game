import * as React from "react";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import { defaultTheme } from "../themes";
import { editPlayerSession, getPlayerSession, getPlayers, getGameSessions, getCreatures } from "../requests";
import { useNavigate, useParams } from "react-router-dom";
import { useEffect } from "react";
import { Checkbox, FormControl, FormControlLabel, InputLabel, MenuItem, Select, TextField } from "@mui/material";
import { displayName, PlayerSession } from "../database";
import { DateTimePicker, LocalizationProvider } from "@mui/x-date-pickers";
import dayjs from "dayjs";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { useAppDispatch, useAppSelector } from "../redux/hooks";
import { actions, resetState } from "../redux/slices/player-session-form-slice";
import Footer from "../components/footer";

export default function EditPlayerSession() {
  const navigate = useNavigate();
  const params = useParams();

  const dispatch = useAppDispatch();
  const state = useAppSelector(state => state.playerSessionForm);

  useEffect(() => {
    resetState(dispatch);
    getPlayerSession(params.id)
      .then(playerSession => {
        dispatch(actions.setId(playerSession.id));
        dispatch(actions.setGameSessionId(playerSession.gameSessionId));
        dispatch(actions.setPlayerId(playerSession.playerId));
        dispatch(actions.setStartTimestamp(playerSession.startTimestamp));
        dispatch(actions.setEndTimestamp(playerSession.endTimestamp));
        dispatch(actions.setIsCompleted(playerSession.isCompleted));
        dispatch(actions.setIsWon(playerSession.isWon));
        dispatch(actions.setTimeAlive(playerSession.timeAlive));
        dispatch(actions.setPlayedAsCreature(playerSession.playedAsCreature));
        dispatch(actions.setUsedCreatureId(playerSession.usedCreatureId));
        dispatch(actions.setSelfReviveCount(playerSession.selfReviveCount));
        dispatch(actions.setAllyReviveCount(playerSession.allyReviveCount));
      });
    getGameSessions()
      .then(gameSessions => dispatch(actions.setGameSessions(gameSessions.items)));
    getPlayers()
      .then(players => dispatch(actions.setPlayers(players.items)));
    getCreatures()
      .then(creatures => dispatch(actions.setCreatures(creatures.items)));
  }, []);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const newPlayerSession: PlayerSession = new PlayerSession();

    newPlayerSession.id = state.id;
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

    const editedPlayerSession: PlayerSession | undefined = await editPlayerSession(newPlayerSession);

    if (editedPlayerSession) {
      navigate(`/playerSession/${editedPlayerSession.id}`);
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
            Редактирование сеанса игрока
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
                {state.gameSessions.map(gameSession => (
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
                {state.players.map(player => (
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
              control={<Checkbox id="isCompleted" name="isCompleted" checked={state.isCompleted ?? false} />}
              label="Игра завершена"
              sx={{ mb: 2 }}
              onChange={(_, checked) => dispatch(actions.setIsCompleted(checked))}
            />
            <FormControlLabel
              control={<Checkbox id="isWon" name="isWon" checked={state.isWon ?? false} />}
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
              value={state.timeAlive ?? ""}
              onChange={e => dispatch(actions.setTimeAlive(e.target.value !== "" ? e.target.value : null))}
            />
            <FormControlLabel
              control={<Checkbox id="playedAsCreature" name="playedAsCreature" checked={state.playedAsCreature ?? false} />}
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
                onChange={e => dispatch(actions.setUsedCreatureId(e.target.value !== "" ? e.target.value : null))}
              >
                <MenuItem value="">
                  <em>Не выбрано</em>
                </MenuItem>
                {state.creatures.map(creature => (
                  <MenuItem key={creature.id} value={creature.id}>{displayName(creature)}</MenuItem>
                ))}
              </Select>
            </FormControl>
            <TextField
              margin="normal"
              fullWidth
              id="selfReviveCount"
              name="selfReviveCount"
              label="Количество самовозрождений"
              value={state.selfReviveCount ?? ""}
              onChange={e => dispatch(actions.setSelfReviveCount(e.target.value !== "" ? Number(e.target.value) : null))}
            />
            <TextField
              margin="normal"
              fullWidth
              id="allyReviveCount"
              name="allyReviveCount"
              label="Количество возрождений союзников"
              value={state.allyReviveCount ?? ""}
              onChange={e => dispatch(actions.setAllyReviveCount(e.target.value !== "" ? Number(e.target.value) : null))}
            />
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
