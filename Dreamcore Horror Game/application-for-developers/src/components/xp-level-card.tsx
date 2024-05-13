import * as React from 'react';
import Typography from '@mui/material/Typography';
import Grid from '@mui/material/Grid';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import { Button, CardActions } from '@mui/material';
import { displayName, XpLevel } from '../database';
import { Link } from 'react-router-dom';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import DeleteXpLevelModal from './delete-xp-level-modal';
import { canDelete, canEdit } from '../auth-manager';

interface XpLevelCardProps {
  xpLevel: XpLevel
}

export default function XpLevelCard(props: XpLevelCardProps) {
  const navigate = useNavigate();

  const { xpLevel } = props;
  const [deleteModalOpen, setDeleteModalOpen] = useState<boolean>(false);

  return (
    <Grid item xs={12} md={6}>
      <Card sx={{ backgroundColor: "#fbfbfb" }}>
        <CardContent>
          <Typography gutterBottom variant="h5" component="div">
            {displayName(xpLevel)}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            ID: {xpLevel.id}
          </Typography>
        </CardContent>
        <CardActions sx={{ display: "flex", justifyContent: "center" }}>
          <Button size="small" color="primary" variant="outlined" component={Link} to={`/xpLevel/${xpLevel.id}`}>Перейти</Button>
          {canEdit() &&
            <Button size="small" color="primary" variant="outlined" component={Link} to={`/editXpLevel/${xpLevel.id}`}>Редактировать</Button>
          }
          {canDelete() &&
            <Button size="small" color="error" variant="outlined" onClick={() => setDeleteModalOpen(true)}>Удалить</Button>
          }
        </CardActions>
      </Card>
      <DeleteXpLevelModal open={deleteModalOpen} setOpen={setDeleteModalOpen} xpLevel={xpLevel} onDelete={() => navigate(0)} />
    </Grid>
  );
}
