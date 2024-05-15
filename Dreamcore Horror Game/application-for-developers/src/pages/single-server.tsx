import * as React from "react";
import CssBaseline from "@mui/material/CssBaseline";
import Grid from "@mui/material/Grid";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import Header from "../components/header";
import Footer from "../components/footer";
import { defaultTheme } from "../themes";
import { Button, Typography } from "@mui/material";
import { displayName, Server } from "../database";
import { Link, useNavigate, useParams } from "react-router-dom";
import { getServer } from "../requests";
import { useEffect, useState } from "react";
import { toYesNo } from "../value-format-helper";
import DeleteServerModal from "../components/deletion-modals/delete-server-modal";
import { canDelete, canEdit } from "../auth-state";

export default function SingleServer() {
  const navigate = useNavigate();
  const params = useParams();

  const [server, setServer] = useState<Server>(new Server());

  const [deleteModalOpen, setDeleteModalOpen] = useState<boolean>(false);

  useEffect(() => {
    getServer(params.id)
      .then(server => setServer(server));
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
                {displayName(server)}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>ID</b>: {server.id}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>IP-адрес</b>: {server.ipAddress}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>Вместимость</b>: {server.playerCapacity}
              </Typography>
            </Grid>
            <Grid item xs={12}>
              <Typography component="p" variant="body1" align="left">
                <b>В сети</b>: {toYesNo(server.isOnline)}
              </Typography>
            </Grid>
          </Grid>
          <Grid container spacing={4} direction="row" justifyContent="start" alignItems="center" my={4} px={2}>
            {canEdit() &&
              <Button size="medium" variant="outlined" color="primary" sx={{ mx: 1 }} component={Link} to={`/editServer/${server.id}`}>Редактировать</Button>
            }
            {canDelete() &&
              <Button size="medium" variant="outlined" color="error" sx={{ mx: 1 }} onClick={() => setDeleteModalOpen(true)}>Удалить</Button>
            }
            </Grid>
          <Grid container spacing={4} direction="row" justifyContent="center" alignItems="center" my={4} px={2}>
            <Button size="medium" variant="contained" color="primary" sx={{ mx: 1 }} component={Link} to={"/servers"}>Вернуться к серверам</Button>
          </Grid>
        </main>
        <Footer />
      </Container>
      <DeleteServerModal open={deleteModalOpen} setOpen={setDeleteModalOpen} server={server} onDelete={() => navigate("/servers")} />
    </ThemeProvider>
  );
}
