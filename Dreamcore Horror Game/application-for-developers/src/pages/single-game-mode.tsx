import * as React from "react";
import CssBaseline from "@mui/material/CssBaseline";
import Grid from "@mui/material/Grid";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import Header from "../components/header";
import Footer from "../components/footer";
import { defaultTheme } from "../themes";
import { Button, Typography } from "@mui/material";
import { displayName, GameMode } from "../database";
import { Link, useNavigate, useParams } from "react-router-dom";
import { getGameMode } from "../requests";
import { useEffect, useState } from "react";
import NoDataTypography from "../components/no-data-typography";
import { toReadableTime, toYesNo } from "../value-format-helper";
import DeleteGameModeModal from "../components/deletion-modals/delete-game-mode-modal";
import { canDelete, canEdit } from "../auth-manager";

export default function SingleGameMode() {
  const navigate = useNavigate();
  const params = useParams();

  const [gameMode, setGameMode] = useState<GameMode>(new GameMode());

  const [deleteModalOpen, setDeleteModalOpen] = useState<boolean>(false);

  useEffect(() => {
    getGameMode(params.id)
      .then(gameMode => setGameMode(gameMode));
  }, []);
  
  return (
    <ThemeProvider theme={defaultTheme}>
      <CssBaseline />
      <Container maxWidth="lg">
        <Header />
        <main>
          <Grid container spacing={1}>
            <Grid item xs={12} my={4}>
              <Typography component="h4" variant="h4" align="left">
                {displayName(gameMode)}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>ID</b>: {gameMode.id}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Название ассета</b>: {gameMode.assetName}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Максимальное число игроков</b>: {gameMode.maxPlayers ?? "-"}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Ограничение по времени</b>: {gameMode.timeLimit ? toReadableTime(gameMode.timeLimit) : "-"}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Активен</b>: {gameMode.isActive !== null ? toYesNo(gameMode.isActive) : <NoDataTypography />}
              </Typography>
            </Grid>
          </Grid>
          <Grid container spacing={4} direction="row" justifyContent="start" alignItems="center" my={4} px={2}>
            {canEdit() &&
              <Button size="medium" variant="outlined" color="primary" sx={{ mx: 1 }} component={Link} to={`/editGameMode/${gameMode.id}`}>Редактировать</Button>
            }
            {canDelete() &&
              <Button size="medium" variant="outlined" color="error" sx={{ mx: 1 }} onClick={() => setDeleteModalOpen(true)}>Удалить</Button>
            }
            </Grid>
          <Grid container spacing={4} direction="row" justifyContent="center" alignItems="center" my={4} px={2}>
            <Button size="medium" variant="contained" color="primary" sx={{ mx: 1 }} component={Link} to={"/gameModes"}>Вернуться к игровым режимам</Button>
          </Grid>
        </main>
        <Footer />
      </Container>
      <DeleteGameModeModal open={deleteModalOpen} setOpen={setDeleteModalOpen} gameMode={gameMode} onDelete={() => navigate("/gameModes")} />
    </ThemeProvider>
  );
}
