import Backdrop from "@mui/material/Backdrop";
import Typography from "@mui/material/Typography";

import styles from "./FullscreenLoading.module.css";
import { Box } from "@mui/material";

export default function FullscreenLoading({ message }: { message?: string }) {
  return (
    <Backdrop
      sx={{ color: "#fff", zIndex: (theme) => theme.zIndex.drawer + 1 }}
      open={true}
    >
      <Box
        display="flex"
        justifyContent="center"
        alignItems="center"
        height="100vh"
      >
        {/* https://cssloaders.github.io/ */}
        <span className={styles.loader}></span>
      </Box>
      <Typography variant="h6" sx={{ mt: 0, ml: 10 }}>
        {message || "Cooking..."}
      </Typography>
    </Backdrop>
  );
}
