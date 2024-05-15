import * as React from "react";
import CssBaseline from "@mui/material/CssBaseline";
import Grid from "@mui/material/Grid";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import Header from "../components/header";
import Footer from "../components/footer";
import { defaultTheme } from "../themes";
import { Button, Typography } from "@mui/material";
import { displayName, PlayerSession } from "../database";
import { Link, useNavigate, useParams } from "react-router-dom";
import { getPlayerSession } from "../requests";
import { useEffect, useState } from "react";
import NoDataTypography from "../components/no-data-typography";
import { toReadableTime, toReadableUtcDateTime, toYesNo } from "../value-format-helper";
import DeletePlayerSessionModal from "../components/deletion-modals/delete-player-session-modal";
import { canDelete, canEdit } from "../auth-state";
import GameSessionCard from "../components/cards/game-session-card";
import PlayerCard from "../components/cards/player-card";
import CreatureCard from "../components/cards/creature-card";

export default function SinglePlayerSession() {
  const navigate = useNavigate();
  const params = useParams();

  const [playerSession, setPlayerSession] = useState<PlayerSession>(new PlayerSession());

  const [deleteModalOpen, setDeleteModalOpen] = useState<boolean>(false);

  useEffect(() => {
    getPlayerSession(params.id)
      .then(playerSession => setPlayerSession(playerSession));
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
                {displayName(playerSession)}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>ID</b>: {playerSession.id}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Игровой сеанс</b>:
              </Typography>
              {playerSession.gameSession ? <GameSessionCard gameSession={playerSession.gameSession} /> : <NoDataTypography />}
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Игрок</b>:
              </Typography>
              {playerSession.player ? <PlayerCard player={playerSession.player} /> : <NoDataTypography />}
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Дата и время начала</b>: {toReadableUtcDateTime(playerSession.startTimestamp)}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Дата и время окончания</b>: {playerSession.endTimestamp ? toReadableUtcDateTime(playerSession.endTimestamp) : "-"}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Игра завершена</b>: {playerSession.isCompleted !== null ? toYesNo(playerSession.isCompleted) : "-"}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Победа</b>: {playerSession.isWon !== null ? toYesNo(playerSession.isWon) : "-"}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Время жизни</b>: {playerSession.timeAlive !== null ? toReadableTime(playerSession.timeAlive) : "-"}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Использовано существо</b>: {playerSession.playedAsCreature !== null ? toYesNo(playerSession.playedAsCreature) : "-"}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Использованное существо</b>:
              </Typography>
              {playerSession.usedCreature ? <CreatureCard creature={playerSession.usedCreature} /> : <NoDataTypography />}
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Количество самовозрождений</b>: {playerSession.selfReviveCount ?? "-"}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Количество возрождений союзников</b>: {playerSession.allyReviveCount ?? "-"}
              </Typography>
            </Grid>
          </Grid>
          <Grid container spacing={4} direction="row" justifyContent="start" alignItems="center" my={4} px={2}>
            {canEdit() &&
              <Button size="medium" variant="outlined" color="primary" sx={{ mx: 1 }} component={Link} to={`/editPlayerSession/${playerSession.id}`}>Редактировать</Button>
            }
            {canDelete() &&
              <Button size="medium" variant="outlined" color="error" sx={{ mx: 1 }} onClick={() => setDeleteModalOpen(true)}>Удалить</Button>
            }
            </Grid>
          <Grid container spacing={4} direction="row" justifyContent="center" alignItems="center" my={4} px={2}>
            <Button size="medium" variant="contained" color="primary" sx={{ mx: 1 }} component={Link} to={"/playerSessions"}>Вернуться к сеансам игроков</Button>
          </Grid>
        </main>
        <Footer />
      </Container>
      <DeletePlayerSessionModal open={deleteModalOpen} setOpen={setDeleteModalOpen} playerSession={playerSession} onDelete={() => navigate("/playerSessions")} />
    </ThemeProvider>
  );
}
