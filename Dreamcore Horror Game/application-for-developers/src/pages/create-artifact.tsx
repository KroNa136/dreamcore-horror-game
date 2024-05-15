import * as React from "react";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import TextField from "@mui/material/TextField";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import { defaultTheme } from "../themes";
import { createArtifact, getRarityLevels } from "../requests";
import { useNavigate } from "react-router-dom";
import { Artifact, displayName, RarityLevel } from "../database";
import { useAppDispatch, useAppSelector } from "../redux/hooks";
import { actions, resetState } from "../redux/slices/artifact-form-slice";
import Footer from "../components/footer";
import { useEffect, useState } from "react";
import { FormControl, InputLabel, MenuItem, Select } from "@mui/material";

export default function CreateArtifact() {
  const navigate = useNavigate();

  const dispatch = useAppDispatch();
  const state = useAppSelector(state => state.artifactForm);

  const [rarityLevels, setRarityLevels] = useState<RarityLevel[]>([]);

  useEffect(() => {
    resetState(dispatch);
    getRarityLevels()
      .then(rarityLevels => setRarityLevels(rarityLevels.items));
  }, []);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const newArtifact: Artifact = new Artifact();

    newArtifact.assetName = state.assetName;
    newArtifact.rarityLevelId = state.rarityLevelId;

    const createdArtifact: Artifact | undefined = await createArtifact(newArtifact);

    if (createdArtifact) {
      navigate(`/artifact/${createdArtifact.id}`);
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
            Создание артефакта
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
            <FormControl sx={{ my: 1, minWidth: 120 }} fullWidth>
              <InputLabel id="rarityLevelLabel">Уровень редкости</InputLabel>
              <Select
                labelId="rarityLevelLabel"
                id="rarityLevel"
                label="Уровень редкости"
                name="rarityLevel"
                value={state.rarityLevelId ?? ""}
                onChange={e => dispatch(actions.setRarityLevelId(e.target.value))}
              >
                <MenuItem value="">
                  <em>Не выбрано</em>
                </MenuItem>
                {rarityLevels.map(rarityLevel => (
                  <MenuItem key={rarityLevel.id} value={rarityLevel.id}>{displayName(rarityLevel)}</MenuItem>
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
