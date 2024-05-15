import * as React from "react";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import TextField from "@mui/material/TextField";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import { defaultTheme } from "../themes";
import { editCreature, getCreature, getXpLevels } from "../requests";
import { useNavigate, useParams } from "react-router-dom";
import { useEffect } from "react";
import { Creature, displayName } from "../database";
import { useAppDispatch, useAppSelector } from "../redux/hooks";
import { actions, resetState } from "../redux/slices/creature-form-slice";
import Footer from "../components/footer";
import { FormControl, InputLabel, MenuItem, Select } from "@mui/material";

export default function EditCreature() {
  const navigate = useNavigate();
  const params = useParams();

  const dispatch = useAppDispatch();
  const state = useAppSelector(state => state.creatureForm);

  useEffect(() => {
    resetState(dispatch);
    getCreature(params.id)
      .then(creature => {
        dispatch(actions.setId(creature.id));
        dispatch(actions.setAssetName(creature.assetName));
        dispatch(actions.setRequiredXpLevelId(creature.requiredXpLevelId));
        dispatch(actions.setHealth(creature.health));
        dispatch(actions.setMovementSpeed(creature.movementSpeed));
      });
    getXpLevels()
      .then(xpLevels => dispatch(actions.setXpLevels(xpLevels.items)));
  }, []);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const newCreature: Creature = new Creature();

    newCreature.id = state.id;
    newCreature.assetName = state.assetName;
    newCreature.requiredXpLevelId = state.requiredXpLevelId;
    newCreature.health = state.health;
    newCreature.movementSpeed = state.movementSpeed;

    const editedCreature: Creature | undefined = await editCreature(newCreature);

    if (editedCreature) {
      navigate(`/creature/${editedCreature.id}`);
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
            Редактирование существа
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
                {state.xpLevels.map(xpLevel => (
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
              value={state.health}
              onChange={e => dispatch(actions.setHealth(Number(e.target.value)))}
            />
            <TextField
              margin="normal"
              required
              fullWidth
              id="movementSpeed"
              name="movementSpeed"
              label="Скорость передвижения"
              value={state.movementSpeed}
              onChange={e => dispatch(actions.setMovementSpeed(Number(e.target.value)))}
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
