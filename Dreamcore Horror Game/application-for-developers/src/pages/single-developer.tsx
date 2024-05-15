import * as React from "react";
import CssBaseline from "@mui/material/CssBaseline";
import Grid from "@mui/material/Grid";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import Header from "../components/header";
import Footer from "../components/footer";
import { defaultTheme } from "../themes";
import { Button, Typography } from "@mui/material";
import { displayName, Developer } from "../database";
import { Link, useNavigate, useParams } from "react-router-dom";
import { getDeveloper } from "../requests";
import { useEffect, useState } from "react";
import NoDataTypography from "../components/no-data-typography";
import { toYesNo } from "../value-format-helper";
import DeleteDeveloperModal from "../components/deletion-modals/delete-developer-modal";
import { canDelete, canEdit } from "../auth-state";
import DeveloperAccessLevelCard from "../components/cards/developer-access-level-card";

export default function SingleDeveloper() {
  const navigate = useNavigate();
  const params = useParams();

  const [developer, setDeveloper] = useState<Developer>(new Developer());

  const [deleteModalOpen, setDeleteModalOpen] = useState<boolean>(false);

  useEffect(() => {
    getDeveloper(params.id)
      .then(developer => setDeveloper(developer));
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
                {displayName(developer)}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>ID</b>: {developer.id}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Логин</b>: {developer.login}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Уровень доступа</b>:
              </Typography>
              {developer.developerAccessLevel ? <DeveloperAccessLevelCard developerAccessLevel={developer.developerAccessLevel} /> : <NoDataTypography />}
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>В сети</b>: {toYesNo(developer.isOnline)}
              </Typography>
            </Grid>
          </Grid>
          <Grid container spacing={4} direction="row" justifyContent="start" alignItems="center" my={4} px={2}>
            {canEdit() &&
              <Button size="medium" variant="outlined" color="primary" sx={{ mx: 1 }} component={Link} to={`/editDeveloper/${developer.id}`}>Редактировать</Button>
            }
            {canDelete() &&
              <Button size="medium" variant="outlined" color="error" sx={{ mx: 1 }} onClick={() => setDeleteModalOpen(true)}>Удалить</Button>
            }
            </Grid>
          <Grid container spacing={4} direction="row" justifyContent="center" alignItems="center" my={4} px={2}>
            <Button size="medium" variant="contained" color="primary" sx={{ mx: 1 }} component={Link} to={"/developers"}>Вернуться к разработчикам</Button>
          </Grid>
        </main>
        <Footer />
      </Container>
      <DeleteDeveloperModal open={deleteModalOpen} setOpen={setDeleteModalOpen} developer={developer} onDelete={() => navigate("/developers")} />
    </ThemeProvider>
  );
}
