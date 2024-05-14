import * as React from "react";
import CssBaseline from "@mui/material/CssBaseline";
import Grid from "@mui/material/Grid";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import Header from "../components/header";
import Footer from "../components/footer";
import { defaultTheme } from "../themes";
import { Button, Typography } from "@mui/material";
import { displayName, Ability } from "../database";
import { Link, useNavigate, useParams } from "react-router-dom";
import { getAbility } from "../requests";
import { useEffect, useState } from "react";
import DeleteAbilityModal from "../components/deletion-modals/delete-ability-modal";
import { canDelete, canEdit } from "../auth-manager";

export default function SingleAbility() {
  const navigate = useNavigate();
  const params = useParams();

  const [ability, setAbility] = useState<Ability>(new Ability());

  const [deleteModalOpen, setDeleteModalOpen] = useState<boolean>(false);

  useEffect(() => {
    getAbility(params.id)
      .then(ability => setAbility(ability));
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
                {displayName(ability)}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>ID</b>: {ability.id}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Название ассета</b>: {ability.assetName}
              </Typography>
            </Grid>
          </Grid>
          <Grid container spacing={4} direction="row" justifyContent="start" alignItems="center" my={4} px={2}>
            {canEdit() &&
              <Button size="medium" variant="outlined" color="primary" sx={{ mx: 1 }} component={Link} to={`/editAbility/${ability.id}`}>Редактировать</Button>
            }
            {canDelete() &&
              <Button size="medium" variant="outlined" color="error" sx={{ mx: 1 }} onClick={() => setDeleteModalOpen(true)}>Удалить</Button>
            }
            </Grid>
          <Grid container spacing={4} direction="row" justifyContent="center" alignItems="center" my={4} px={2}>
            <Button size="medium" variant="contained" color="primary" sx={{ mx: 1 }} component={Link} to={"/abilities"}>Вернуться к способностям</Button>
          </Grid>
        </main>
        <Footer />
      </Container>
      <DeleteAbilityModal open={deleteModalOpen} setOpen={setDeleteModalOpen} ability={ability} onDelete={() => navigate("/abilities")} />
    </ThemeProvider>
  );
}
