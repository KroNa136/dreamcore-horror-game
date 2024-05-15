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
import { useEffect } from "react";
import { isSignedIn as isAlreadySignedIn } from "../auth-state";
import Footer from "../components/footer";
import { useAppDispatch, useAppSelector } from "../redux/hooks";
import { actions, resetState } from "../redux/slices/sign-in-form-slice";

export default function SignIn() {
  const navigate = useNavigate();

  const dispatch = useAppDispatch();
  const state = useAppSelector(state => state.signInForm);

  useEffect(() => {
    if (isAlreadySignedIn()) {
      navigate("/");
    }
  }, []);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const loginData: LoginData = {
      login: state.login,
      password: state.password
    };

    const result = await loginAsDeveloper(loginData);
    if (result) {
      resetState(dispatch);
      navigate("/");
    }
  };

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
              onChange={e => dispatch(actions.setLogin(e.target.value))}
            />
            <TextField
              margin="normal"
              required
              fullWidth
              name="password"
              label="Пароль"
              type="password"
              id="password"
              onChange={e => dispatch(actions.setPassword(e.target.value))}
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
            </Grid>
            */}
          </Box>
        </Box>
        <Footer/>
      </Container>
    </ThemeProvider>
  );
}
