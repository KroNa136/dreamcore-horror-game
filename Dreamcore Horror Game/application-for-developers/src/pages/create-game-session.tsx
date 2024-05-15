import * as React from "react";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import { defaultTheme } from "../themes";
import { createGameSession, getGameModes, getServers } from "../requests";
import { useNavigate } from "react-router-dom";
import { useEffect } from "react";
import { FormControl, InputLabel, MenuItem, Select } from "@mui/material";
import { displayName, GameSession } from "../database";
import { DateTimePicker, LocalizationProvider } from "@mui/x-date-pickers";
import dayjs from "dayjs";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { useAppDispatch, useAppSelector } from "../redux/hooks";
import { actions, resetState } from "../redux/slices/game-session-form-slice";
import Footer from "../components/footer";

export default function CreateGameSession() {
  const navigate = useNavigate();

  const dispatch = useAppDispatch();
  const state = useAppSelector(state => state.gameSessionForm);

  useEffect(() => {
    resetState(dispatch);
    getServers()
      .then(servers => dispatch(actions.setServers(servers.items)));
    getGameModes()
      .then(gameModes => dispatch(actions.setGameModes(gameModes.items)));
  }, []);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const newGameSession: GameSession = new GameSession();

    newGameSession.serverId = state.serverId;
    newGameSession.gameModeId = state.gameModeId;
    newGameSession.startTimestamp = state.startTimestamp;
    newGameSession.endTimestamp = state.endTimestamp;

    const createdGameSession: GameSession | undefined = await createGameSession(newGameSession);

    if (createdGameSession) {
      navigate(`/gameSession/${createdGameSession.id}`);
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
            Создание игрового сеанса
          </Typography>
          <Box component="form" onSubmit={handleSubmit} noValidate sx={{ mt: 1 }}>
            <FormControl sx={{ my: 1, minWidth: 120 }} fullWidth>
              <InputLabel id="serverLabel">Сервер</InputLabel>
              <Select
                labelId="serverLabel"
                id="server"
                label="Сервер"
                name="server"
                value={state.serverId ?? ""}
                onChange={e => dispatch(actions.setServerId(e.target.value))}
              >
                <MenuItem value="">
                  <em>Не выбрано</em>
                </MenuItem>
                {state.servers.map(server => (
                  <MenuItem key={server.id} value={server.id}>{displayName(server)}</MenuItem>
                ))}
              </Select>
            </FormControl>
            <FormControl sx={{ my: 1, minWidth: 120 }} fullWidth>
              <InputLabel id="gameModeLabel">Игровой режим</InputLabel>
              <Select
                labelId="gameModeLabel"
                id="gameMode"
                label="Игровой режим"
                name="gameMode"
                value={state.gameModeId ?? ""}
                onChange={e => dispatch(actions.setGameModeId(e.target.value))}
              >
                <MenuItem value="">
                  <em>Не выбрано</em>
                </MenuItem>
                {state.gameModes.map(gameMode => (
                  <MenuItem key={gameMode.id} value={gameMode.id}>{displayName(gameMode)}</MenuItem>
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
