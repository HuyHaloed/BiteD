import { useState } from "react";
import { Button, Typography, Box } from "@mui/material";

export default function ErrorMessage({ error }: { error?: object | null }) {
  const [showDetails, setShowDetails] = useState(false);

  return (
    <Box>
      <Typography variant="h6" color="error">
        An unexpected error occurred. Please try again or contact the
        administrator.
      </Typography>
      {error && (
        <>
          <Button
            variant="contained"
            color="primary"
            onClick={() => setShowDetails(!showDetails)}
          >
            {showDetails ? "Hide Details" : "Show Details"}
          </Button>
          {showDetails && (
            <Box mt={2}>
              <Typography variant="body2" component="pre">
                {JSON.stringify(error, null, 2)}
              </Typography>
            </Box>
          )}
        </>
      )}
    </Box>
  );
}
