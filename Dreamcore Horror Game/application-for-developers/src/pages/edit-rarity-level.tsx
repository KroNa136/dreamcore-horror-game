import * as React from "react";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import TextField from "@mui/material/TextField";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import { defaultTheme } from "../themes";
import { editRarityLevel, getRarityLevel } from "../requests";
import { useNavigate, useParams } from "react-router-dom";
import { useEffect } from "react";
import { RarityLevel } from "../database";
import { useAppDispatch, useAppSelector } from "../redux/hooks";
import { actions, resetState } from "../redux/slices/rarity-level-form-slice";
import Footer from "../components/footer";

export default function EditRarityLevel() {
  const navigate = useNavigate();
  const params = useParams();

  const dispatch = useAppDispatch();
  const state = useAppSelector(state => state.rarityLevelForm);

  useEffect(() => {
    resetState(dispatch);
    getRarityLevel(params.id)
      .then(rarityLevel => {
        dispatch(actions.setId(rarityLevel.id));
        dispatch(actions.setAssetName(rarityLevel.assetName));
        dispatch(actions.setProbability(rarityLevel.probability));
      });
  }, []);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const newRarityLevel: RarityLevel = new RarityLevel();

    newRarityLevel.id = state.id;
    newRarityLevel.assetName = state.assetName;
    newRarityLevel.probability = state.probability;

    const editedRarityLevel: RarityLevel | undefined = await editRarityLevel(newRarityLevel);

    if (editedRarityLevel) {
      navigate(`/rarityLevel/${editedRarityLevel.id}`);
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
            Редактирование уровня редкости
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
            <TextField
              margin="normal"
              required
              fullWidth
              id="probability"
              name="probability"
              label="Вероятность"
              value={state.probability}
              onChange={e => dispatch(actions.setProbability(Number(e.target.value)))}
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
