import * as React from "react";
import CssBaseline from "@mui/material/CssBaseline";
import Grid from "@mui/material/Grid";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import Header from "../components/header";
import GameModeCard from "../components/cards/game-mode-card";
import Footer from "../components/footer";
import { defaultTheme } from "../themes";
import { Button, TextField, Typography } from "@mui/material";
import CustomPagination from "../components/custom-pagination";
import { useEffect, useState } from "react";
import { getGameModes, getGameModesWhereDisplayName } from "../requests";
import { GameMode } from "../database";
import { createSearchParams, Link, useNavigate, useSearchParams } from "react-router-dom";
import { canCreate } from "../auth-manager";

interface TableProps {
  defaultShowBy: number,
}

let lastPage = 0;

export default function AllGameModes(props: TableProps) {
  const navigate = useNavigate();
  const [searchParams, setSearchParams] = useSearchParams();

  const parsedPage = parseInt(searchParams.get("page") ?? "");
  const page = Number.isNaN(parsedPage) ? 1 : parsedPage;
  const parsedShowBy = parseInt(searchParams.get("showBy") ?? "");
  const showBy = Number.isNaN(parsedShowBy) ? props.defaultShowBy : parsedShowBy;

  const [gameModes, setGameModes] = useState<GameMode[]>([]);
  const [pageCount, setPageCount] = useState<number>(0);

  const [searchQuery, setSearchQuery] = useState<string>("");

  const getAllAndHandleSearch = () => {
    const search = searchParams.get("search");
    if (search) {
      getGameModesWhereDisplayName(search, page, showBy)
        .then(gameModes => {
          if (gameModes.pageCount < page) {
            navigate({
              pathname: "/gameModes",
              search: createSearchParams({ page: gameModes.pageCount.toString(), showBy: showBy.toString(), search: search }).toString()
            });
          }
          setGameModes(gameModes.items);
          setPageCount(gameModes.pageCount);
        });
    } else {
      getGameModes(page, showBy)
        .then(gameModes => {
          if (gameModes.pageCount < page) {
            navigate({
              pathname: "/gameModes",
              search: createSearchParams({ page: gameModes.pageCount.toString(), showBy: showBy.toString() }).toString()
            });
          }
          setGameModes(gameModes.items);
          setPageCount(gameModes.pageCount);
        });
    }
  };

  if (page !== lastPage) {
    lastPage = page;
    getAllAndHandleSearch();
  }

  useEffect(() => {
    getAllAndHandleSearch();
  }, [searchParams]);

  const handleSearchQueryChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSearchQuery(event.target.value);
  };

  const handleSearchClick = () => {
    setSearchParams(searchParams => {
      if (searchQuery !== "") {
        searchParams.set("search", searchQuery);
      } else {
        searchParams.delete("search");
      }
      searchParams.set("page", "1");
      return searchParams;
    });
  };

  return (
    <ThemeProvider theme={defaultTheme}>
      <CssBaseline />
      <Container maxWidth="lg">
        <Header />
        <main>
          <Typography component="h3" variant="h3" align="left" my={4}>Игровые режимы</Typography>
          <Grid container spacing={4} direction="row" justifyContent="space-between" alignItems="center" my={4} px={2}>
            {canCreate() &&
              <Button size="medium" variant="contained" color="primary" sx={{ mx: 1 }} component={Link} to={"/createGameMode"}>Создать</Button>
            }
            <Grid justifyContent="end" alignItems="center" justifySelf="end">
              <TextField sx={{ mx: 1, mb: 2, width: 500 }} size="small" margin="normal" id="search" label="Поиск" name="search" onChange={handleSearchQueryChange} />
              <Button size="medium" variant="outlined" color="primary" sx={{ mx: 1, mt: 2 }} onClick={handleSearchClick}>Найти</Button>
            </Grid>
          </Grid>
          <Grid container spacing={4}>
            {gameModes.map((gameMode) => (
              <GameModeCard key={gameMode.id} gameMode={gameMode} />
            ))}
          </Grid>
        </main>
        <CustomPagination pageCount={pageCount} currentPage={page} searchQuery={searchQuery} link="/gameModes" showBy={showBy} />
        <Footer />
      </Container>
    </ThemeProvider>
  );
}