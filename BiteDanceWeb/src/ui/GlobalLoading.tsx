import { useIsFetching, useIsMutating } from "@tanstack/react-query";
import { LinearProgress } from "@mui/material";

const GlobalLoading = () => {
  const isFetching = useIsFetching();
  const isMutating = useIsMutating();

  return (
    <>
      {(isFetching > 0 || isMutating > 0) && (
        <LinearProgress
          style={{
            position: "fixed",
            top: 0,
            left: 0,
            width: "100%",
            zIndex: 1300,
          }}
        />
      )}
    </>
  );
};

export default GlobalLoading;
