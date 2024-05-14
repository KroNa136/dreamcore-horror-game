import { createTheme } from "@mui/material/styles";
import { brown } from "@mui/material/colors";

export const defaultTheme = createTheme({
  palette: {
    primary: {
      main: brown[500],
    },
    secondary: {
      main: brown[300]
    }
  },
});
