import * as React from "react";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import TextField from "@mui/material/TextField";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import { defaultTheme } from "../themes";
import { editXpLevel, getXpLevel } from "../requests";
import { useNavigate, useParams } from "react-router-dom";
import { useEffect } from "react";
import { XpLevel } from "../database";
import { useAppDispatch, useAppSelector } from "../redux/hooks";
import { actions, resetState } from "../redux/slices/xp-level-form-slice";
import Footer from "../components/footer";

export default function EditXpLevel() {
  const navigate = useNavigate();
  const params = useParams();

  const dispatch = useAppDispatch();
  const state = useAppSelector(state => state.xpLevelForm);

  useEffect(() => {
    resetState(dispatch);
    getXpLevel(params.id)
      .then(xpLevel => {
        dispatch(actions.setId(xpLevel.id));
        dispatch(actions.setNumber(xpLevel.number));
        dispatch(actions.setRequiredXp(xpLevel.requiredXp));
      });
  }, []);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const newXpLevel: XpLevel = new XpLevel();

    newXpLevel.id = state.id;
    newXpLevel.number = state.number;
    newXpLevel.requiredXp = state.requiredXp;

    const editedXpLevel: XpLevel | undefined = await editXpLevel(newXpLevel);

    if (editedXpLevel) {
      navigate(`/xpLevel/${editedXpLevel.id}`);
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
            Редактирование уровня опыта
          </Typography>
          <Box component="form" onSubmit={handleSubmit} noValidate sx={{ mt: 1 }}>
            <TextField
              margin="normal"
              required
              fullWidth
              id="number"
              name="number"
              label="Номер"
              value={state.number}
              onChange={e => dispatch(actions.setNumber(Number(e.target.value)))}
            />
            <TextField
              margin="normal"
              required
              fullWidth
              id="requiredXp"
              name="requiredXp"
              label="Требуемый опыт"
              value={state.requiredXp}
              onChange={e => dispatch(actions.setRequiredXp(Number(e.target.value)))}
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
