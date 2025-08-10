import { Box, Typography } from "@mui/material";

interface QrStatBoxProps {
  title: string;
  value: number | undefined;
}

function QrStatBox({ title, value }: QrStatBoxProps) {
  return (
    <Box
      display="flex"
      flexDirection="column"
      alignItems="center"
      sx={{
        backgroundColor: "lightgreen",
        borderRadius: "5px",
        padding: "10px",
        width: "100px",
      }}
    >
      <Typography variant="body2">{title}</Typography>
      <Typography variant="h6" fontWeight={600} color="primary">
        {value}
      </Typography>
    </Box>
  );
}

export default QrStatBox;
