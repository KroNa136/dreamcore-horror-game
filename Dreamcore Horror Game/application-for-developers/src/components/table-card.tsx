import * as React from 'react';
import Typography from '@mui/material/Typography';
import Grid from '@mui/material/Grid';
import Card from '@mui/material/Card';
import CardActionArea from '@mui/material/CardActionArea';
import CardContent from '@mui/material/CardContent';
import CardMedia from '@mui/material/CardMedia';

interface TableCardProps {
  table: {
    title: string;
    databaseTitle: string;
    entryCount: number;
    image: string;
    imageLabel: string;
    link: string;
  };
}

export default function TableCard(props: TableCardProps) {
  const { table } = props;

  return (
    <Grid item xs={12} md={6}>
      <CardActionArea component="a" href={table.link}>
        <Card sx={{ display: 'flex', backgroundColor: "#fbfbfb" }}>
          <CardContent sx={{ flex: 1 }}>
            <Typography component="h2" variant="h5">
              {table.title}
            </Typography>
            <Typography component="h5" variant="subtitle1" color="secondary">
              {table.databaseTitle}
            </Typography>
            <Typography component="h5" variant="caption" color="secondary" mt={6}>
              Всего записей: {table.entryCount.toString()}
            </Typography>
          </CardContent>
          <CardMedia
            component="img"
            sx={{ width: 64, height: 64, display: { xs: 'none', sm: 'block' }, alignSelf: 'center', mx: 2 }}
            image={table.image}
            alt={table.imageLabel}
          />
        </Card>
      </CardActionArea>
    </Grid>
  );
}