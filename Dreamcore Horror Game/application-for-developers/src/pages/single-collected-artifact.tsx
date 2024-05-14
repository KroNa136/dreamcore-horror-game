import * as React from "react";
import CssBaseline from "@mui/material/CssBaseline";
import Grid from "@mui/material/Grid";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import Header from "../components/header";
import Footer from "../components/footer";
import { defaultTheme } from "../themes";
import { Button, Typography } from "@mui/material";
import { displayName, CollectedArtifact } from "../database";
import { Link, useNavigate, useParams } from "react-router-dom";
import { getCollectedArtifact } from "../requests";
import { useEffect, useState } from "react";
import NoDataTypography from "../components/no-data-typography";
import { toReadableUtcDateTime } from "../value-format-helper";
import DeleteCollectedArtifactModal from "../components/deletion-modals/delete-collected-artifact-modal";
import { canDelete, canEdit } from "../auth-manager";
import PlayerCard from "../components/cards/player-card";
import ArtifactCard from "../components/cards/artifact-card";

export default function SingleCollectedArtifact() {
  const navigate = useNavigate();
  const params = useParams();

  const [collectedArtifact, setCollectedArtifact] = useState<CollectedArtifact>(new CollectedArtifact());

  const [deleteModalOpen, setDeleteModalOpen] = useState<boolean>(false);

  useEffect(() => {
    getCollectedArtifact(params.id)
      .then(collectedArtifact => setCollectedArtifact(collectedArtifact));
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
                {displayName(collectedArtifact)}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>ID</b>: {collectedArtifact.id}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Игрок</b>:
              </Typography>
              {collectedArtifact.player ? <PlayerCard player={collectedArtifact.player} /> : <NoDataTypography />}
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Артефакт</b>:
              </Typography>
              {collectedArtifact.artifact ? <ArtifactCard artifact={collectedArtifact.artifact} /> : <NoDataTypography />}
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Дата и время подбора</b>: {toReadableUtcDateTime(collectedArtifact.collectionTimestamp)}
              </Typography>
            </Grid>
          </Grid>
          <Grid container spacing={4} direction="row" justifyContent="start" alignItems="center" my={4} px={2}>
            {canEdit() &&
              <Button size="medium" variant="outlined" color="primary" sx={{ mx: 1 }} component={Link} to={`/editCollectedArtifact/${collectedArtifact.id}`}>Редактировать</Button>
            }
            {canDelete() &&
              <Button size="medium" variant="outlined" color="error" sx={{ mx: 1 }} onClick={() => setDeleteModalOpen(true)}>Удалить</Button>
            }
            </Grid>
          <Grid container spacing={4} direction="row" justifyContent="center" alignItems="center" my={4} px={2}>
            <Button size="medium" variant="contained" color="primary" sx={{ mx: 1 }} component={Link} to={"/collectedArtifacts"}>Вернуться к подобранным артефактам</Button>
          </Grid>
        </main>
        <Footer />
      </Container>
      <DeleteCollectedArtifactModal open={deleteModalOpen} setOpen={setDeleteModalOpen} collectedArtifact={collectedArtifact} onDelete={() => navigate("/collectedArtifacts")} />
    </ThemeProvider>
  );
}
