import { IconButton, Stack, Typography } from "@mui/material";
import { ArrowBackIos, ArrowForwardIos } from "@mui/icons-material";
import dayjs from "dayjs";

interface WeekNavigatorProps {
  changeWeek: (direction: "previous" | "next") => void;
  startOfWeek: dayjs.Dayjs;
  endOfWeek: dayjs.Dayjs;
}

export default function WeekNavigator({
  changeWeek,
  startOfWeek,
  endOfWeek,
}: WeekNavigatorProps) {
  return (
    <Stack
      direction="row"
      spacing={2}
      justifyContent="center"
      alignItems="center"
    >
      <IconButton onClick={() => changeWeek("previous")}>
        <ArrowBackIos />
      </IconButton>
      <Typography variant="h6" fontWeight={500}>
        {startOfWeek.format("DD.MM")} - {endOfWeek.format("DD.MM")}
      </Typography>
      <IconButton onClick={() => changeWeek("next")}>
        <ArrowForwardIos />
      </IconButton>
    </Stack>
  );
}
