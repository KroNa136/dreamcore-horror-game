import * as React from "react";
import CssBaseline from "@mui/material/CssBaseline";
import Grid from "@mui/material/Grid";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import Header from "../components/header";
import Footer from "../components/footer";
import { defaultTheme } from "../themes";
import { Button, Typography } from "@mui/material";
import { displayName, AcquiredAbility } from "../database";
import { Link, useNavigate, useParams } from "react-router-dom";
import { getAcquiredAbility } from "../requests";
import { useEffect, useState } from "react";
import NoDataTypography from "../components/no-data-typography";
import { toReadableUtcDateTime } from "../value-format-helper";
import DeleteAcquiredAbilityModal from "../components/deletion-modals/delete-acquired-ability-modal";
import { canDelete, canEdit } from "../auth-manager";
import PlayerCard from "../components/cards/player-card";
import AbilityCard from "../components/cards/ability-card";

export default function SingleAcquiredAbility() {
  const navigate = useNavigate();
  const params = useParams();

  const [acquiredAbility, setAcquiredAbility] = useState<AcquiredAbility>(new AcquiredAbility());

  const [deleteModalOpen, setDeleteModalOpen] = useState<boolean>(false);

  useEffect(() => {
    getAcquiredAbility(params.id)
      .then(acquiredAbility => setAcquiredAbility(acquiredAbility));
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
                {displayName(acquiredAbility)}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>ID</b>: {acquiredAbility.id}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Игрок</b>:
              </Typography>
              {acquiredAbility.player ? <PlayerCard player={acquiredAbility.player} /> : <NoDataTypography />}
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Способность</b>:
              </Typography>
              {acquiredAbility.ability ? <AbilityCard ability={acquiredAbility.ability} /> : <NoDataTypography />}
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Дата и время приобретения</b>: {toReadableUtcDateTime(acquiredAbility.acquirementTimestamp)}
              </Typography>
            </Grid>
          </Grid>
          <Grid container spacing={4} direction="row" justifyContent="start" alignItems="center" my={4} px={2}>
            {canEdit() &&
              <Button size="medium" variant="outlined" color="primary" sx={{ mx: 1 }} component={Link} to={`/editAcquiredAbility/${acquiredAbility.id}`}>Редактировать</Button>
            }
            {canDelete() &&
              <Button size="medium" variant="outlined" color="error" sx={{ mx: 1 }} onClick={() => setDeleteModalOpen(true)}>Удалить</Button>
            }
            </Grid>
          <Grid container spacing={4} direction="row" justifyContent="center" alignItems="center" my={4} px={2}>
            <Button size="medium" variant="contained" color="primary" sx={{ mx: 1 }} component={Link} to={"/acquiredAbilities"}>Вернуться к приобретённым способностям</Button>
          </Grid>
        </main>
        <Footer />
      </Container>
      <DeleteAcquiredAbilityModal open={deleteModalOpen} setOpen={setDeleteModalOpen} acquiredAbility={acquiredAbility} onDelete={() => navigate("/acquiredAbilities")} />
    </ThemeProvider>
  );
}
