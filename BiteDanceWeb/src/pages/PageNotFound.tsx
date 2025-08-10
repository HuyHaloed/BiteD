import { Home } from "@mui/icons-material";
import { Typography, Button, Container, Stack } from "@mui/material";
import { useNavigate } from "react-router-dom";
function PageNotFound() {
  const navigate = useNavigate();

  return (
    <Container>
      <Stack
        spacing={2}
        alignItems="center"
        justifyContent="center"
        height="100vh"
      >
        <Typography variant="h2" gutterBottom textAlign="center">
          Page not found
        </Typography>
        <Button
          variant="contained"
          color="primary"
          onClick={() => navigate("/")}
          startIcon={<Home />}
        >
          Go to home
        </Button>
      </Stack>
    </Container>
  );
}

export default PageNotFound;
