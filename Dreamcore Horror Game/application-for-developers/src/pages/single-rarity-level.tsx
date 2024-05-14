import * as React from "react";
import CssBaseline from "@mui/material/CssBaseline";
import Grid from "@mui/material/Grid";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import Header from "../components/header";
import Footer from "../components/footer";
import { defaultTheme } from "../themes";
import { Button, Typography } from "@mui/material";
import { displayName, RarityLevel } from "../database";
import { Link, useNavigate, useParams } from "react-router-dom";
import { getRarityLevel } from "../requests";
import { useEffect, useState } from "react";
import DeleteRarityLevelModal from "../components/deletion-modals/delete-rarity-level-modal";
import { canDelete, canEdit } from "../auth-manager";

export default function SingleRarityLevel() {
  const navigate = useNavigate();
  const params = useParams();

  const [rarityLevel, setRarityLevel] = useState<RarityLevel>(new RarityLevel());

  const [deleteModalOpen, setDeleteModalOpen] = useState<boolean>(false);

  useEffect(() => {
    getRarityLevel(params.id)
      .then(rarityLevel => setRarityLevel(rarityLevel));
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
                {displayName(rarityLevel)}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>ID</b>: {rarityLevel.id}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Название ассета</b>: {rarityLevel.assetName}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Вероятность</b>: {rarityLevel.probability}
              </Typography>
            </Grid>
          </Grid>
          <Grid container spacing={4} direction="row" justifyContent="start" alignItems="center" my={4} px={2}>
            {canEdit() &&
              <Button size="medium" variant="outlined" color="primary" sx={{ mx: 1 }} component={Link} to={`/editRarityLevel/${rarityLevel.id}`}>Редактировать</Button>
            }
            {canDelete() &&
              <Button size="medium" variant="outlined" color="error" sx={{ mx: 1 }} onClick={() => setDeleteModalOpen(true)}>Удалить</Button>
            }
            </Grid>
          <Grid container spacing={4} direction="row" justifyContent="center" alignItems="center" my={4} px={2}>
            <Button size="medium" variant="contained" color="primary" sx={{ mx: 1 }} component={Link} to={"/rarityLevels"}>Вернуться к уровням редкости</Button>
          </Grid>
        </main>
        <Footer />
      </Container>
      <DeleteRarityLevelModal open={deleteModalOpen} setOpen={setDeleteModalOpen} rarityLevel={rarityLevel} onDelete={() => navigate("/rarityLevels")} />
    </ThemeProvider>
  );
}
