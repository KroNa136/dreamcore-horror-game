import * as React from "react";
import Typography from "@mui/material/Typography";
import Grid from "@mui/material/Grid";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import { Button, CardActions } from "@mui/material";
import { displayName, GameSession } from "../../database";
import { Link } from "react-router-dom";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import DeleteGameSessionModal from "../deletion-modals/delete-game-session-modal";
import { canDelete, canEdit } from "../../auth-manager";

interface GameSessionCardProps {
  gameSession: GameSession
}

export default function GameSessionCard(props: GameSessionCardProps) {
  const navigate = useNavigate();

  const { gameSession } = props;
  const [deleteModalOpen, setDeleteModalOpen] = useState<boolean>(false);

  return (
    <Grid item xs={12} md={6}>
      <Card sx={{ backgroundColor: "#fbfbfb" }}>
        <CardContent>
          <Typography gutterBottom variant="h5" component="div">
            {displayName(gameSession)}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            ID: {gameSession.id}
          </Typography>
        </CardContent>
        <CardActions sx={{ display: "flex", justifyContent: "center" }}>
          <Button size="small" color="primary" variant="outlined" component={Link} to={`/gameSession/${gameSession.id}`}>Перейти</Button>
          {canEdit() &&
            <Button size="small" color="primary" variant="outlined" component={Link} to={`/editGameSession/${gameSession.id}`}>Редактировать</Button>
          }
          {canDelete() &&
            <Button size="small" color="error" variant="outlined" onClick={() => setDeleteModalOpen(true)}>Удалить</Button>
          }
        </CardActions>
      </Card>
      <DeleteGameSessionModal open={deleteModalOpen} setOpen={setDeleteModalOpen} gameSession={gameSession} onDelete={() => navigate(0)} />
    </Grid>
  );
}
