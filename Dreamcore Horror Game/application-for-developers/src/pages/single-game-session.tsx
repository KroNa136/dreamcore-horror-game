import * as React from "react";
import CssBaseline from "@mui/material/CssBaseline";
import Grid from "@mui/material/Grid";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import Header from "../components/header";
import Footer from "../components/footer";
import { defaultTheme } from "../themes";
import { Button, Typography } from "@mui/material";
import { displayName, GameSession } from "../database";
import { Link, useNavigate, useParams } from "react-router-dom";
import { getGameSession } from "../requests";
import { useEffect, useState } from "react";
import NoDataTypography from "../components/no-data-typography";
import { toReadableUtcDateTime } from "../value-format-helper";
import DeleteGameSessionModal from "../components/deletion-modals/delete-game-session-modal";
import { canDelete, canEdit } from "../auth-manager";
import ServerCard from "../components/cards/server-card";
import GameModeCard from "../components/cards/game-mode-card";

export default function SingleGameSession() {
  const navigate = useNavigate();
  const params = useParams();

  const [gameSession, setGameSession] = useState<GameSession>(new GameSession());

  const [deleteModalOpen, setDeleteModalOpen] = useState<boolean>(false);

  useEffect(() => {
    getGameSession(params.id)
      .then(gameSession => setGameSession(gameSession));
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
                {displayName(gameSession)}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>ID</b>: {gameSession.id}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Сервер</b>:
              </Typography>
              {gameSession.server ? <ServerCard server={gameSession.server} /> : <NoDataTypography />}
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Игровой режим</b>:
              </Typography>
              {gameSession.gameMode ? <GameModeCard gameMode={gameSession.gameMode} /> : <NoDataTypography />}
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Дата и время начала</b>: {toReadableUtcDateTime(gameSession.startTimestamp)}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Дата и время окончания</b>: {gameSession.endTimestamp ? toReadableUtcDateTime(gameSession.endTimestamp) : "-"}
              </Typography>
            </Grid>
          </Grid>
          <Grid container spacing={4} direction="row" justifyContent="start" alignItems="center" my={4} px={2}>
            {canEdit() &&
              <Button size="medium" variant="outlined" color="primary" sx={{ mx: 1 }} component={Link} to={`/editGameSession/${gameSession.id}`}>Редактировать</Button>
            }
            {canDelete() &&
              <Button size="medium" variant="outlined" color="error" sx={{ mx: 1 }} onClick={() => setDeleteModalOpen(true)}>Удалить</Button>
            }
            </Grid>
          <Grid container spacing={4} direction="row" justifyContent="center" alignItems="center" my={4} px={2}>
            <Button size="medium" variant="contained" color="primary" sx={{ mx: 1 }} component={Link} to={"/gameSessions"}>Вернуться к игровым сеансам</Button>
          </Grid>
        </main>
        <Footer />
      </Container>
      <DeleteGameSessionModal open={deleteModalOpen} setOpen={setDeleteModalOpen} gameSession={gameSession} onDelete={() => navigate("/gameSessions")} />
    </ThemeProvider>
  );
}
