import * as React from "react";
import CssBaseline from "@mui/material/CssBaseline";
import Grid from "@mui/material/Grid";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import Header from "../components/header";
import Footer from "../components/footer";
import { defaultTheme } from "../themes";
import { Button, Typography } from "@mui/material";
import { displayName, Artifact } from "../database";
import { Link, useNavigate, useParams } from "react-router-dom";
import { getArtifact } from "../requests";
import { useEffect, useState } from "react";
import NoDataTypography from "../components/no-data-typography";
import DeleteArtifactModal from "../components/deletion-modals/delete-artifact-modal";
import { canDelete, canEdit } from "../auth-state";
import RarityLevelCard from "../components/cards/rarity-level-card";

export default function SingleArtifact() {
  const navigate = useNavigate();
  const params = useParams();

  const [artifact, setArtifact] = useState<Artifact>(new Artifact());

  const [deleteModalOpen, setDeleteModalOpen] = useState<boolean>(false);

  useEffect(() => {
    getArtifact(params.id)
      .then(artifact => setArtifact(artifact));
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
                {displayName(artifact)}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>ID</b>: {artifact.id}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Название ассета</b>: {artifact.assetName}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Уровень редкости</b>:
              </Typography>
              {artifact.rarityLevel ? <RarityLevelCard rarityLevel={artifact.rarityLevel} /> : <NoDataTypography />}
            </Grid>
          </Grid>
          <Grid container spacing={4} direction="row" justifyContent="start" alignItems="center" my={4} px={2}>
            {canEdit() &&
              <Button size="medium" variant="outlined" color="primary" sx={{ mx: 1 }} component={Link} to={`/editArtifact/${artifact.id}`}>Редактировать</Button>
            }
            {canDelete() &&
              <Button size="medium" variant="outlined" color="error" sx={{ mx: 1 }} onClick={() => setDeleteModalOpen(true)}>Удалить</Button>
            }
            </Grid>
          <Grid container spacing={4} direction="row" justifyContent="center" alignItems="center" my={4} px={2}>
            <Button size="medium" variant="contained" color="primary" sx={{ mx: 1 }} component={Link} to={"/artifacts"}>Вернуться к артефактам</Button>
          </Grid>
        </main>
        <Footer />
      </Container>
      <DeleteArtifactModal open={deleteModalOpen} setOpen={setDeleteModalOpen} artifact={artifact} onDelete={() => navigate("/artifacts")} />
    </ThemeProvider>
  );
}
