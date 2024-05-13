import { Container, Pagination, PaginationItem } from "@mui/material";
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import ArrowForwardIcon from '@mui/icons-material/ArrowForward';
import { ChangeEvent } from "react";
import { createSearchParams, useNavigate } from "react-router-dom";

interface PaginationProps {
  pageCount: number,
  currentPage: number,
  link: string,
  showBy: number,
  searchQuery: string | undefined
}

export default function CustomPagination(props: PaginationProps) {
  const navigate = useNavigate();

  const handleChange = (event: ChangeEvent<unknown>, value: number) => {
    if (props.searchQuery) {
      navigate({
        pathname: props.link,
        search: createSearchParams({ page: value.toString(), showBy: props.showBy.toString(), search: props.searchQuery }).toString()
      });
    }
    else {
      navigate({
        pathname: props.link,
        search: createSearchParams({ page: value.toString(), showBy: props.showBy.toString() }).toString()
      });
    }
  };

  return (
    <Container sx={{ display: "flex", justifyContent: "center", my: 4 }}>
      <Pagination
        count={props.pageCount}
        page={props.currentPage}
        color="primary"
        shape="rounded"
        size="large"
        showFirstButton
        showLastButton
        renderItem={item => (
          <PaginationItem
            slots={{ previous: ArrowBackIcon, next: ArrowForwardIcon }}
            {...item}
          />
        )}
        onChange={handleChange}
      />
    </Container>
  );
}