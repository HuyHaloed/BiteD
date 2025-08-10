import $api from "../../services/api";
import WeekNavigator from "../../ui/WeekNavigator";
import { useWeekly } from "../../hooks/useWeekly";
import { Card, CardContent, Stack, Typography } from "@mui/material";
import { getDailyOrderStatus } from "../../utils";
import { GreenQrCodeDisplay } from "./QrCodeDisplay";
import QrStatBox from "./QrStatBox";
import { Event, LabelOutlined } from "@mui/icons-material";
import dayjs from "dayjs";
import ShiftOrderInfo from "../order/ShiftOrderInfo";

function GreenQr() {
  const me = $api.useQuery("get", "/swagger/Users/me");
  const weekly = useWeekly();
  const weeklyOrders = $api.useQuery("get", "/swagger/Orders", {
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
        PRE-ORDER QR CODE
      </Typography>
      {me.data && <GreenQrCodeDisplay userId={me.data.id} />}
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
          title="Ordered"
          value={weeklyOrders.data?.numberOfShiftsOrdered}
        />
        <QrStatBox
          title="Scanned"
          value={weeklyOrders.data?.numberOfShiftsScanned}
        />
        <QrStatBox
          title="Remaining"
          value={weeklyOrders.data?.numberOfShiftsRemaining}
        />
      </Stack>
      <Stack spacing={2}>
        {weeklyOrders.data?.dailyOrders.map((dailyOrder) => (
          <Card
            key={dailyOrder.id}
            sx={{
              width: "100%",
              elevation: 2,
              borderRadius: "16px",
              border: "1px solid",
              borderColor: "primary.light",
            }}
          >
            <CardContent>
              <Stack
                direction="row"
                justifyContent="space-between"
                alignItems="center"
                mb={3}
              >
                <Stack direction="row" alignItems="center">
                  <Event />{" "}
                  <Typography
                    variant="h6"
                    style={{ marginLeft: "8px", fontWeight: 600 }}
                  >
                    {dayjs(dailyOrder.date).format("DD.MM")}
                  </Typography>
                </Stack>
                <Stack direction="row" alignItems="center">
                  <LabelOutlined />{" "}
                  <Typography
                    variant="h6"
                    style={{ marginLeft: "8px", fontWeight: 600 }}
                  >
                    {getDailyOrderStatus(dailyOrder.status)}
                  </Typography>
                </Stack>
              </Stack>
              <Stack spacing={2}>
                {dailyOrder.shiftOrders.map((shiftOrder) => (
                  <ShiftOrderInfo key={shiftOrder.id} shiftOrder={shiftOrder} />
                ))}
              </Stack>
            </CardContent>
          </Card>
        ))}
      </Stack>
    </Stack>
  );
}

export default GreenQr;
