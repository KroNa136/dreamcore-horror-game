import * as React from "react";
import Typography from "@mui/material/Typography";
import Grid from "@mui/material/Grid";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import { Button, CardActions } from "@mui/material";
import { displayName, DeveloperAccessLevel } from "../../database";
import { Link } from "react-router-dom";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import DeleteDeveloperAccessLevelModal from "../deletion-modals/delete-developer-access-level-modal";
import { canDelete, canEdit } from "../../auth-manager";

interface DeveloperAccessLevelCardProps {
  developerAccessLevel: DeveloperAccessLevel
}

export default function DeveloperAccessLevelCard(props: DeveloperAccessLevelCardProps) {
  const navigate = useNavigate();

  const { developerAccessLevel } = props;
  const [deleteModalOpen, setDeleteModalOpen] = useState<boolean>(false);

  return (
    <Grid item xs={12} md={6}>
      <Card sx={{ backgroundColor: "#fbfbfb" }}>
        <CardContent>
          <Typography gutterBottom variant="h5" component="div">
            {displayName(developerAccessLevel)}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            ID: {developerAccessLevel.id}
          </Typography>
        </CardContent>
        <CardActions sx={{ display: "flex", justifyContent: "center" }}>
          <Button size="small" color="primary" variant="outlined" component={Link} to={`/developerAccessLevel/${developerAccessLevel.id}`}>Перейти</Button>
          {canEdit() &&
            <Button size="small" color="primary" variant="outlined" component={Link} to={`/editDeveloperAccessLevel/${developerAccessLevel.id}`}>Редактировать</Button>
          }
          {canDelete() &&
            <Button size="small" color="error" variant="outlined" onClick={() => setDeleteModalOpen(true)}>Удалить</Button>
          }
        </CardActions>
      </Card>
      <DeleteDeveloperAccessLevelModal open={deleteModalOpen} setOpen={setDeleteModalOpen} developerAccessLevel={developerAccessLevel} onDelete={() => navigate(0)} />
    </Grid>
  );
}
