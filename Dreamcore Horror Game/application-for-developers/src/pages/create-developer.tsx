import * as React from "react";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import TextField from "@mui/material/TextField";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import { defaultTheme } from "../themes";
import { createDeveloper, getDeveloperAccessLevels } from "../requests";
import { useNavigate } from "react-router-dom";
import { useEffect } from "react";
import { FormControl, InputLabel, MenuItem, Select } from "@mui/material";
import { displayName, Developer } from "../database";
import { useAppDispatch, useAppSelector } from "../redux/hooks";
import { actions, resetState } from "../redux/slices/developer-form-slice";
import Footer from "../components/footer";

export default function CreateDeveloper() {
  const navigate = useNavigate();

  const dispatch = useAppDispatch();
  const state = useAppSelector(state => state.developerForm);

  useEffect(() => {
    resetState(dispatch);
    getDeveloperAccessLevels()
      .then(developerAccessLevels => dispatch(actions.setDeveloperAccessLevels(developerAccessLevels.items)));
  }, []);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const newDeveloper: Developer = new Developer();

    newDeveloper.login = state.login;
    newDeveloper.password = state.password;
    newDeveloper.developerAccessLevelId = state.developerAccessLevelId;

    const createdDeveloper: Developer | undefined = await createDeveloper(newDeveloper);

    if (createdDeveloper) {
      navigate(`/developer/${createdDeveloper.id}`);
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
            Создание разработчика
          </Typography>
          <Box component="form" onSubmit={handleSubmit} noValidate sx={{ mt: 1 }}>
            <TextField
              margin="normal"
              required
              fullWidth
              id="login"
              name="login"
              label="Логин"
              autoFocus
              onChange={e => dispatch(actions.setLogin(e.target.value))}
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
            <FormControl sx={{ my: 1, minWidth: 120 }} fullWidth>
              <InputLabel id="developerAccessLevelLabel">Уровень доступа разработчиков</InputLabel>
              <Select
                labelId="developerAccessLevelLabel"
                id="developerAccessLevel"
                label="Уровень доступа разработчиков"
                name="developerAccessLevel"
                value={state.developerAccessLevelId ?? ""}
                onChange={e => dispatch(actions.setDeveloperAccessLevelId(e.target.value))}
              >
                <MenuItem value="">
                  <em>Не выбрано</em>
                </MenuItem>
                {state.developerAccessLevels.map(developerAccessLevel => (
                  <MenuItem key={developerAccessLevel.id} value={developerAccessLevel.id}>{displayName(developerAccessLevel)}</MenuItem>
                ))}
              </Select>
            </FormControl>
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
