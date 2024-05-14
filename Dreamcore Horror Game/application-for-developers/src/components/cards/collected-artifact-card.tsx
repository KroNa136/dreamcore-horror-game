import * as React from "react";
import Typography from "@mui/material/Typography";
import Grid from "@mui/material/Grid";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import { Button, CardActions } from "@mui/material";
import { displayName, CollectedArtifact } from "../../database";
import { Link } from "react-router-dom";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import DeleteCollectedArtifactModal from "../deletion-modals/delete-collected-artifact-modal";
import { canDelete, canEdit } from "../../auth-manager";

interface CollectedArtifactCardProps {
  collectedArtifact: CollectedArtifact
}

export default function CollectedArtifactCard(props: CollectedArtifactCardProps) {
  const navigate = useNavigate();

  const { collectedArtifact } = props;
  const [deleteModalOpen, setDeleteModalOpen] = useState<boolean>(false);

  return (
    <Grid item xs={12} md={6}>
      <Card sx={{ backgroundColor: "#fbfbfb" }}>
        <CardContent>
          <Typography gutterBottom variant="h5" component="div">
            {displayName(collectedArtifact)}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            ID: {collectedArtifact.id}
          </Typography>
        </CardContent>
        <CardActions sx={{ display: "flex", justifyContent: "center" }}>
          <Button size="small" color="primary" variant="outlined" component={Link} to={`/collectedArtifact/${collectedArtifact.id}`}>Перейти</Button>
          {canEdit() &&
            <Button size="small" color="primary" variant="outlined" component={Link} to={`/editCollectedArtifact/${collectedArtifact.id}`}>Редактировать</Button>
          }
          {canDelete() &&
            <Button size="small" color="error" variant="outlined" onClick={() => setDeleteModalOpen(true)}>Удалить</Button>
          }
        </CardActions>
      </Card>
      <DeleteCollectedArtifactModal open={deleteModalOpen} setOpen={setDeleteModalOpen} collectedArtifact={collectedArtifact} onDelete={() => navigate(0)} />
    </Grid>
  );
}
