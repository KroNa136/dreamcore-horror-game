import * as React from "react";
import Typography from "@mui/material/Typography";
import Grid from "@mui/material/Grid";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import { Button, CardActions } from "@mui/material";
import { displayName, PlayerSession } from "../../database";
import { Link } from "react-router-dom";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import DeletePlayerSessionModal from "../deletion-modals/delete-player-session-modal";
import { canDelete, canEdit } from "../../auth-manager";

interface PlayerSessionCardProps {
  playerSession: PlayerSession
}

export default function PlayerSessionCard(props: PlayerSessionCardProps) {
  const navigate = useNavigate();

  const { playerSession } = props;
  const [deleteModalOpen, setDeleteModalOpen] = useState<boolean>(false);

  return (
    <Grid item xs={12} md={6}>
      <Card sx={{ backgroundColor: "#fbfbfb" }}>
        <CardContent>
          <Typography gutterBottom variant="h5" component="div">
            {displayName(playerSession)}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            ID: {playerSession.id}
          </Typography>
        </CardContent>
        <CardActions sx={{ display: "flex", justifyContent: "center" }}>
          <Button size="small" color="primary" variant="outlined" component={Link} to={`/playerSession/${playerSession.id}`}>Перейти</Button>
          {canEdit() &&
            <Button size="small" color="primary" variant="outlined" component={Link} to={`/editPlayerSession/${playerSession.id}`}>Редактировать</Button>
          }
          {canDelete() &&
            <Button size="small" color="error" variant="outlined" onClick={() => setDeleteModalOpen(true)}>Удалить</Button>
          }
        </CardActions>
      </Card>
      <DeletePlayerSessionModal open={deleteModalOpen} setOpen={setDeleteModalOpen} playerSession={playerSession} onDelete={() => navigate(0)} />
    </Grid>
  );
}
