import { useOrderData } from "../features/order/useOrderData";
import { FormControl, Button, Stack, Typography, Box } from "@mui/material";
import dayjs from "dayjs";
import isoWeek from "dayjs/plugin/isoWeek";
import DailyOrder from "../features/order/DailyOrder";
import DailyOrderedInfo from "../features/order/DailyOrderedInfo";
import WeekNavigator from "../ui/WeekNavigator";
import "./Order.css";
import { useState } from "react";
import ConfettiExplosion from "react-confetti-explosion";

dayjs.extend(isoWeek);

export default function Order() {
  const { dateTime, location, menu, orderStatus } = useOrderData();
  const [isConfettiExploding, setIsConfettiExploding] = useState(false);

  return (
    <>
      {/* show confetti when order success */}
      {isConfettiExploding && (
        <ConfettiExplosion
          force={0.6}
          duration={2500}
          particleCount={200}
          height={2000}
          colors={["#9A0023", "#FF003C", "#AF739B", "#FAC7F3", "#F7DBF4"]}
          onComplete={() => setIsConfettiExploding(false)}
        />
      )}
      {/* location picker */}
      <FormControl fullWidth>
        <Typography variant="body2" textAlign="center">
          Location
        </Typography>
        <div className="custom-select-container">
          {/* <LocationOnIcon className="location-icon" /> */}
          <select
            className="custom-select"
            value={location.locationId?.toString() || ""}
            onChange={(event) =>
              location.setLocationId(Number(event.target.value))
            }
          >
            {location.locations.data?.map((location) => (
              <option key={location.id} value={location.id}>
                {location.name}
              </option>
            ))}
          </select>
        </div>
      </FormControl>
      {/* <Debug obj={menu}>Monthly Menu</Debug>
      <Debug obj={orderStatus}>Monthly Orders Status</Debug> */}
      {/* week navigator */}
      <Stack direction="column" spacing={2}>
        <WeekNavigator
          changeWeek={dateTime.changeWeek}
          startOfWeek={dateTime.startOfWeek}
          endOfWeek={dateTime.endOfWeek}
        />
        {/* week days */}
        <Stack direction="row" spacing={2} justifyContent="center">
          {dateTime.daysOfWeek.map((day) => (
            <Stack
              direction="column"
              spacing={1}
              key={day.format("YYYY-MM-DD")}
            >
              <Typography variant="body2" textAlign="center" fontSize={12}>
                {day.format("ddd")}
              </Typography>
              <Button
                onClick={() => dateTime.setDay(day)}
                color={
                  dayjs(day).isSame(dateTime.day, "day")
                    ? "primary"
                    : orderStatus.weeklyStatus?.some(
                        (status) =>
                          dayjs(status.date).isSame(day, "day") &&
                          !status.canOrder
                      )
                    ? "inherit"
                    : "secondary"
                }
                variant="contained"
                sx={{
                  borderRadius: "50%",
                  border: orderStatus.weeklyStatus?.some(
                    (status) =>
                      dayjs(status.date).isSame(day, "day") && status.isOrdered // green border if the day is ordered
                  )
                    ? "2px solid green"
                    : "none",
                  width: "30px",
                  height: "33px", // make the button taller to accommodate the border
                  minWidth: "30px",
                }}
              >
                {day.format("D")}
              </Button>
            </Stack>
          ))}
        </Stack>
      </Stack>
      {/* <Debug obj={orderStatus.selectedStatus}>Selected status</Debug> */}
      {/* menu and order */}
      {dateTime.day && location.location && (
        <Box mt={2}>
          {menu?.selectedMenu && !orderStatus.selectedStatus?.isOrdered && (
            <>
              {!orderStatus.selectedStatus?.canOrder && (
                <Typography
                  variant="body2"
                  textAlign="center"
                  color="text.secondary"
                >
                  Ordering is closed for this day.
                </Typography>
              )}
              <DailyOrder
                menu={menu.selectedMenu}
                location={location.location}
                key={menu.selectedMenu.date}
                canOrder={orderStatus.selectedStatus?.canOrder ?? false}
                onOrderSuccess={() => setIsConfettiExploding(true)}
              />
            </>
          )}
          {!menu.selectedMenu && (
            <Typography variant="body2" textAlign="center">
              No menu for this day
            </Typography>
          )}
          {orderStatus.selectedStatus?.isOrdered &&
            orderStatus.selectedStatus.dailyOrderInfo && (
              <DailyOrderedInfo
                orderInfo={orderStatus.selectedStatus.dailyOrderInfo}
                key={orderStatus.selectedStatus.date}
              />
            )}
        </Box>
      )}
    </>
  );
}
