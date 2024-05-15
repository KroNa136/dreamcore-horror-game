import * as React from "react";
import Typography from "@mui/material/Typography";
import Grid from "@mui/material/Grid";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import { Button, CardActions } from "@mui/material";
import { displayName, Creature } from "../../database";
import { Link } from "react-router-dom";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import DeleteCreatureModal from "../deletion-modals/delete-creature-modal";
import { canDelete, canEdit } from "../../auth-state";

interface CreatureCardProps {
  creature: Creature
}

export default function CreatureCard(props: CreatureCardProps) {
  const navigate = useNavigate();

  const { creature } = props;
  const [deleteModalOpen, setDeleteModalOpen] = useState<boolean>(false);

  return (
    <Grid item xs={12} md={6}>
      <Card sx={{ backgroundColor: "#fbfbfb" }}>
        <CardContent>
          <Typography gutterBottom variant="h5" component="div">
            {displayName(creature)}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            ID: {creature.id}
          </Typography>
        </CardContent>
        <CardActions sx={{ display: "flex", justifyContent: "center" }}>
          <Button size="small" color="primary" variant="outlined" component={Link} to={`/creature/${creature.id}`}>Перейти</Button>
          {canEdit() &&
            <Button size="small" color="primary" variant="outlined" component={Link} to={`/editCreature/${creature.id}`}>Редактировать</Button>
          }
          {canDelete() &&
            <Button size="small" color="error" variant="outlined" onClick={() => setDeleteModalOpen(true)}>Удалить</Button>
          }
        </CardActions>
      </Card>
      <DeleteCreatureModal open={deleteModalOpen} setOpen={setDeleteModalOpen} creature={creature} onDelete={() => navigate(0)} />
    </Grid>
  );
}
