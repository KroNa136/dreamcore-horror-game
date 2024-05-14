import * as React from 'react';
import CssBaseline from '@mui/material/CssBaseline';
import Grid from '@mui/material/Grid';
import Container from '@mui/material/Container';
import { ThemeProvider } from '@mui/material/styles';
import Header from '../components/header';
import Footer from '../components/footer';
import { defaultTheme } from "../themes";
import { Button, Typography } from '@mui/material';
import { displayName, Player, XpLevel } from '../database';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { getPlayer } from '../requests';
import { useEffect, useState } from 'react';
import XpLevelCard from '../components/cards/xp-level-card';
import NoDataTypography from '../components/no-data-typography';
import { toReadableUtcDateTime, toYesNo } from '../value-format-helper';
import DeletePlayerModal from '../components/deletion-modals/delete-player-modal';
import { canDelete, canEdit } from '../auth-manager';

export default function SinglePlayer() {
  const navigate = useNavigate();
  const params = useParams();

  const [player, setPlayer] = useState<Player>(new Player());

  const [deleteModalOpen, setDeleteModalOpen] = useState<boolean>(false);

  useEffect(() => {
    getPlayer(params.id)
      .then(player => setPlayer(player));
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
                {displayName(player)}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>ID</b>: {player.id}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Никнейм</b>: {player.username}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Адрес электронной почты</b>: {player.email}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Дата и время регистрации</b>: {toReadableUtcDateTime(player.registrationTimestamp)}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Разрешение на сбор опциональных данных</b>: {toYesNo(player.collectOptionalData)}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>В сети</b>: {toYesNo(player.isOnline)}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Уровень опыта</b>:
              </Typography>
              {player.xpLevel && Object.keys(player.xpLevel).length > 0 ? <XpLevelCard xpLevel={player.xpLevel as XpLevel} /> : <NoDataTypography />}
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Опыт</b>: {player.xp}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Очки способностей</b>: {player.abilityPoints}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Очки духовной энергии</b>: {player.spiritEnergyPoints}
              </Typography>
            </Grid>
          </Grid>
          {canEdit() &&
            <Grid container spacing={4} direction="row" justifyContent="start" alignItems="center" my={4} px={2}>
              <Button size="medium" variant="outlined" color="primary" sx={{ mx: 1 }} component={Link} to={`/editPlayer/${player.id}`}>Редактировать</Button>
              {canDelete() &&
                <Button size="medium" variant="outlined" color="error" sx={{ mx: 1 }} onClick={() => setDeleteModalOpen(true)}>Удалить</Button>
              }
            </Grid>
          }
          <Grid container spacing={4} direction="row" justifyContent="center" alignItems="center" my={4} px={2}>
            <Button size="medium" variant="contained" color="primary" sx={{ mx: 1 }} component={Link} to={"/players"}>Вернуться к игрокам</Button>
          </Grid>
        </main>
        <Footer />
      </Container>
      <DeletePlayerModal open={deleteModalOpen} setOpen={setDeleteModalOpen} player={player} onDelete={() => navigate("/players")} />
    </ThemeProvider>
  );
}
