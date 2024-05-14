import * as React from "react";
import Avatar from "@mui/material/Avatar";
import Button from "@mui/material/Button";
import CssBaseline from "@mui/material/CssBaseline";
import TextField from "@mui/material/TextField";
import Box from "@mui/material/Box";
import LockOutlinedIcon from "@mui/icons-material/LockOutlined";
import Typography from "@mui/material/Typography";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import { defaultTheme } from "../themes";
import { loginAsDeveloper, LoginData } from "../requests";
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { isSignedIn as isAlreadySignedIn } from "../auth-manager";
import Footer from "../components/footer";

export default function SignIn() {
  const [signedIn, setSignedIn] = useState<boolean>(false);
  const navigate = useNavigate();

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const data = new FormData(event.currentTarget);
    const login = data.get("login")?.toString();
    const password = data.get("password")?.toString();
    const loginData: LoginData = {
      login: login ?? "",
      password: password ?? "",
    };
    const result = await loginAsDeveloper(loginData);
    if (result === true) {
      setSignedIn(true);
    }
  };

  useEffect(() => {
    if (isAlreadySignedIn()) {
      navigate("/");
    }
  }, []);

  useEffect(() => {
    if (signedIn) {
      navigate("/");
    }
  }, [signedIn]);

  return (
    <ThemeProvider theme={defaultTheme}>
      <Container component="main" maxWidth="xs">
        <CssBaseline />
        <Box
          sx={{
            marginTop: 10,
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
          }}
        >
          <Avatar sx={{ m: 1, bgcolor: "primary.main" }}>
            <LockOutlinedIcon />
          </Avatar>
          <Typography component="h1" variant="h5">
            Вход
          </Typography>
          <Box component="form" onSubmit={handleSubmit} noValidate sx={{ mt: 1 }}>
            <TextField
              margin="normal"
              required
              fullWidth
              id="login"
              label="Логин"
              name="login"
              autoFocus
            />
            <TextField
              margin="normal"
              required
              fullWidth
              name="password"
              label="Пароль"
              type="password"
              id="password"
            />
            <Button
              type="submit"
              fullWidth
              variant="contained"
              color="primary"
              sx={{ mt: 3, mb: 2 }}
            >
              Войти
            </Button>
            {/*
            <Grid container>
              <Grid item xs>
                <Link href="#" variant="body2">
                  Забыли пароль?
                </Link>
              </Grid>
              <Grid item>
                <Link href="#" variant="body2">
                  {"Don't have an account? Sign Up"}
                </Link>
              </Grid>
            </Grid>
            */}
          </Box>
        </Box>
        <Footer/>
      </Container>
    </ThemeProvider>
  );
}
