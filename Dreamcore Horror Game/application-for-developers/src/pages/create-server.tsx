import * as React from "react";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import TextField from "@mui/material/TextField";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import { defaultTheme } from "../themes";
import { createServer } from "../requests";
import { useNavigate } from "react-router-dom";
import { useEffect } from "react";
import { Checkbox, FormControlLabel } from "@mui/material";
import { Server } from "../database";
import { useAppDispatch, useAppSelector } from "../redux/hooks";
import { actions, resetState } from "../redux/slices/server-form-slice";
import Footer from "../components/footer";

export default function CreateServer() {
  const navigate = useNavigate();

  const dispatch = useAppDispatch();
  const state = useAppSelector(state => state.serverForm);

  useEffect(() => {
    resetState(dispatch);
  }, []);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const newServer: Server = new Server();

    newServer.ipAddress = state.ipAddress;
    newServer.password = state.password;
    newServer.playerCapacity = state.playerCapacity;
    newServer.isOnline = state.isOnline;

    const createdServer: Server | undefined = await createServer(newServer);

    if (createdServer) {
      navigate(`/server/${createdServer.id}`);
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
            Создание сервера
          </Typography>
          <Box component="form" onSubmit={handleSubmit} noValidate sx={{ mt: 1 }}>
            <TextField
              margin="normal"
              required
              fullWidth
              id="ipAddress"
              name="ipAddress"
              label="IP-адрес"
              autoFocus
              onChange={e => dispatch(actions.setIpAddress(e.target.value))}
            />
            <TextField
              margin="normal"
              required
              fullWidth
              id="password"
              name="password"
              label="Пароль"
              type="password"
              onChange={e => dispatch(actions.setPassword(e.target.value))}
            />
            <TextField
              margin="normal"
              required
              fullWidth
              id="playerCapacity"
              name="playerCapacity"
              label="Вместимость"
              onChange={e => dispatch(actions.setPlayerCapacity(Number(e.target.value)))}
            />
            <FormControlLabel
              control={<Checkbox id="isOnline" name="isOnline" />}
              label="В сети"
              sx={{ mb: 2 }}
              onChange={(_, checked) => dispatch(actions.setIsOnline(checked))}
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
