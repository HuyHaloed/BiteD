import $api from "../../services/api";
import {
  Stack,
  Typography,
  Table,
  TableContainer,
  TableHead,
  TableRow,
  TableCell,
  Paper,
  TableBody,
} from "@mui/material";
import WeekNavigator from "../../ui/WeekNavigator";
import { useWeekly } from "../../hooks/useWeekly";
import { BlueQrCodeDisplay } from "./QrCodeDisplay";
import QrStatBox from "./QrStatBox";
import dayjs from "dayjs";

function BlueQr() {
  const me = $api.useQuery("get", "/swagger/Users/me");
  const weekly = useWeekly();
  const weeklyScanes = $api.useQuery("get", "/swagger/Checkins/blue/weekly", {
    params: {
      query: {
        firstDayOfWeek: weekly.startOfWeek.format("YYYY-MM-DD"),
      },
    },
  });

  return (
    <Stack
      spacing={2}
      display="flex"
      flexDirection="column"
      alignItems="center"
    >
      <Typography variant="h5" textAlign="center">
        PERSONAL QR CODE
      </Typography>
      {me.data && <BlueQrCodeDisplay userId={me.data.id} />}
      <Typography variant="h6" textAlign="center">
        ORDER HISTORY
      </Typography>
      <WeekNavigator
        changeWeek={weekly.changeWeek}
        startOfWeek={weekly.startOfWeek}
        endOfWeek={weekly.endOfWeek}
      />
      <Stack direction="row" spacing={2} justifyContent="center">
        <QrStatBox
          title="Available"
          value={weeklyScanes.data?.maxScansPerWeek}
        />
        <QrStatBox title="Scanned" value={weeklyScanes.data?.scansThisWeek} />
        <QrStatBox
          title="Remaining"
          value={weeklyScanes.data?.scansRemaining}
        />
      </Stack>
      {weeklyScanes.data?.checkins.length === 0 ? (
        <Typography variant="body1" textAlign="center">
          No check-ins found for this week.
        </Typography>
      ) : (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Date time</TableCell>
                <TableCell>Location Name</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {weeklyScanes.data?.checkins.map((checkin) => (
                <TableRow key={checkin.id}>
                  <TableCell>
                    {dayjs(checkin.datetime).format("DD.MM.YYYY HH:mm")}
                  </TableCell>
                  <TableCell>{checkin.locationName}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
    </Stack>
  );
}

export default BlueQr;
