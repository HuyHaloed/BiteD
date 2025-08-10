import {
  Box,
  Button,
  Card,
  CardContent,
  Grid,
  IconButton,
  InputBase,
  Paper,
  Typography,
  Avatar,
} from "@mui/material";
import SearchIcon from "@mui/icons-material/Search";
import ArrowForwardIcon from "@mui/icons-material/ArrowForward";
import { NavLink, useNavigate } from "react-router-dom";
import { useState } from "react";
import $api from "../services/api";
import { LocationType } from "../services/api.openapi";
import ErrorMessage from "../ui/ErrorMessage";
import FullscreenLoading from "../ui/FullscreenLoading";
import Map from "../ui/Map";
import AddIcon from "@mui/icons-material/Add";
import { toShortName } from "../utils";

function Locations() {
  const navigate = useNavigate();
  const locations = $api.useQuery("get", "/swagger/Locations");
  const [filter, setFilter] = useState<string>("");

  if (locations.isLoading) {
    return <FullscreenLoading />;
  }

  if (locations.error) {
    return <ErrorMessage error={locations.error} />;
  }

  let filteredData = locations.data;

  if (filter !== "") {
    filteredData = locations.data?.filter((d) =>
      d.name?.toLowerCase().includes(filter)
    );
  }

  return (
    <Grid container sx={{ height: "85vh" }}>
      {/* Left Side - Map */}
      <Grid item xs={12} md={6}>
        <Map />
      </Grid>

      {/* Right Side - Details */}
      <Grid item xs={12} md={6} sx={{ p: 2 }}>
        <Grid
          container
          justifyContent="space-between"
          alignItems="center"
          mb={2}
        >
          <Button
            variant="contained"
            color="success"
            onClick={() => navigate("/new-location")}
            startIcon={<AddIcon />}
          >
            ADD NEW LOCATION
          </Button>
          <Paper
            component="form"
            sx={{
              display: "flex",
              alignItems: "center",
              width: 400,
              borderRadius: "10px",
            }}
          >
            <InputBase
              sx={{ ml: 1, flex: 1, borderRadius: "10px" }}
              placeholder="Search..."
              onChange={(e) => setFilter(e.target.value)}
              value={filter}
            />
            <IconButton type="submit" sx={{ p: "10px" }}>
              <SearchIcon />
            </IconButton>
          </Paper>
        </Grid>

        <Box sx={{ height: "calc(85vh - 64px)", overflowY: "auto" }}>
          {filteredData?.length === 0 ? (
            <Typography variant="body1" textAlign="center" marginTop={5}>
              No locations found
            </Typography>
          ) : (
            filteredData?.map((l) => (
              <Card
                variant="outlined"
                sx={{ mb: 2, borderRadius: "16px" }}
                key={l.id}
              >
                <CardContent sx={{ display: "flex", alignItems: "center" }}>
                  <Avatar
                    sx={{
                      width: 56,
                      height: 56,
                      mr: 2,
                      bgcolor: l.isActive ? "primary.main" : "error.main",
                    }}
                  >
                    {toShortName(l.name)}
                  </Avatar>
                  <Box sx={{ flexGrow: 1 }}>
                    <Grid container spacing={1}>
                      <Grid item xs={6}>
                        <Typography variant="body2">Location Name</Typography>
                        <Typography variant="body1" fontWeight="bold">
                          {l.name}
                        </Typography>
                      </Grid>
                      <Grid item xs={6}>
                        <Typography variant="body2">Supplier</Typography>
                        <Typography variant="body1" fontWeight="bold">
                          {l.supplierName ?? "-"}
                        </Typography>
                      </Grid>
                      <Grid item xs={6}>
                        <Typography variant="body2">Location Type</Typography>
                        <Typography variant="body1" fontWeight="bold">
                          {LocationType[l.type]}
                        </Typography>
                      </Grid>
                      <Grid item xs={6}>
                        <Typography variant="body2">Admins</Typography>
                        <Typography variant="body1" fontWeight="bold">
                          {l.admins.length > 0
                            ? l.admins.map((a) => a.name).join(", ")
                            : "-"}
                        </Typography>
                      </Grid>
                    </Grid>
                  </Box>
                  <IconButton component={NavLink} to={`/location/${l.id}`}>
                    <ArrowForwardIcon />
                  </IconButton>
                </CardContent>
              </Card>
            ))
          )}
        </Box>
      </Grid>
    </Grid>
  );
}

export default Locations;
