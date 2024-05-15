import * as React from "react";
import CssBaseline from "@mui/material/CssBaseline";
import Grid from "@mui/material/Grid";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import Header from "../components/header";
import Footer from "../components/footer";
import { defaultTheme } from "../themes";
import { Button, Typography } from "@mui/material";
import { displayName, DeveloperAccessLevel } from "../database";
import { Link, useNavigate, useParams } from "react-router-dom";
import { getDeveloperAccessLevel } from "../requests";
import { useEffect, useState } from "react";
import DeleteDeveloperAccessLevelModal from "../components/deletion-modals/delete-developer-access-level-modal";
import { canDelete, canEdit } from "../auth-state";

export default function SingleDeveloperAccessLevel() {
  const navigate = useNavigate();
  const params = useParams();

  const [developerAccessLevel, setDeveloperAccessLevel] = useState<DeveloperAccessLevel>(new DeveloperAccessLevel());

  const [deleteModalOpen, setDeleteModalOpen] = useState<boolean>(false);

  useEffect(() => {
    getDeveloperAccessLevel(params.id)
      .then(developerAccessLevel => setDeveloperAccessLevel(developerAccessLevel));
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
                {displayName(developerAccessLevel)}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>ID</b>: {developerAccessLevel.id}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Название</b>: {developerAccessLevel.name}
              </Typography>
            </Grid>
          </Grid>
          <Grid container spacing={4} direction="row" justifyContent="start" alignItems="center" my={4} px={2}>
            {canEdit() &&
              <Button size="medium" variant="outlined" color="primary" sx={{ mx: 1 }} component={Link} to={`/editDeveloperAccessLevel/${developerAccessLevel.id}`}>Редактировать</Button>
            }
            {canDelete() &&
              <Button size="medium" variant="outlined" color="error" sx={{ mx: 1 }} onClick={() => setDeleteModalOpen(true)}>Удалить</Button>
            }
            </Grid>
          <Grid container spacing={4} direction="row" justifyContent="center" alignItems="center" my={4} px={2}>
            <Button size="medium" variant="contained" color="primary" sx={{ mx: 1 }} component={Link} to={"/developerAccessLevels"}>Вернуться к уровням доступа разработчиков</Button>
          </Grid>
        </main>
        <Footer />
      </Container>
      <DeleteDeveloperAccessLevelModal open={deleteModalOpen} setOpen={setDeleteModalOpen} developerAccessLevel={developerAccessLevel} onDelete={() => navigate("/developerAccessLevels")} />
    </ThemeProvider>
  );
}
