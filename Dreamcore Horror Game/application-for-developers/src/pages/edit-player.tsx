import * as React from 'react';
import Button from '@mui/material/Button';
import CssBaseline from '@mui/material/CssBaseline';
import TextField from '@mui/material/TextField';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Container from '@mui/material/Container';
import { ThemeProvider } from '@mui/material/styles';
import { defaultTheme } from "../themes";
import { editPlayer, getPlayer, getXpLevels } from '../requests';
import { useNavigate, useParams } from 'react-router-dom';
import { useEffect, useState } from 'react';
import { Checkbox, FormControl, FormControlLabel, InputLabel, MenuItem, Select } from '@mui/material';
import { displayName, Player, XpLevel } from '../database';
import { DateTimePicker, LocalizationProvider } from '@mui/x-date-pickers';
import dayjs from 'dayjs';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { useAppDispatch, useAppSelector } from '../redux/hooks';
import { actions } from '../redux/slices/player-form-slice';
import Footer from '../components/footer';

export default function EditPlayer() {
  const navigate = useNavigate();
  const params = useParams();

  const dispatch = useAppDispatch();
  const state = useAppSelector(state => state.playerForm);

  useEffect(() => {
    getPlayer(params.id)
      .then(player => {
        dispatch(actions.setId(player.id));
        dispatch(actions.setUsername(player.username));
        dispatch(actions.setEmail(player.email));
        dispatch(actions.setPassword(player.password));
        dispatch(actions.setRefreshToken(player.refreshToken));
        dispatch(actions.setRegistrationTimestamp(player.registrationTimestamp));
        dispatch(actions.setCollectOptionalData(player.collectOptionalData));
        dispatch(actions.setXpLevelId(player.xpLevelId));
        dispatch(actions.setXp(player.xp));
        dispatch(actions.setAbilityPoints(player.abilityPoints));
        dispatch(actions.setSpiritEnergyPoints(player.spiritEnergyPoints));
      });
  }, []);

  const [xpLevels, setXpLevels] = useState<XpLevel[]>([]);

  useEffect(() => {
    getXpLevels()
      .then(xpLevels => setXpLevels(xpLevels.items));
  }, []);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const newPlayer: Player = new Player();

    newPlayer.id = state.id;
    newPlayer.username = state.username;
    newPlayer.email = state.email;
    newPlayer.password = state.password;
    newPlayer.refreshToken = state.refreshToken;
    newPlayer.registrationTimestamp = state.registrationTimestamp;
    newPlayer.collectOptionalData = state.collectOptionalData;
    newPlayer.xpLevelId = state.xpLevelId;
    newPlayer.xp = state.xp;
    newPlayer.abilityPoints = state.abilityPoints;
    newPlayer.spiritEnergyPoints = state.spiritEnergyPoints;

    const editedPlayer: Player | undefined = await editPlayer(newPlayer);

    if (editedPlayer) {
      navigate(`/player/${editedPlayer.id}`);
    }
  };

  return (
    <ThemeProvider theme={defaultTheme}>
      <Container component="main" maxWidth="xs">
        <CssBaseline />
        <Box
          sx={{
            marginTop: 10,
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center',
          }}
        >
          <Typography component="h4" variant="h4">
            Редактирование игрока
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
              value={state.username}
              onChange={e => dispatch(actions.setUsername(e.target.value))}
            />
            <TextField
              margin="normal"
              required
              fullWidth
              id="email"
              name="email"
              label="Адрес электронной почты"
              value={state.email}
              onChange={e => dispatch(actions.setEmail(e.target.value))}
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
              control={<Checkbox id="collectOptionalData" name="collectOptionalData" checked={state.collectOptionalData} />}
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
              value={state.xp}
              onChange={e => dispatch(actions.setXp(Number(e.target.value)))}
            />
            <TextField
              margin="normal"
              required
              fullWidth
              id="abilityPoints"
              name="abilityPoints"
              label="Очки способностей"
              value={state.abilityPoints}
              onChange={e => dispatch(actions.setAbilityPoints(Number(e.target.value)))}
            />
            <TextField
              margin="normal"
              required
              fullWidth
              id="spiritEnergyPoints"
              name="spiritEnergyPoints"
              label="Очки духовной энергии"
              value={state.spiritEnergyPoints}
              onChange={e => dispatch(actions.setSpiritEnergyPoints(Number(e.target.value)))}
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
