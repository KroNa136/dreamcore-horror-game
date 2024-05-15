import * as React from "react";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import TextField from "@mui/material/TextField";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import { defaultTheme } from "../themes";
import { createGameMode } from "../requests";
import { useNavigate } from "react-router-dom";
import { Checkbox, FormControlLabel } from "@mui/material";
import { GameMode } from "../database";
import { useAppDispatch, useAppSelector } from "../redux/hooks";
import { actions, resetState } from "../redux/slices/game-mode-form-slice";
import Footer from "../components/footer";
import { useEffect } from "react";

export default function CreateGameMode() {
  const navigate = useNavigate();

  const dispatch = useAppDispatch();
  const state = useAppSelector(state => state.gameModeForm);

  useEffect(() => {
    resetState(dispatch);
  }, []);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const newGameMode: GameMode = new GameMode();

    newGameMode.assetName = state.assetName;
    newGameMode.maxPlayers = state.maxPlayers;
    newGameMode.timeLimit = state.timeLimit;
    newGameMode.isActive = state.isActive;

    const createdGameMode: GameMode | undefined = await createGameMode(newGameMode);

    if (createdGameMode) {
      navigate(`/gameMode/${createdGameMode.id}`);
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
            Создание игрового режима
          </Typography>
          <Box component="form" onSubmit={handleSubmit} noValidate sx={{ mt: 1 }}>
            <TextField
              margin="normal"
              required
              fullWidth
              id="assetName"
              name="assetName"
              label="Название ассета"
              autoFocus
              onChange={e => dispatch(actions.setAssetName(e.target.value))}
            />
            <TextField
              margin="normal"
              fullWidth
              id="maxPlayers"
              name="maxPlayers"
              label="Максимальное число игроков"
              onChange={e => dispatch(actions.setMaxPlayers(e.target.value !== "" ? Number(e.target.value) : null))}
            />
            <TextField
              margin="normal"
              fullWidth
              id="timeLimit"
              name="timeLimit"
              label="Ограничение по времени (чч:мм:сс)"
              onChange={e => dispatch(actions.setTimeLimit(e.target.value !== "" ? e.target.value : null))}
            />
            <FormControlLabel
              control={<Checkbox id="isActive" name="isActive" />}
              label="Активен"
              sx={{ mb: 2 }}
              onChange={(_, checked) => dispatch(actions.setIsActive(checked))}
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
