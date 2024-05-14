import * as React from "react";
import CssBaseline from "@mui/material/CssBaseline";
import Grid from "@mui/material/Grid";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import Header from "../components/header";
import CollectedArtifactCard from "../components/cards/collected-artifact-card";
import Footer from "../components/footer";
import { defaultTheme } from "../themes";
import { Button, TextField, Typography } from "@mui/material";
import CustomPagination from "../components/custom-pagination";
import { useEffect, useState } from "react";
import { getCollectedArtifacts, getCollectedArtifactsWhereDisplayName } from "../requests";
import { CollectedArtifact } from "../database";
import { createSearchParams, Link, useNavigate, useSearchParams } from "react-router-dom";
import { canCreate } from "../auth-manager";

interface TableProps {
  defaultShowBy: number,
}

let lastPage = 0;

export default function AllCollectedArtifacts(props: TableProps) {
  const navigate = useNavigate();
  const [searchParams, setSearchParams] = useSearchParams();

  const parsedPage = parseInt(searchParams.get("page") ?? "");
  const page = Number.isNaN(parsedPage) ? 1 : parsedPage;
  const parsedShowBy = parseInt(searchParams.get("showBy") ?? "");
  const showBy = Number.isNaN(parsedShowBy) ? props.defaultShowBy : parsedShowBy;

  const [collectedArtifacts, setCollectedArtifacts] = useState<CollectedArtifact[]>([]);
  const [pageCount, setPageCount] = useState<number>(0);

  const [searchQuery, setSearchQuery] = useState<string>("");

  const getAllAndHandleSearch = () => {
    const search = searchParams.get("search");
    if (search) {
      getCollectedArtifactsWhereDisplayName(search, page, showBy)
        .then(collectedArtifacts => {
          if (collectedArtifacts.pageCount < page) {
            navigate({
              pathname: "/collectedArtifacts",
              search: createSearchParams({ page: collectedArtifacts.pageCount.toString(), showBy: showBy.toString(), search: search }).toString()
            });
          }
          setCollectedArtifacts(collectedArtifacts.items);
          setPageCount(collectedArtifacts.pageCount);
        });
    } else {
      getCollectedArtifacts(page, showBy)
        .then(collectedArtifacts => {
          if (collectedArtifacts.pageCount < page) {
            navigate({
              pathname: "/collectedArtifacts",
              search: createSearchParams({ page: collectedArtifacts.pageCount.toString(), showBy: showBy.toString() }).toString()
            });
          }
          setCollectedArtifacts(collectedArtifacts.items);
          setPageCount(collectedArtifacts.pageCount);
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
          <Typography component="h3" variant="h3" align="left" my={4}>Собранные артефакты</Typography>
          <Grid container spacing={4} direction="row" justifyContent="space-between" alignItems="center" my={4} px={2}>
            {canCreate() &&
              <Button size="medium" variant="contained" color="primary" sx={{ mx: 1 }} component={Link} to={"/createCollectedArtifact"}>Создать</Button>
            }
            <Grid justifyContent="end" alignItems="center" justifySelf="end">
              <TextField sx={{ mx: 1, mb: 2, width: 500 }} size="small" margin="normal" id="search" label="Поиск" name="search" onChange={handleSearchQueryChange} />
              <Button size="medium" variant="outlined" color="primary" sx={{ mx: 1, mt: 2 }} onClick={handleSearchClick}>Найти</Button>
            </Grid>
          </Grid>
          <Grid container spacing={4}>
            {collectedArtifacts.map((collectedArtifact) => (
              <CollectedArtifactCard key={collectedArtifact.id} collectedArtifact={collectedArtifact} />
            ))}
          </Grid>
        </main>
        <CustomPagination pageCount={pageCount} currentPage={page} searchQuery={searchQuery} link="/collectedArtifacts" showBy={showBy} />
        <Footer />
      </Container>
    </ThemeProvider>
  );
}