import * as React from "react";
import Typography from "@mui/material/Typography";
import Grid from "@mui/material/Grid";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import { Button, CardActions } from "@mui/material";
import { displayName, Ability } from "../../database";
import { Link } from "react-router-dom";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import DeleteAbilityModal from "../deletion-modals/delete-ability-modal";
import { canDelete, canEdit } from "../../auth-state";

interface AbilityCardProps {
  ability: Ability
}

export default function AbilityCard(props: AbilityCardProps) {
  const navigate = useNavigate();

  const { ability } = props;
  const [deleteModalOpen, setDeleteModalOpen] = useState<boolean>(false);

  return (
    <Grid item xs={12} md={6}>
      <Card sx={{ backgroundColor: "#fbfbfb" }}>
        <CardContent>
          <Typography gutterBottom variant="h5" component="div">
            {displayName(ability)}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            ID: {ability.id}
          </Typography>
        </CardContent>
        <CardActions sx={{ display: "flex", justifyContent: "center" }}>
          <Button size="small" color="primary" variant="outlined" component={Link} to={`/ability/${ability.id}`}>Перейти</Button>
          {canEdit() &&
            <Button size="small" color="primary" variant="outlined" component={Link} to={`/editAbility/${ability.id}`}>Редактировать</Button>
          }
          {canDelete() &&
            <Button size="small" color="error" variant="outlined" onClick={() => setDeleteModalOpen(true)}>Удалить</Button>
          }
        </CardActions>
      </Card>
      <DeleteAbilityModal open={deleteModalOpen} setOpen={setDeleteModalOpen} ability={ability} onDelete={() => navigate(0)} />
    </Grid>
  );
}
