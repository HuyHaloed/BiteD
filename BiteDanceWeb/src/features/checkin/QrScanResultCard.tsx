import { Grid, Paper, Typography } from "@mui/material";
import { CodeType, components } from "../../services/api.openapi";
import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import ErrorIcon from "@mui/icons-material/Error";
import WarningIcon from "@mui/icons-material/Warning";
import CancelIcon from "@mui/icons-material/Cancel";
import PersonIcon from "@mui/icons-material/Person";
import RestaurantMenuIcon from "@mui/icons-material/RestaurantMenu";
import InfoIcon from "@mui/icons-material/Info";
import AccessTimeIcon from "@mui/icons-material/AccessTime";
import { green, red, blue, orange, yellow } from "@mui/material/colors";
import dayjs from "dayjs";
import { ChatBubbleOutline, RuleOutlined } from "@mui/icons-material";

export default function QrScanResultCard({
  result,
}: {
  result: components["schemas"]["CheckinResult"];
}) {
  const getIcon = (codeType: CodeType) => {
    switch (codeType) {
      case CodeType.Green:
        return <RestaurantMenuIcon sx={{ fontSize: 60, color: green[500] }} />;
      case CodeType.Blue:
        return <PersonIcon sx={{ fontSize: 60, color: blue[500] }} />;
      case CodeType.Red:
        return <PersonIcon sx={{ fontSize: 60, color: red[500] }} />;
      default:
        return <InfoIcon sx={{ fontSize: 60 }} />;
    }
  };


  const getStatusInfo = () => {
    if (result.isSuccess) {
      return {
        borderColor: green[500],
        icon: <CheckCircleIcon color="success" />,
        text: "Success"
      };
    }


    
    switch (result.message) {
      case "Order has been canceled.":
        return {
          borderColor: red[500],
          icon: <CancelIcon sx={{ color: red[500] }} />,
          text: "Canceled"
        };

/*      case "Exceeded 1 blue checkins per day.":
        return {
          borderColor: blue[500],
          icon: <WarningIcon sx={{ color: blue[500] }} />,
          text: "Exceeded Blue Checkin"
        };*/
      case "Order has already been scanned.":
        return {
          borderColor: yellow[700],
          icon: <WarningIcon sx={{ color: orange[500] }} />,
          text: "Already Scanned"
        };
      default:
        return {
          borderColor: red[500],
          icon: <ErrorIcon color="error" />,
          text: "Failed"
        };
    }
  };

  const statusInfo = getStatusInfo();

  return (
    <Grid item xs={12} key={result.scannedAt}>
      <Paper
        variant="outlined"
        sx={{
          padding: 2,
          borderColor: statusInfo.borderColor,
          borderWidth: 3,
        }}
      >
        <Grid container spacing={2}>
          <Grid
            item
            xs={2}
            sx={{
              display: "flex",
              justifyContent: "center",
              alignItems: "center",
            }}
          >
            {getIcon(result.codeType)}
          </Grid>
          <Grid item xs={10}>
            <Grid container spacing={2}>
              {/* Name */}
              <Grid item xs={6}>
                <Typography
                  variant="body2"
                  sx={{ display: "flex", alignItems: "center" }}
                >
                  <PersonIcon sx={{ marginRight: 1 }} /> Employee Name
                </Typography>
                <Typography variant="h6">
                  {result.employeeName ?? "-"}
                </Typography>
              </Grid>
              {/* Status */}
              <Grid item xs={6}>
                <Typography
                  variant="body2"
                  sx={{ display: "flex", alignItems: "center" }}
                >
                  <RuleOutlined sx={{ marginRight: 1 }} /> Scan Status
                </Typography>
                <Typography
                  variant="h6"
                  sx={{ display: "flex", alignItems: "center" }}
                >
                  {statusInfo.icon} {statusInfo.text}
                </Typography>
              </Grid>
              {/* Dish Names */}
              <Grid item xs={6}>
                <Typography
                  variant="body2"
                  sx={{ display: "flex", alignItems: "center" }}
                >
                  <RestaurantMenuIcon sx={{ marginRight: 1 }} /> Dish Names
                </Typography>
                <Typography variant="h6">
                  {result.dishNames?.join(", ") ?? "-"}
                </Typography>
              </Grid>
              {/* Scanned at */}
              <Grid item xs={6}>
                <Typography
                  variant="body2"
                  sx={{ display: "flex", alignItems: "center" }}
                >
                  <AccessTimeIcon sx={{ marginRight: 1 }} /> Scanned at
                </Typography>
                <Typography variant="h6">
                  {dayjs(result.scannedAt).format("HH:mm:ss")}
                </Typography>
              </Grid>
            </Grid>
            {/* Message */}
            <Typography
              variant="body2"
              sx={{ display: "flex", alignItems: "center" }}
            >
              <ChatBubbleOutline sx={{ marginRight: 1 }} /> Message
            </Typography>
            <Typography variant="h6">{result.message}</Typography>
          </Grid>
        </Grid>
      </Paper>
    </Grid>
  );
}