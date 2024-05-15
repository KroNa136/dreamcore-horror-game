import * as React from "react";
import Typography from "@mui/material/Typography";
import Grid from "@mui/material/Grid";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import { Button, CardActions } from "@mui/material";
import { displayName, Developer } from "../../database";
import { Link } from "react-router-dom";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import DeleteDeveloperModal from "../deletion-modals/delete-developer-modal";
import { canDelete, canEdit } from "../../auth-state";

interface DeveloperCardProps {
  developer: Developer
}

export default function DeveloperCard(props: DeveloperCardProps) {
  const navigate = useNavigate();

  const { developer } = props;
  const [deleteModalOpen, setDeleteModalOpen] = useState<boolean>(false);

  return (
    <Grid item xs={12} md={6}>
      <Card sx={{ backgroundColor: "#fbfbfb" }}>
        <CardContent>
          <Typography gutterBottom variant="h5" component="div">
            {displayName(developer)}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            ID: {developer.id}
          </Typography>
        </CardContent>
        <CardActions sx={{ display: "flex", justifyContent: "center" }}>
          <Button size="small" color="primary" variant="outlined" component={Link} to={`/developer/${developer.id}`}>Перейти</Button>
          {canEdit() &&
            <Button size="small" color="primary" variant="outlined" component={Link} to={`/editDeveloper/${developer.id}`}>Редактировать</Button>
          }
          {canDelete() &&
            <Button size="small" color="error" variant="outlined" onClick={() => setDeleteModalOpen(true)}>Удалить</Button>
          }
        </CardActions>
      </Card>
      <DeleteDeveloperModal open={deleteModalOpen} setOpen={setDeleteModalOpen} developer={developer} onDelete={() => navigate(0)} />
    </Grid>
  );
}
