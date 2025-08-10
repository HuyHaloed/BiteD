import { useEffect } from "react";
import {
  Box,
  Grid,
  TextField,
  Typography,
  Card,
  CardContent,
  MenuItem,
} from "@mui/material";
import { useState } from "react";
import $api from "../services/api";
import FullscreenLoading from "../ui/FullscreenLoading";
import ErrorMessage from "../ui/ErrorMessage";

function InfoCard({
  title,
  value,
  color,
}: {
  title: string;
  value: string;
  color: string;
}) {
  return (
    <Grid item xs={12} md={3}>
      <Card>
        <CardContent style={{ textAlign: "center" }}>
          <Typography variant="subtitle1" gutterBottom fontWeight="bold">
            {title}
          </Typography>
          <Typography
            variant="h4"
            component="div"
            color={color}
            fontWeight="bold"
          >
            {value}
          </Typography>
        </CardContent>
      </Card>
    </Grid>
  );
}

export default function RedCodeRequestsSummary() {
  const me = $api.useQuery("get", "/swagger/Users/me", {
    params: { query: { includeAssignedLocations: true } },
  });
  const [locationId, setLocationId] = useState<number | null>(null);
  const redCodeRequestsSummary = $api.useQuery(
    "get",
    "/swagger/RedCodes/requests/{locationId}/summary",
    {
      params: { path: { locationId: locationId! } },
    },
    {
      enabled: locationId !== null,
    }
  );

  // Set default location
  useEffect(() => {
    const defaultLocationId = me.data?.assignedLocations?.[0]?.id ?? null;
    if (defaultLocationId) {
      setLocationId(defaultLocationId);
    }
  }, [me.data]);

  if (me.error || redCodeRequestsSummary.error)
    return (
      <ErrorMessage
        error={{
          me: me.error,
          redCodeRequestsSummary: redCodeRequestsSummary.error,
        }}
      />
    );

  return (
    <Box p={4}>
      {(me.isLoading || redCodeRequestsSummary.isLoading) && (
        <FullscreenLoading />
      )}
      <Typography variant="h4" component="h1" gutterBottom align="center">
        External Request <strong>Dashboard</strong>
      </Typography>
      <Grid container spacing={3} alignItems="center">
        <Grid item xs={12} md={2}>
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
      </Grid>
      {redCodeRequestsSummary.data ? (
        <Grid container spacing={3} mt={3}>
          <InfoCard
            title="Total External Requests"
            value={redCodeRequestsSummary.data.totalRequests.toString() ?? "NA"}
            color="success.main"
          />
          {Object.entries(
            redCodeRequestsSummary.data.requestsByStatus ?? {}
          ).map(([status, count]) => {
            return (
              <InfoCard
                key={status}
                title={`${status} Requests`}
                value={count.toString()}
                color="success.main"
              />
            );
          })}
        </Grid>
      ) : null}
    </Box>
  );
}
