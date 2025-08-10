import { useEffect, useState } from "react";
import dayjs from "dayjs";
import isoWeek from "dayjs/plugin/isoWeek";
import $api from "../../services/api";
import { useWeekly } from "../../hooks/useWeekly";

dayjs.extend(isoWeek);

export function useOrderData() {
  const me = $api.useQuery("get", "/swagger/Users/me");

  // Locations
  const locations = $api.useQuery("get", "/swagger/Locations");
  const [locationId, setLocationId] = useState<number | null>(null);
  const location = locations.data?.find((loc) => loc.id === locationId);

  // Datetime
  const {
    week,
    setWeek,
    day,
    setDay,
    startOfWeek,
    endOfWeek,
    daysOfWeek,
    changeWeek,
  } = useWeekly();

  // Weekly Menu
  const weeklyMenu = $api.useQuery(
    "get",
    "/swagger/MonthlyMenus/weekly",
    {
      params: {
        query: {
          locationId: locationId!,
          firstDayOfWeek: startOfWeek.format("YYYY-MM-DD"),
        },
      },
    },
    { enabled: locationId != null }
  );

  // Weekly Orders Status
  const weeklyOrdersStatus = $api.useQuery(
    "get",
    "/swagger/Orders/status",
    {
      params: {
        query: {
          locationId: locationId!,
          firstDayOfWeek: startOfWeek.format("YYYY-MM-DD"),
        },
      },
    },
    { enabled: locationId != null }
  );

  useEffect(() => {
    if (locations.data) {
      const preferredLocationId = me.data?.preferredLocationId;
      setLocationId(preferredLocationId ?? locations.data[0]?.id);
    }
  }, [me.data, locations.data]);

 
 useEffect(() => {
  if (startOfWeek && daysOfWeek && !day) {
    const tomorrow = dayjs().add(1, "day");
    const tomorrowInWeek = daysOfWeek.find((d) => d.isSame(tomorrow, "day"));

    if (tomorrowInWeek) {
      setDay(tomorrowInWeek);
    } else {
      // Ngày mai không thuộc tuần hiện tại → chuyển sang tuần kế tiếp
      const nextWeek = startOfWeek.add(7, "day");
      setWeek(nextWeek);
      setDay(nextWeek); // ngày mai là thứ hai tuần sau
    }
  }
}, [startOfWeek, daysOfWeek, day, setDay, setWeek]);


  return {
    dateTime: {
      week,
      setWeek,
      day,
      setDay,
      startOfWeek,
      endOfWeek,
      daysOfWeek,
      changeWeek,
    },
    location: {
      locationId,
      setLocationId,
      locations,
      location,
    },
    menu: {
      weeklyMenu: weeklyMenu.data,
      selectedMenu: weeklyMenu.data?.find((m) =>
        dayjs(m.date).isSame(day, "day")
      ),
    },
    orderStatus: {
      weeklyStatus: weeklyOrdersStatus.data,
      selectedStatus: weeklyOrdersStatus.data?.find((s) =>
        dayjs(s.date).isSame(day, "day")
      ),
    },
  };
}
