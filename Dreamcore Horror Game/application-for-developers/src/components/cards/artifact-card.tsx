import * as React from "react";
import Typography from "@mui/material/Typography";
import Grid from "@mui/material/Grid";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import { Button, CardActions } from "@mui/material";
import { displayName, Artifact } from "../../database";
import { Link } from "react-router-dom";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import DeleteArtifactModal from "../deletion-modals/delete-artifact-modal";
import { canDelete, canEdit } from "../../auth-state";

interface ArtifactCardProps {
  artifact: Artifact
}

export default function ArtifactCard(props: ArtifactCardProps) {
  const navigate = useNavigate();

  const { artifact } = props;
  const [deleteModalOpen, setDeleteModalOpen] = useState<boolean>(false);

  return (
    <Grid item xs={12} md={6}>
      <Card sx={{ backgroundColor: "#fbfbfb" }}>
        <CardContent>
          <Typography gutterBottom variant="h5" component="div">
            {displayName(artifact)}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            ID: {artifact.id}
          </Typography>
        </CardContent>
        <CardActions sx={{ display: "flex", justifyContent: "center" }}>
          <Button size="small" color="primary" variant="outlined" component={Link} to={`/artifact/${artifact.id}`}>Перейти</Button>
          {canEdit() &&
            <Button size="small" color="primary" variant="outlined" component={Link} to={`/editArtifact/${artifact.id}`}>Редактировать</Button>
          }
          {canDelete() &&
            <Button size="small" color="error" variant="outlined" onClick={() => setDeleteModalOpen(true)}>Удалить</Button>
          }
        </CardActions>
      </Card>
      <DeleteArtifactModal open={deleteModalOpen} setOpen={setDeleteModalOpen} artifact={artifact} onDelete={() => navigate(0)} />
    </Grid>
  );
}
