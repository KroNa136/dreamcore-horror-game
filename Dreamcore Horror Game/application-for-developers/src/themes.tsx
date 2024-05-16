import { createTheme } from "@mui/material/styles";
import { brown as mainColor } from "@mui/material/colors";

export const defaultTheme = createTheme({
  palette: {
    primary: {
      main: mainColor[500],
    },
    secondary: {
      main: mainColor[300]
    }
  },
});
