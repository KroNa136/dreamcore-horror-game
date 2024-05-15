import * as React from "react";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import TextField from "@mui/material/TextField";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import { defaultTheme } from "../themes";
import { editDeveloperAccessLevel, getDeveloperAccessLevel } from "../requests";
import { useNavigate, useParams } from "react-router-dom";
import { useEffect } from "react";
import { DeveloperAccessLevel } from "../database";
import { useAppDispatch, useAppSelector } from "../redux/hooks";
import { actions, resetState } from "../redux/slices/developer-access-level-form-slice";
import Footer from "../components/footer";

export default function EditDeveloperAccessLevel() {
  const navigate = useNavigate();
  const params = useParams();

  const dispatch = useAppDispatch();
  const state = useAppSelector(state => state.developerAccessLevelForm);

  useEffect(() => {
    resetState(dispatch);
    getDeveloperAccessLevel(params.id)
      .then(developerAccessLevel => {
        dispatch(actions.setId(developerAccessLevel.id));
        dispatch(actions.setName(developerAccessLevel.name));
      });
  }, []);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const newDeveloperAccessLevel: DeveloperAccessLevel = new DeveloperAccessLevel();

    newDeveloperAccessLevel.id = state.id;
    newDeveloperAccessLevel.name = state.name;

    const editedDeveloperAccessLevel: DeveloperAccessLevel | undefined = await editDeveloperAccessLevel(newDeveloperAccessLevel);

    if (editedDeveloperAccessLevel) {
      navigate(`/developerAccessLevel/${editedDeveloperAccessLevel.id}`);
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
            Редактирование уровня доступа разработчика
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
              value={state.name}
              onChange={e => dispatch(actions.setName(e.target.value))}
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
