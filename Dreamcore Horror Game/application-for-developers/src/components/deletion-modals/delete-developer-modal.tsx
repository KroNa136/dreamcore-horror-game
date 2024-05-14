import * as React from 'react';
import Typography from '@mui/material/Typography';
import Grid from '@mui/material/Grid';
import { Backdrop, Box, Button, Fade, Modal } from '@mui/material';
import { displayName, Developer } from '../database';
import { deleteDeveloper } from '../requests';

interface DeleteDeveloperModalProps {
  open: boolean,
  setOpen: React.Dispatch<React.SetStateAction<boolean>>,
  developer: Developer,
  onDelete: () => any
}

export default function DeleteDeveloperModal(props: DeleteDeveloperModalProps) {
  const handleDelete = () => {
    deleteDeveloper(props.developer.id)
      .then(success => {
        if (success === true) {
          props.setOpen(false);
          props.onDelete();
        }
      });
  };

  return (
    <Modal
      open={props.open}
      onClose={() => props.setOpen(false)}
      aria-labelledby="modal-modal-title"
      aria-describedby="modal-modal-description"
      closeAfterTransition
      slots={{ backdrop: Backdrop }}
      slotProps={{
        backdrop: {
          timeout: 200,
        },
      }}
    >
      <Fade in={props.open}>
        <Box sx={{
          position: 'absolute' as 'absolute',
          top: '50%',
          left: '50%',
          transform: 'translate(-50%, -50%)',
          width: 480,
          bgcolor: 'background.paper',
          boxShadow: 24,
          p: 4,
        }}>
          <Typography id="modal-modal-title" variant="h6" component="h2">
            Подтверждение
          </Typography>
          <Typography id="modal-modal-description" sx={{ mt: 2 }}>
            Вы точно хотите удалить разработчика {displayName(props.developer)}?
          </Typography>
          <Grid container justifyContent="center" mt={3}>
            <Button size="medium" color="error" variant="outlined" sx={{ mx: 2 }} onClick={handleDelete}>Да</Button>
            <Button size="medium" color="primary" variant="outlined" sx={{ mx: 2 }} onClick={() => props.setOpen(false)}>Нет</Button>
          </Grid>
        </Box>
      </Fade>
    </Modal>
  );
}