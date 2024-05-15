import * as React from "react";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import TextField from "@mui/material/TextField";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import { defaultTheme } from "../themes";
import { createPlayer, getXpLevels } from "../requests";
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { Checkbox, FormControl, FormControlLabel, InputLabel, MenuItem, Select } from "@mui/material";
import { displayName, Player, XpLevel } from "../database";
import { DateTimePicker, LocalizationProvider } from "@mui/x-date-pickers";
import dayjs from "dayjs";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { useAppDispatch, useAppSelector } from "../redux/hooks";
import { actions, resetState } from "../redux/slices/player-form-slice";
import Footer from "../components/footer";

export default function CreatePlayer() {
  const navigate = useNavigate();

  const dispatch = useAppDispatch();
  const state = useAppSelector(state => state.playerForm);

  const [xpLevels, setXpLevels] = useState<XpLevel[]>([]);

  useEffect(() => {
    resetState(dispatch);
    getXpLevels()
      .then(xpLevels => setXpLevels(xpLevels.items));
  }, []);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const newPlayer: Player = new Player();

    newPlayer.username = state.username;
    newPlayer.email = state.email;
    newPlayer.password = state.password;
    newPlayer.registrationTimestamp = state.registrationTimestamp;
    newPlayer.collectOptionalData = state.collectOptionalData;
    newPlayer.xpLevelId = state.xpLevelId;
    newPlayer.xp = state.xp;
    newPlayer.abilityPoints = state.abilityPoints;
    newPlayer.spiritEnergyPoints = state.spiritEnergyPoints;

    const createdPlayer: Player | undefined = await createPlayer(newPlayer);

    if (createdPlayer) {
      navigate(`/player/${createdPlayer.id}`);
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
            Создание игрока
          </Typography>
          <Box component="form" onSubmit={handleSubmit} noValidate sx={{ mt: 1 }}>
            <TextField
              margin="normal"
              required
              fullWidth
              id="username"
              name="username"
              label="Никнейм"
              autoFocus
              onChange={e => dispatch(actions.setUsername(e.target.value))}
            />
            <TextField
              margin="normal"
              required
              fullWidth
              id="email"
              name="email"
              label="Адрес электронной почты"
              onChange={e => dispatch(actions.setEmail(e.target.value))}
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
            <LocalizationProvider dateAdapter={AdapterDayjs}>
              <DateTimePicker
                sx={{ my: 2, display: "flex" }}
                name="registrationTimestamp"
                label="Дата и время регистрации"
                value={dayjs(state.registrationTimestamp)}
                onChange={value => dispatch(actions.setRegistrationTimestamp(value?.toJSON() ?? ""))}
              />
            </LocalizationProvider>
            <FormControlLabel
              control={<Checkbox id="collectOptionalData" name="collectOptionalData" />}
              label="Разрешение на сбор опциональных данных"
              sx={{ mb: 2 }}
              onChange={(_, checked) => dispatch(actions.setCollectOptionalData(checked))}
            />
            <FormControl sx={{ my: 1, minWidth: 120 }} fullWidth>
              <InputLabel id="xpLevelLabel">Уровень опыта</InputLabel>
              <Select
                labelId="xpLevelLabel"
                id="xpLevel"
                label="Уровень опыта"
                name="xpLevel"
                value={state.xpLevelId ?? ""}
                onChange={e => dispatch(actions.setXpLevelId(e.target.value))}
              >
                <MenuItem value="">
                  <em>Не выбрано</em>
                </MenuItem>
                {xpLevels.map(xpLevel => (
                  <MenuItem key={xpLevel.id} value={xpLevel.id}>{displayName(xpLevel)}</MenuItem>
                ))}
              </Select>
            </FormControl>
            <TextField
              margin="normal"
              required
              fullWidth
              id="xp"
              name="xp"
              label="Опыт"
              onChange={e => dispatch(actions.setXp(Number(e.target.value)))}
            />
            <TextField
              margin="normal"
              required
              fullWidth
              id="abilityPoints"
              name="abilityPoints"
              label="Очки способностей"
              onChange={e => dispatch(actions.setAbilityPoints(Number(e.target.value)))}
            />
            <TextField
              margin="normal"
              required
              fullWidth
              id="spiritEnergyPoints"
              name="spiritEnergyPoints"
              label="Очки духовной энергии"
              onChange={e => dispatch(actions.setSpiritEnergyPoints(Number(e.target.value)))}
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
