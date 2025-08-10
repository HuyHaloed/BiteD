import { Box, Typography } from "@mui/material";
import { components } from "../../services/api.openapi";
import PersonOutlineIcon from "@mui/icons-material/PersonOutline";

export default function RequestDetails({
  request,
}: {
  request: components["schemas"]["RedCodeRequestDto"];
}) {
  return (
    <Box display="flex" flexDirection="column" gap={1}>
      <Box display="flex" alignItems="center">
        <PersonOutlineIcon />
        <Typography variant="body2" ml={1}>
          Reason for visit: {request.guestPurposeOfVisit}
        </Typography>
      </Box>
      
    </Box>
  );
}
