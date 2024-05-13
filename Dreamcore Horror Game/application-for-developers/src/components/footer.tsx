import * as React from 'react';
import Box from '@mui/material/Box';
import Container from '@mui/material/Container';
import Typography from '@mui/material/Typography';
import Copyright from './copyright';

export default function Footer() {
  return (
    <Box component="footer" sx={{ bgcolor: 'background.paper', py: 6, mt: 4, borderTop: 1, borderColor: 'divider' }}>
      <Container maxWidth="lg">
        <Typography variant="h6" align="center" gutterBottom>
          Dreamcore Horror Game
        </Typography>
        <Typography
          variant="subtitle1"
          align="center"
          color="text.secondary"
          component="p"
          mb={1}
        >
          Корпоративное веб-решение для управления данными и облегчения процесса разработки.
        </Typography>
        <Copyright />
      </Container>
    </Box>
  );
}