import React, { useEffect, useState } from "react";
import dayjs from "dayjs";

const CurrentTime: React.FC = () => {
  const [currentTime, setCurrentTime] = useState(dayjs());

  useEffect(() => {
    const timer = setInterval(() => {
      setCurrentTime(dayjs());
    }, 1000);

    return () => clearInterval(timer);
  }, []);

  return <span>Current Time: {currentTime.format("YYYY-MM-DD HH:mm:ss")}</span>;
};

export default CurrentTime;
