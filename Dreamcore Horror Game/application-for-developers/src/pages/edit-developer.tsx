import * as React from "react";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import TextField from "@mui/material/TextField";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import { defaultTheme } from "../themes";
import { editDeveloper, getDeveloper, getDeveloperAccessLevels } from "../requests";
import { useNavigate, useParams } from "react-router-dom";
import { useEffect } from "react";
import { FormControl, InputLabel, MenuItem, Select } from "@mui/material";
import { displayName, Developer } from "../database";
import { useAppDispatch, useAppSelector } from "../redux/hooks";
import { actions, resetState } from "../redux/slices/developer-form-slice";
import Footer from "../components/footer";

export default function EditDeveloper() {
  const navigate = useNavigate();
  const params = useParams();

  const dispatch = useAppDispatch();
  const state = useAppSelector(state => state.developerForm);

  useEffect(() => {
    resetState(dispatch);
    getDeveloper(params.id)
      .then(developer => {
        dispatch(actions.setId(developer.id));
        dispatch(actions.setLogin(developer.login));
        dispatch(actions.setPassword(developer.password));
        dispatch(actions.setRefreshToken(developer.refreshToken));
        dispatch(actions.setDeveloperAccessLevelId(developer.developerAccessLevelId));
      });
    getDeveloperAccessLevels()
      .then(developerAccessLevels => dispatch(actions.setDeveloperAccessLevels(developerAccessLevels.items)));
  }, []);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const newDeveloper: Developer = new Developer();

    newDeveloper.id = state.id;
    newDeveloper.login = state.login;
    newDeveloper.password = state.password;
    newDeveloper.refreshToken = state.refreshToken;
    newDeveloper.developerAccessLevelId = state.developerAccessLevelId;

    const editedDeveloper: Developer | undefined = await editDeveloper(newDeveloper);

    if (editedDeveloper) {
      navigate(`/developer/${editedDeveloper.id}`);
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
            Редактирование разработчика
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
              value={state.login}
              onChange={e => dispatch(actions.setLogin(e.target.value))}
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
              Сохранить
            </Button>
          </Box>
        </Box>
        <Footer />
      </Container>
    </ThemeProvider>
  );
}
