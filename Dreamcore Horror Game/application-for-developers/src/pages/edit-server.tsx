import * as React from "react";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import TextField from "@mui/material/TextField";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import { defaultTheme } from "../themes";
import { editServer, getServer } from "../requests";
import { useNavigate, useParams } from "react-router-dom";
import { useEffect } from "react";
import { Checkbox, FormControlLabel } from "@mui/material";
import { Server } from "../database";
import { useAppDispatch, useAppSelector } from "../redux/hooks";
import { actions, resetState } from "../redux/slices/server-form-slice";
import Footer from "../components/footer";

export default function EditServer() {
  const navigate = useNavigate();
  const params = useParams();

  const dispatch = useAppDispatch();
  const state = useAppSelector(state => state.serverForm);

  useEffect(() => {
    resetState(dispatch);
    getServer(params.id)
      .then(server => {
        dispatch(actions.setId(server.id));
        dispatch(actions.setIpAddress(server.ipAddress));
        dispatch(actions.setPassword(server.password));
        dispatch(actions.setRefreshToken(server.refreshToken));
        dispatch(actions.setPlayerCapacity(server.playerCapacity));
        dispatch(actions.setIsOnline(server.isOnline));
      });
  }, []);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const newServer: Server = new Server();

    newServer.id = state.id;
    newServer.ipAddress = state.ipAddress;
    newServer.password = state.password;
    newServer.refreshToken = state.refreshToken;
    newServer.playerCapacity = state.playerCapacity;
    newServer.isOnline = state.isOnline;

    const editedServer: Server | undefined = await editServer(newServer);

    if (editedServer) {
      navigate(`/server/${editedServer.id}`);
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
            Редактирование сервера
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
              value={state.ipAddress}
              onChange={e => dispatch(actions.setIpAddress(e.target.value))}
            />
            <TextField
              margin="normal"
              required
              fullWidth
              id="playerCapacity"
              name="playerCapacity"
              label="Вместимость"
              value={state.playerCapacity}
              onChange={e => dispatch(actions.setPlayerCapacity(Number(e.target.value)))}
            />
            <FormControlLabel
              control={<Checkbox id="isOnline" name="isOnline" checked={state.isOnline} />}
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
              Сохранить
            </Button>
          </Box>
        </Box>
        <Footer />
      </Container>
    </ThemeProvider>
  );
}
