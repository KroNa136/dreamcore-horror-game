import * as React from "react";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import TextField from "@mui/material/TextField";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import { defaultTheme } from "../themes";
import { createRarityLevel } from "../requests";
import { useNavigate } from "react-router-dom";
import { RarityLevel } from "../database";
import { useAppDispatch, useAppSelector } from "../redux/hooks";
import { actions, resetState } from "../redux/slices/rarity-level-form-slice";
import Footer from "../components/footer";
import { useEffect } from "react";

export default function CreateRarityLevel() {
  const navigate = useNavigate();

  const dispatch = useAppDispatch();
  const state = useAppSelector(state => state.rarityLevelForm);

  useEffect(() => {
    resetState(dispatch);
  }, []);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const newRarityLevel: RarityLevel = new RarityLevel();

    newRarityLevel.assetName = state.assetName;
    newRarityLevel.probability = state.probability;

    const createdRarityLevel: RarityLevel | undefined = await createRarityLevel(newRarityLevel);

    if (createdRarityLevel) {
      navigate(`/rarityLevel/${createdRarityLevel.id}`);
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
            Создание уровня редкости
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
              onChange={e => dispatch(actions.setAssetName(e.target.value))}
            />
            <TextField
              margin="normal"
              required
              fullWidth
              id="probability"
              name="probability"
              label="Вероятность"
              onChange={e => dispatch(actions.setProbability(Number(e.target.value)))}
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
