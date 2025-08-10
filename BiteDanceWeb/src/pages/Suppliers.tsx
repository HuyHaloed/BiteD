import { useState } from "react";
import { NavLink, useNavigate } from "react-router-dom";
import {
  Box,
  Button,
  Card,
  CardContent,
  IconButton,
  InputBase,
  Paper,
  Typography,
  Avatar,
  Grid,
} from "@mui/material";
import SearchIcon from "@mui/icons-material/Search";
import ArrowForwardIcon from "@mui/icons-material/ArrowForward";
import $api from "../services/api";
import ErrorMessage from "../ui/ErrorMessage";
import FullscreenLoading from "../ui/FullscreenLoading";
import Map from "../ui/Map";
import AddIcon from "@mui/icons-material/Add";
import { toShortName } from "../utils";

export default function Suppliers() {
  const suppliers = $api.useQuery("get", "/swagger/Suppliers");
  const [filter, setFilter] = useState<string>("");
  const navigate = useNavigate();

  let filteredData = suppliers.data;
  if (filter !== "") {
    filteredData = suppliers.data?.filter((d) =>
      d.name?.toLowerCase().includes(filter)
    );
  }

  if (suppliers.isLoading) {
    return <FullscreenLoading />;
  }

  if (suppliers.error) {
    return <ErrorMessage error={suppliers.error} />;
  }

  return (
    <Grid container sx={{ height: "85vh" }}>
      {/* Left Side - Map */}
      <Grid item xs={12} md={6}>
        <Map />
      </Grid>

      {/* Right Side - Details */}
      <Grid
        item
        xs={12}
        md={6}
        sx={{
          p: 2,
        }}
      >
        <Grid
          container
          justifyContent="space-between"
          alignItems="center"
          mb={2}
        >
          <Button
            variant="contained"
            color="success"
            onClick={() => navigate("/new-supplier")}
            startIcon={<AddIcon />}
          >
            ADD NEW SUPPLIER
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
              No suppliers found
            </Typography>
          ) : (
            filteredData?.map((s) => (
              <Card
                variant="outlined"
                sx={{ mb: 2, borderRadius: "16px" }}
                key={s.id}
              >
                <CardContent sx={{ display: "flex", alignItems: "center" }}>
                  <Avatar
                    sx={{
                      width: 56,
                      height: 56,
                      mr: 2,
                      bgcolor: s.isActive ? "primary.main" : "error.main",
                    }}
                  >
                    {toShortName(s.name)}
                  </Avatar>
                  <Box sx={{ flexGrow: 1 }}>
                    <Grid container spacing={1}>
                      <Grid item xs={12}>
                        <Typography variant="body2">Supplier Name</Typography>
                        <Typography variant="body1" fontWeight="bold">
                          {s.name}
                        </Typography>
                      </Grid>
                      <Grid item xs={12}>
                        <Typography variant="body2">
                          Assigned locations
                        </Typography>
                        <Typography variant="body1" fontWeight="bold">
                          {s.assignedLocations.map((a) => a.name).join(", ")}
                        </Typography>
                      </Grid>
                    </Grid>
                  </Box>
                  <IconButton component={NavLink} to={`/supplier/${s.id}`}>
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
