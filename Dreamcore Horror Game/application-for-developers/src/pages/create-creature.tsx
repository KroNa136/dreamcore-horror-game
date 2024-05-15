import * as React from "react";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import TextField from "@mui/material/TextField";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import { defaultTheme } from "../themes";
import { createCreature, getXpLevels } from "../requests";
import { useNavigate } from "react-router-dom";
import { Creature, displayName, XpLevel } from "../database";
import { useAppDispatch, useAppSelector } from "../redux/hooks";
import { actions, resetState } from "../redux/slices/creature-form-slice";
import Footer from "../components/footer";
import { useEffect, useState } from "react";
import { FormControl, InputLabel, MenuItem, Select } from "@mui/material";

export default function CreateCreature() {
  const navigate = useNavigate();

  const dispatch = useAppDispatch();
  const state = useAppSelector(state => state.creatureForm);

  const [xpLevels, setXpLevels] = useState<XpLevel[]>([]);

  useEffect(() => {
    resetState(dispatch);
    getXpLevels()
      .then(xpLevels => setXpLevels(xpLevels.items));
  }, []);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const newCreature: Creature = new Creature();

    newCreature.assetName = state.assetName;
    newCreature.requiredXpLevelId = state.requiredXpLevelId;
    newCreature.health = state.health;
    newCreature.movementSpeed = state.movementSpeed;

    const createdCreature: Creature | undefined = await createCreature(newCreature);

    if (createdCreature) {
      navigate(`/creature/${createdCreature.id}`);
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
            Создание существа
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
              <InputLabel id="requiredXpLevelLabel">Требуемый уровень опыта</InputLabel>
              <Select
                labelId="requiredXpLevelLabel"
                id="requiredXpLevel"
                label="Требуемый уровень опыта"
                name="requiredXpLevel"
                value={state.requiredXpLevelId ?? ""}
                onChange={e => dispatch(actions.setRequiredXpLevelId(e.target.value))}
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
              id="health"
              name="health"
              label="Здоровье"
              onChange={e => dispatch(actions.setHealth(Number(e.target.value)))}
            />
            <TextField
              margin="normal"
              required
              fullWidth
              id="movementSpeed"
              name="movementSpeed"
              label="Скорость передвижения"
              onChange={e => dispatch(actions.setMovementSpeed(Number(e.target.value)))}
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
