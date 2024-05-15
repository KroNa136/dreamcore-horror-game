import * as React from "react";
import CssBaseline from "@mui/material/CssBaseline";
import Grid from "@mui/material/Grid";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import Header from "../components/header";
import DeveloperAccessLevelCard from "../components/cards/developer-access-level-card";
import Footer from "../components/footer";
import { defaultTheme } from "../themes";
import { Button, TextField, Typography } from "@mui/material";
import CustomPagination from "../components/custom-pagination";
import { useEffect, useState } from "react";
import { getDeveloperAccessLevels, getDeveloperAccessLevelsWhereDisplayName } from "../requests";
import { DeveloperAccessLevel } from "../database";
import { createSearchParams, Link, useNavigate, useSearchParams } from "react-router-dom";
import { canCreate } from "../auth-state";

interface TableProps {
  defaultShowBy: number,
}

let lastPage = 0;

export default function AllDeveloperAccessLevels(props: TableProps) {
  const navigate = useNavigate();
  const [searchParams, setSearchParams] = useSearchParams();

  const parsedPage = parseInt(searchParams.get("page") ?? "");
  const page = Number.isNaN(parsedPage) ? 1 : parsedPage;
  const parsedShowBy = parseInt(searchParams.get("showBy") ?? "");
  const showBy = Number.isNaN(parsedShowBy) ? props.defaultShowBy : parsedShowBy;

  const [developerAccessLevels, setDeveloperAccessLevels] = useState<DeveloperAccessLevel[]>([]);
  const [pageCount, setPageCount] = useState<number>(0);

  const [searchQuery, setSearchQuery] = useState<string>("");

  const getAllAndHandleSearch = () => {
    const search = searchParams.get("search");
    if (search) {
      getDeveloperAccessLevelsWhereDisplayName(search, page, showBy)
        .then(developerAccessLevels => {
          if (developerAccessLevels.pageCount < page) {
            navigate({
              pathname: "/developerAccessLevels",
              search: createSearchParams({ page: developerAccessLevels.pageCount.toString(), showBy: showBy.toString(), search: search }).toString()
            });
          }
          setDeveloperAccessLevels(developerAccessLevels.items);
          setPageCount(developerAccessLevels.pageCount);
        });
    } else {
      getDeveloperAccessLevels(page, showBy)
        .then(developerAccessLevels => {
          if (developerAccessLevels.pageCount < page) {
            navigate({
              pathname: "/developerAccessLevels",
              search: createSearchParams({ page: developerAccessLevels.pageCount.toString(), showBy: showBy.toString() }).toString()
            });
          }
          setDeveloperAccessLevels(developerAccessLevels.items);
          setPageCount(developerAccessLevels.pageCount);
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
          <Typography component="h3" variant="h3" align="left" my={4}>Уровни доступа разработчиков</Typography>
          <Grid container spacing={4} direction="row" justifyContent="space-between" alignItems="center" my={4} px={2}>
            {canCreate() &&
              <Button size="medium" variant="contained" color="primary" sx={{ mx: 1 }} component={Link} to={"/createDeveloperAccessLevel"}>Создать</Button>
            }
            <Grid justifyContent="end" alignItems="center" justifySelf="end">
              <TextField sx={{ mx: 1, mb: 2, width: 500 }} size="small" margin="normal" id="search" label="Поиск" name="search" onChange={e => setSearchQuery(e.target.value)} />
              <Button size="medium" variant="outlined" color="primary" sx={{ mx: 1, mt: 2 }} onClick={handleSearchClick}>Найти</Button>
            </Grid>
          </Grid>
          <Grid container spacing={4}>
            {developerAccessLevels.map((developerAccessLevel) => (
              <DeveloperAccessLevelCard key={developerAccessLevel.id} developerAccessLevel={developerAccessLevel} />
            ))}
          </Grid>
        </main>
        <CustomPagination pageCount={pageCount} currentPage={page} searchQuery={searchQuery} link="/developerAccessLevels" showBy={showBy} />
        <Footer />
      </Container>
    </ThemeProvider>
  );
}