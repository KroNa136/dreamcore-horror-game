import * as React from "react";
import CssBaseline from "@mui/material/CssBaseline";
import Grid from "@mui/material/Grid";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import Header from "../components/header";
import Footer from "../components/footer";
import { defaultTheme } from "../themes";
import { Button, Typography } from "@mui/material";
import { displayName, Creature } from "../database";
import { Link, useNavigate, useParams } from "react-router-dom";
import { getCreature } from "../requests";
import { useEffect, useState } from "react";
import XpLevelCard from "../components/cards/xp-level-card";
import NoDataTypography from "../components/no-data-typography";
import DeleteCreatureModal from "../components/deletion-modals/delete-creature-modal";
import { canDelete, canEdit } from "../auth-manager";

export default function SingleCreature() {
  const navigate = useNavigate();
  const params = useParams();

  const [creature, setCreature] = useState<Creature>(new Creature());

  const [deleteModalOpen, setDeleteModalOpen] = useState<boolean>(false);

  useEffect(() => {
    getCreature(params.id)
      .then(creature => setCreature(creature));
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
                {displayName(creature)}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>ID</b>: {creature.id}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Название ассета</b>: {creature.assetName}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Требуемый уровень опыта</b>:
              </Typography>
              {creature.requiredXpLevel ? <XpLevelCard xpLevel={creature.requiredXpLevel} /> : <NoDataTypography />}
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Здоровье</b>: {creature.health}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Скорость передвижения</b>: {creature.movementSpeed}
              </Typography>
            </Grid>
          </Grid>
          <Grid container spacing={4} direction="row" justifyContent="start" alignItems="center" my={4} px={2}>
            {canEdit() &&
              <Button size="medium" variant="outlined" color="primary" sx={{ mx: 1 }} component={Link} to={`/editCreature/${creature.id}`}>Редактировать</Button>
            }
            {canDelete() &&
              <Button size="medium" variant="outlined" color="error" sx={{ mx: 1 }} onClick={() => setDeleteModalOpen(true)}>Удалить</Button>
            }
            </Grid>
          <Grid container spacing={4} direction="row" justifyContent="center" alignItems="center" my={4} px={2}>
            <Button size="medium" variant="contained" color="primary" sx={{ mx: 1 }} component={Link} to={"/creatures"}>Вернуться к существам</Button>
          </Grid>
        </main>
        <Footer />
      </Container>
      <DeleteCreatureModal open={deleteModalOpen} setOpen={setDeleteModalOpen} creature={creature} onDelete={() => navigate("/creatures")} />
    </ThemeProvider>
  );
}
