import * as React from "react";
import Typography from "@mui/material/Typography";
import Grid from "@mui/material/Grid";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import { Button, CardActions } from "@mui/material";
import { displayName, AcquiredAbility } from "../../database";
import { Link } from "react-router-dom";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import DeleteAcquiredAbilityModal from "../deletion-modals/delete-acquired-ability-modal";
import { canDelete, canEdit } from "../../auth-manager";

interface AcquiredAbilityCardProps {
  acquiredAbility: AcquiredAbility
}

export default function AcquiredAbilityCard(props: AcquiredAbilityCardProps) {
  const navigate = useNavigate();

  const { acquiredAbility } = props;
  const [deleteModalOpen, setDeleteModalOpen] = useState<boolean>(false);

  return (
    <Grid item xs={12} md={6}>
      <Card sx={{ backgroundColor: "#fbfbfb" }}>
        <CardContent>
          <Typography gutterBottom variant="h5" component="div">
            {displayName(acquiredAbility)}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            ID: {acquiredAbility.id}
          </Typography>
        </CardContent>
        <CardActions sx={{ display: "flex", justifyContent: "center" }}>
          <Button size="small" color="primary" variant="outlined" component={Link} to={`/acquiredAcquiredAbility/${acquiredAbility.id}`}>Перейти</Button>
          {canEdit() &&
            <Button size="small" color="primary" variant="outlined" component={Link} to={`/editAcquiredAbility/${acquiredAbility.id}`}>Редактировать</Button>
          }
          {canDelete() &&
            <Button size="small" color="error" variant="outlined" onClick={() => setDeleteModalOpen(true)}>Удалить</Button>
          }
        </CardActions>
      </Card>
      <DeleteAcquiredAbilityModal open={deleteModalOpen} setOpen={setDeleteModalOpen} acquiredAbility={acquiredAbility} onDelete={() => navigate(0)} />
    </Grid>
  );
}
