import * as React from "react";
import Typography from "@mui/material/Typography";
import Grid from "@mui/material/Grid";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import { Button, CardActions } from "@mui/material";
import { displayName, Player } from "../../database";
import { Link } from "react-router-dom";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import DeletePlayerModal from "../deletion-modals/delete-player-modal";
import { canDelete, canEdit } from "../../auth-manager";

interface PlayerCardProps {
  player: Player
}

export default function PlayerCard(props: PlayerCardProps) {
  const navigate = useNavigate();

  const { player } = props;
  const [deleteModalOpen, setDeleteModalOpen] = useState<boolean>(false);

  return (
    <Grid item xs={12} md={6}>
      <Card sx={{ backgroundColor: "#fbfbfb" }}>
        <CardContent>
          <Typography gutterBottom variant="h5" component="div">
            {displayName(player)}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            ID: {player.id}
          </Typography>
        </CardContent>
        <CardActions sx={{ display: "flex", justifyContent: "center" }}>
          <Button size="small" color="primary" variant="outlined" component={Link} to={`/player/${player.id}`}>Перейти</Button>
          {canEdit() &&
            <Button size="small" color="primary" variant="outlined" component={Link} to={`/editPlayer/${player.id}`}>Редактировать</Button>
          }
          {canDelete() &&
            <Button size="small" color="error" variant="outlined" onClick={() => setDeleteModalOpen(true)}>Удалить</Button>
          }
        </CardActions>
      </Card>
      <DeletePlayerModal open={deleteModalOpen} setOpen={setDeleteModalOpen} player={player} onDelete={() => navigate(0)} />
    </Grid>
  );
}
