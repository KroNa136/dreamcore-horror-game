import * as React from "react";
import CssBaseline from "@mui/material/CssBaseline";
import Grid from "@mui/material/Grid";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import Header from "../components/header";
import Footer from "../components/footer";
import { defaultTheme } from "../themes";
import { Button, Typography } from "@mui/material";
import { displayName, XpLevel } from "../database";
import { Link, useNavigate, useParams } from "react-router-dom";
import { getXpLevel } from "../requests";
import { useEffect, useState } from "react";
import DeleteXpLevelModal from "../components/deletion-modals/delete-xp-level-modal";
import { canDelete, canEdit } from "../auth-manager";

export default function SingleXpLevel() {
  const navigate = useNavigate();
  const params = useParams();

  const [xpLevel, setXpLevel] = useState<XpLevel>(new XpLevel());

  const [deleteModalOpen, setDeleteModalOpen] = useState<boolean>(false);

  useEffect(() => {
    getXpLevel(params.id)
      .then(xpLevel => setXpLevel(xpLevel));
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
                {displayName(xpLevel)}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>ID</b>: {xpLevel.id}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Номер</b>: {xpLevel.number}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Требуемый опыт</b>: {xpLevel.requiredXp}
              </Typography>
            </Grid>
          </Grid>
          <Grid container spacing={4} direction="row" justifyContent="start" alignItems="center" my={4} px={2}>
            {canEdit() &&
              <Button size="medium" variant="outlined" color="primary" sx={{ mx: 1 }} component={Link} to={`/editXpLevel/${xpLevel.id}`}>Редактировать</Button>
            }
            {canDelete() &&
              <Button size="medium" variant="outlined" color="error" sx={{ mx: 1 }} onClick={() => setDeleteModalOpen(true)}>Удалить</Button>
            }
            </Grid>
          <Grid container spacing={4} direction="row" justifyContent="center" alignItems="center" my={4} px={2}>
            <Button size="medium" variant="contained" color="primary" sx={{ mx: 1 }} component={Link} to={"/xpLevels"}>Вернуться к уровням опыта</Button>
          </Grid>
        </main>
        <Footer />
      </Container>
      <DeleteXpLevelModal open={deleteModalOpen} setOpen={setDeleteModalOpen} xpLevel={xpLevel} onDelete={() => navigate("/xpLevels")} />
    </ThemeProvider>
  );
}
