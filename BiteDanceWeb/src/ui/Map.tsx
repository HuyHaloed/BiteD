import { Box } from "@mui/material";

export default function Map() {
  return (
    <Box
      sx={{
        width: "100%",
        height: "100%",
        minHeight: "0", // hide on mobile
        backgroundImage: "url(/map.png)",
        backgroundSize: "contain",
        backgroundRepeat: "no-repeat",
        backgroundPosition: "center",
      }}
    ></Box>
  );
}
