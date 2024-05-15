import * as React from "react";
import Typography from "@mui/material/Typography";
import Grid from "@mui/material/Grid";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import { Button, CardActions } from "@mui/material";
import { displayName, RarityLevel } from "../../database";
import { Link } from "react-router-dom";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import DeleteRarityLevelModal from "../deletion-modals/delete-rarity-level-modal";
import { canDelete, canEdit } from "../../auth-state";

interface RarityLevelCardProps {
  rarityLevel: RarityLevel
}

export default function RarityLevelCard(props: RarityLevelCardProps) {
  const navigate = useNavigate();

  const { rarityLevel } = props;
  const [deleteModalOpen, setDeleteModalOpen] = useState<boolean>(false);

  return (
    <Grid item xs={12} md={6}>
      <Card sx={{ backgroundColor: "#fbfbfb" }}>
        <CardContent>
          <Typography gutterBottom variant="h5" component="div">
            {displayName(rarityLevel)}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            ID: {rarityLevel.id}
          </Typography>
        </CardContent>
        <CardActions sx={{ display: "flex", justifyContent: "center" }}>
          <Button size="small" color="primary" variant="outlined" component={Link} to={`/rarityLevel/${rarityLevel.id}`}>Перейти</Button>
          {canEdit() &&
            <Button size="small" color="primary" variant="outlined" component={Link} to={`/editRarityLevel/${rarityLevel.id}`}>Редактировать</Button>
          }
          {canDelete() &&
            <Button size="small" color="error" variant="outlined" onClick={() => setDeleteModalOpen(true)}>Удалить</Button>
          }
        </CardActions>
      </Card>
      <DeleteRarityLevelModal open={deleteModalOpen} setOpen={setDeleteModalOpen} rarityLevel={rarityLevel} onDelete={() => navigate(0)} />
    </Grid>
  );
}
