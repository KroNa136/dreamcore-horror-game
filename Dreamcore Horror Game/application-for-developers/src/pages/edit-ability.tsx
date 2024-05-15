import * as React from "react";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import TextField from "@mui/material/TextField";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import { defaultTheme } from "../themes";
import { editAbility, getAbility } from "../requests";
import { useNavigate, useParams } from "react-router-dom";
import { useEffect } from "react";
import { Ability } from "../database";
import { useAppDispatch, useAppSelector } from "../redux/hooks";
import { actions, resetState } from "../redux/slices/ability-form-slice";
import Footer from "../components/footer";

export default function EditAbility() {
  const navigate = useNavigate();
  const params = useParams();

  const dispatch = useAppDispatch();
  const state = useAppSelector(state => state.abilityForm);

  useEffect(() => {
    resetState(dispatch);
    getAbility(params.id)
      .then(ability => {
        dispatch(actions.setId(ability.id));
        dispatch(actions.setAssetName(ability.assetName));
      });
  }, []);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const newAbility: Ability = new Ability();

    newAbility.id = state.id;
    newAbility.assetName = state.assetName;

    const editedAbility: Ability | undefined = await editAbility(newAbility);

    if (editedAbility) {
      navigate(`/ability/${editedAbility.id}`);
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
            Редактирование способности
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
              value={state.assetName}
              onChange={e => dispatch(actions.setAssetName(e.target.value))}
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
