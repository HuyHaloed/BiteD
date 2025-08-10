import { useEffect } from "react";
import {
  Box,

  Grid,
  MenuItem,

  TextField,
  Typography,
} from "@mui/material";
import { useState } from "react";
import $api from "../services/api";
import FullscreenLoading from "../ui/FullscreenLoading";
import ErrorMessage from "../ui/ErrorMessage";
import dayjs, { Dayjs } from "dayjs";
import { useDebounce } from "use-debounce";
import {
  RedCodeRequestStatus,
} from "../services/api.openapi";
import { LocalizationProvider, DatePicker } from "@mui/x-date-pickers";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";

const cellStyle: React.CSSProperties = {
  border: "1px solid #ddd",
  padding: "8px",
  textAlign: "left",
};

export default function DailyReports() {

  const [selectedDate, setSelectedDate] = useState<Dayjs>(dayjs().add(1,'day')); // hôm nay mặc định
  const formattedDate = selectedDate.format("YYYY-MM-DD");
  const isAfterReportTime = dayjs().isAfter(
    selectedDate.subtract(1, "day").hour(16).minute(0).second(0)
  );



  const me = $api.useQuery("get", "/swagger/Users/me", {
    params: { query: { includeAssignedLocations: true } },
  });

  const [locationId, setLocationId] = useState<number | null>(null);

  // Set default location
  useEffect(() => {
    const defaultLocationId = me.data?.assignedLocations?.[0]?.id ?? null;
    if (defaultLocationId) {
      setLocationId(defaultLocationId);
    }
  }, [me.data]);

   const Report = $api.useQuery("get", "/swagger/Reports/order/dailyReport", {
    params: { query: { reportDate: formattedDate, locationId: parseInt(locationId?.toString() ?? "0") }},
  });
  const [pageNumber] = useState(0);
    const [rowsPerPage] = useState(10);
    const [emailFilter] = useState("");
    const [nameFilter] = useState("");
    const [debouncedEmailFilter] = useDebounce(emailFilter, 500);
    const [debouncedNameFilter] = useDebounce(nameFilter, 500);
    const [statusFilter] = useState<number>(-1);

    const redCodeRequests = $api.useQuery("get", "/swagger/RedCodes/requests", {
      params: {
        query: {
          pageNumber: pageNumber + 1,
          pageSize: rowsPerPage,
          email: debouncedEmailFilter,
          name: debouncedNameFilter,
          status: statusFilter === -1 ? null : statusFilter,
          ReportDate: formattedDate,
          LocationId: parseInt(locationId?.toString() ?? "0")
        },
      },
    });
const approvedCount = redCodeRequests.data?.items.filter(
  (r) => r.status === RedCodeRequestStatus.Approved
).reduce((sum, r) => sum + (r.orderNumbers ?? 0), 0) ?? 0;

const totalCount = redCodeRequests.data?.items.reduce((sum, r) => sum + (r.orderNumbers ?? 0), 0) ?? 0;

  // Trích xuất dữ liệu đúng key từ API
  const dailyOrders = Report.data?.dailyOrders ?? [];

  if (me.error || Report.error)
    return (
      <ErrorMessage
        error={{
          me: me.error,
          Report: Report.error,
        }}
      />
    );

  return (
    <Box p={4}>
      {(me.isLoading || Report.isLoading) && <FullscreenLoading />}
      <Typography variant="h4" component="h1" gutterBottom align="center">
        <strong>Orders Report Dashboard</strong>
      </Typography>
      <Grid container spacing={3} alignItems="center">
  {/* Location Dropdown */}
  <Grid item xs={12} md={3}>
    <TextField
      fullWidth
      label="Location"
      value={locationId ?? ""}
      onChange={(e) => setLocationId(parseInt(e.target.value))}
      variant="outlined"
      size="small"
      select
    >
      {me.data?.assignedLocations?.map((l) => (
        <MenuItem key={l.id} value={l.id}>
          {l.name}
        </MenuItem>
      ))}
    </TextField>
  </Grid>

  {/* Date Picker */}
  <Grid item xs={12} md={3}>
    <LocalizationProvider dateAdapter={AdapterDayjs}>
      <DatePicker
        label="Date"
        value={selectedDate}
        onChange={(newValue) => {
          if (newValue !== null) {
            setSelectedDate(newValue);
          }
        }}
        format="DD/MM/YYYY"
        slots={{
          textField: (props) => <TextField {...props} fullWidth size="small" />,
        }}
      />
    </LocalizationProvider>
  </Grid>
</Grid>
    {isAfterReportTime ? (
  <>
    {/* Tổng số đơn đặt trước */}
      {Report.data?.numberOfPreOrder != null && (
        <Typography variant="h6" sx={{ mt: 4 }}>
          Tổng số đơn đặt trước: <strong>{Report.data.numberOfPreOrder}</strong>
        </Typography>
      )}

      {/* Bảng món ăn */}
      {dailyOrders.length > 0 ? (
        <Box mt={4}>
          <Typography variant="h6" gutterBottom>
            Chi tiết đơn hàng theo món
          </Typography>
          <Box sx={{ overflowX: "auto" }}>
            <table style={{ width: "100%", borderCollapse: "collapse" }}>
              <thead>
                <tr style={{ backgroundColor: "#f5f5f5" }}>
                  <th style={cellStyle}>Date</th>
                  <th style={cellStyle}>Dish</th>
                  <th style={cellStyle}>Shift</th>
                  <th style={cellStyle}>Orders Number</th>
                </tr>
              </thead>
              <tbody>
                {dailyOrders.map((item, index) => (
                  <tr key={index}>
                    <td style={cellStyle}>{item.date}</td>
                    <td style={cellStyle}>{item.dish}</td>
                    <td style={cellStyle}>{item.shift}</td>
                    <td style={cellStyle}>{item.ordersNumber}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </Box>
        </Box>
      ) : (
        <Typography variant="body1" sx={{ mt: 2 }}>
          Không có dữ liệu món ăn cho ngày: {formattedDate}.
        </Typography>
      )}


      {(
        <Box mt={4}>
          <Typography variant="h6" gutterBottom>
            <Typography variant="h6">
            Danh sách red code request trong ngày:{" "}
            <span style={{ color: "green", fontWeight: "bold" }}>
              ({approvedCount} Approved
            </span>
            /
            <span style={{ color: "Black", fontWeight: "bold" }}>
              {totalCount})
            </span>
          </Typography>
          </Typography>
          <Box sx={{ overflowX: "auto" }}>
            <table style={{ width: "100%", borderCollapse: "collapse" }}>
              <thead>
                <tr style={{ backgroundColor: "#f5f5f5" }}>
                  <th style={cellStyle}>Submitted Date</th>
                  <th style={cellStyle}>Request By</th>
                  <th style={cellStyle}>Number of Orders</th>
                  <th style={cellStyle}>Location</th>
                  <th style={cellStyle}>Check In Date</th>
                  <th style={cellStyle}>Status</th>
                  
                </tr>
              </thead>
              <tbody>
                {redCodeRequests.data?.items.map((item, index) => (
                  <tr key={index}>
                    <td style={cellStyle}>{dayjs(item.created).format("DD/MM/YYYY HH:mm")}</td>
                    <td style={cellStyle}>{item.fullName}<br/> ({item.workEmail})</td>
                    <td style={cellStyle}>{item.orderNumbers}</td>
                    <td style={cellStyle}>{item.workLocationName}</td>
                    <td style={cellStyle}>{item.checkInDate}</td>
                    <td style={cellStyle}> <Typography
                    sx={{
                      color:
                        RedCodeRequestStatus[item.status] === "Submitted"
                          ? "black"
                          : RedCodeRequestStatus[item.status] === "Approved"
                          ? "green"
                          : "red",
                    }}
                  >
                    {RedCodeRequestStatus[item.status]}
                  </Typography></td>
                    
                  </tr>
                ))}
              </tbody>
            </table>
          </Box>
        </Box>
      )}
  </>
) : (
  <Typography variant="body1" sx={{ mt: 4 }}>
    Báo cáo chỉ khả dụng sau 16:00 ngày {selectedDate.subtract(1, "day").format("DD/MM/YYYY")}.
  </Typography>
)}
    </Box>
  );
}