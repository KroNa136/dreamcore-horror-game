import * as React from "react";
import Typography from "@mui/material/Typography";
import Grid from "@mui/material/Grid";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import { Button, CardActions } from "@mui/material";
import { displayName, GameMode } from "../../database";
import { Link } from "react-router-dom";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import DeleteGameModeModal from "../deletion-modals/delete-game-mode-modal";
import { canDelete, canEdit } from "../../auth-manager";

interface GameModeCardProps {
  gameMode: GameMode
}

export default function GameModeCard(props: GameModeCardProps) {
  const navigate = useNavigate();

  const { gameMode } = props;
  const [deleteModalOpen, setDeleteModalOpen] = useState<boolean>(false);

  return (
    <Grid item xs={12} md={6}>
      <Card sx={{ backgroundColor: "#fbfbfb" }}>
        <CardContent>
          <Typography gutterBottom variant="h5" component="div">
            {displayName(gameMode)}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            ID: {gameMode.id}
          </Typography>
        </CardContent>
        <CardActions sx={{ display: "flex", justifyContent: "center" }}>
          <Button size="small" color="primary" variant="outlined" component={Link} to={`/gameMode/${gameMode.id}`}>Перейти</Button>
          {canEdit() &&
            <Button size="small" color="primary" variant="outlined" component={Link} to={`/editGameMode/${gameMode.id}`}>Редактировать</Button>
          }
          {canDelete() &&
            <Button size="small" color="error" variant="outlined" onClick={() => setDeleteModalOpen(true)}>Удалить</Button>
          }
        </CardActions>
      </Card>
      <DeleteGameModeModal open={deleteModalOpen} setOpen={setDeleteModalOpen} gameMode={gameMode} onDelete={() => navigate(0)} />
    </Grid>
  );
}
