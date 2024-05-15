import * as React from "react";
import CssBaseline from "@mui/material/CssBaseline";
import Grid from "@mui/material/Grid";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import Header from "../components/header";
import RarityLevelCard from "../components/cards/rarity-level-card";
import Footer from "../components/footer";
import { defaultTheme } from "../themes";
import { Button, TextField, Typography } from "@mui/material";
import CustomPagination from "../components/custom-pagination";
import { useEffect, useState } from "react";
import { getRarityLevels, getRarityLevelsWhereDisplayName } from "../requests";
import { RarityLevel } from "../database";
import { createSearchParams, Link, useNavigate, useSearchParams } from "react-router-dom";
import { canCreate } from "../auth-state";

interface TableProps {
  defaultShowBy: number,
}

let lastPage = 0;

export default function AllRarityLevels(props: TableProps) {
  const navigate = useNavigate();
  const [searchParams, setSearchParams] = useSearchParams();

  const parsedPage = parseInt(searchParams.get("page") ?? "");
  const page = Number.isNaN(parsedPage) ? 1 : parsedPage;
  const parsedShowBy = parseInt(searchParams.get("showBy") ?? "");
  const showBy = Number.isNaN(parsedShowBy) ? props.defaultShowBy : parsedShowBy;

  const [rarityLevels, setRarityLevels] = useState<RarityLevel[]>([]);
  const [pageCount, setPageCount] = useState<number>(0);

  const [searchQuery, setSearchQuery] = useState<string>("");

  const getAllAndHandleSearch = () => {
    const search = searchParams.get("search");
    if (search) {
      getRarityLevelsWhereDisplayName(search, page, showBy)
        .then(rarityLevels => {
          if (rarityLevels.pageCount < page) {
            navigate({
              pathname: "/rarityLevels",
              search: createSearchParams({ page: rarityLevels.pageCount.toString(), showBy: showBy.toString(), search: search }).toString()
            });
          }
          setRarityLevels(rarityLevels.items);
          setPageCount(rarityLevels.pageCount);
        });
    } else {
      getRarityLevels(page, showBy)
        .then(rarityLevels => {
          if (rarityLevels.pageCount < page) {
            navigate({
              pathname: "/rarityLevels",
              search: createSearchParams({ page: rarityLevels.pageCount.toString(), showBy: showBy.toString() }).toString()
            });
          }
          setRarityLevels(rarityLevels.items);
          setPageCount(rarityLevels.pageCount);
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
          <Typography component="h3" variant="h3" align="left" my={4}>Уровни редкости</Typography>
          <Grid container spacing={4} direction="row" justifyContent="space-between" alignItems="center" my={4} px={2}>
            {canCreate() &&
              <Button size="medium" variant="contained" color="primary" sx={{ mx: 1 }} component={Link} to={"/createRarityLevel"}>Создать</Button>
            }
            <Grid justifyContent="end" alignItems="center" justifySelf="end">
              <TextField sx={{ mx: 1, mb: 2, width: 500 }} size="small" margin="normal" id="search" label="Поиск" name="search" onChange={e => setSearchQuery(e.target.value)} />
              <Button size="medium" variant="outlined" color="primary" sx={{ mx: 1, mt: 2 }} onClick={handleSearchClick}>Найти</Button>
            </Grid>
          </Grid>
          <Grid container spacing={4}>
            {rarityLevels.map((rarityLevel) => (
              <RarityLevelCard key={rarityLevel.id} rarityLevel={rarityLevel} />
            ))}
          </Grid>
        </main>
        <CustomPagination pageCount={pageCount} currentPage={page} searchQuery={searchQuery} link="/rarityLevels" showBy={showBy} />
        <Footer />
      </Container>
    </ThemeProvider>
  );
}