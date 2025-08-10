import { useState } from "react";
import dayjs, { Dayjs } from "dayjs";
import isoWeek from "dayjs/plugin/isoWeek";

dayjs.extend(isoWeek);

export function useWeekly() {
  const [week, setWeek] = useState(dayjs());
  const [day, setDay] = useState<Dayjs | null>(null);

  const startOfWeek = week.startOf("isoWeek");
  const endOfWeek = week.endOf("isoWeek");
  const daysOfWeek = Array.from({ length: 7 }, (_, i) =>
    startOfWeek.add(i, "day")
  );

  const changeWeek = (direction: "next" | "previous") => {
    setWeek((prevWeek) => {
      const newWeek =
        direction === "next"
          ? prevWeek.add(1, "week")
          : prevWeek.subtract(1, "week");
      setDay(newWeek.startOf("isoWeek"));
      return newWeek;
    });
  };

  return {
    week,
    setWeek,
    day,
    setDay,
    startOfWeek,
    endOfWeek,
    daysOfWeek,
    changeWeek,
  };
}
