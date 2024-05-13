import * as React from 'react';
import Toolbar from '@mui/material/Toolbar';
import Button from '@mui/material/Button';
import IconButton from '@mui/material/IconButton';
import { Container, Typography } from '@mui/material';
import { logoutAsDeveloper } from '../requests';
import { useNavigate } from 'react-router-dom';
import { useEffect, useState } from 'react';
import { getCurrentLogin } from '../auth-manager';

export default function Header() {
  const [signedOut, setSignedOut] = useState<boolean>(false);
  const navigate = useNavigate();

  const handleSignOut = async () => {
    const result = await logoutAsDeveloper();
    setSignedOut(result);
  }

  useEffect(() => {
    if (signedOut) {
      navigate("/signIn");
    }
  }, [signedOut]);

  return (
    <React.Fragment>
      <Toolbar sx={{ borderBottom: 1, borderColor: 'divider', mb: 4 }}>
        <IconButton onClick={() => navigate("/")}>
          <img width={48} height={48} src="/logo48.png" alt="Логотип" />
        </IconButton>
        <Container sx={{ display: "flex", justifyContent: "right", alignItems: "center" }}>
          <Typography component="p" variant="body1" color="primary" sx={{ mx: 1 }}>
            <b>{getCurrentLogin()}</b>
          </Typography>
          <Button variant="contained" size="small" sx={{ mx: 1 }} onClick={handleSignOut}>
            Выход
          </Button>
        </Container>
      </Toolbar>
    </React.Fragment>
  );
}