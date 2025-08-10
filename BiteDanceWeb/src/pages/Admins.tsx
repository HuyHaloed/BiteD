import { useState } from "react";
import {
  Container,
  Typography,
  TextField,
  Grid,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Button,
  Avatar,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Box,
} from "@mui/material";
import LocationOnIcon from "@mui/icons-material/LocationOn";
import { useConfirm } from "material-ui-confirm";
import { useNavigate } from "react-router-dom";
import $api from "../services/api";
import { components } from "../services/api.openapi";
import { toShortName } from "../utils";

export default function Admins() {
  const admins = $api.useQuery("get", "/swagger/Users/admins", {
    query: { includeAssignedLocations: true },
  });
  const locations = $api.useQuery("get", "/swagger/Locations");
  const deactivateAdmin = $api.useMutation(
    "post",
    "/swagger/Users/admins/{email}/deactivate"
  );
  const confirm = useConfirm();
  const navigate = useNavigate();
  const [filter, setFilter] = useState<string>("");
  const [selectedLocation, setSelectedLocation] = useState<string>("");

  let filteredData = admins.data;
  if (filter !== "") {
    filteredData = admins.data?.filter((d) =>
      d.name?.toLowerCase().includes(filter)
    );
  }
  if (selectedLocation !== "") {
    filteredData = filteredData?.filter((d) =>
      d.assignedLocations.some((l) => l.id === parseInt(selectedLocation))
    );
  }

  function handleDeactivateAdmin(admin: components["schemas"]["AdminDto"]) {
    const locationsWithOnlyOneAdmin = admin.assignedLocations.filter(
      (l) => l.admins.length === 1
    );
    if (locationsWithOnlyOneAdmin.length == 0) {
      confirm({
        title: "Are you sure you want to deactivate this admin?",
        description: "This action cannot be undone.",
      }).then(() => {
        deactivateAdmin.mutate({ params: { path: { email: admin.email } } });
      });
    } else {
      confirm({
        confirmationText: "Go to location",
        title: "Cannot deactivate admin",
        description:
          "The following location(s) will NOT have any admin in charge if this admin is deactivated: " +
          locationsWithOnlyOneAdmin.map((l) => l.name).join(", "),
      }).then(() => {
        navigate("/location/" + locationsWithOnlyOneAdmin[0].id);
      });
    }
  }

  return (
    <Container>
      <Box my={4}>
        <Typography variant="h4" component="h1" gutterBottom align="center">
          Manage <strong>Admins</strong>
        </Typography>
      </Box>
      <Grid
        container
        spacing={2}
        alignItems="center"
        style={{ marginBottom: 20 }}
      >
        <Grid item xs={12} sm={6} md={4}>
          <TextField
            fullWidth
            label="Search"
            variant="outlined"
            size="small"
            value={filter}
            onChange={(e) => setFilter(e.target.value)}
          />
        </Grid>
        <Grid item xs={12} sm={6} md={4}>
          <FormControl fullWidth variant="outlined" size="small">
            <InputLabel>Location</InputLabel>
            <Select
              label="Location"
              value={selectedLocation}
              onChange={(e) => setSelectedLocation(e.target.value)}
            >
              <MenuItem value="">
                <em>None</em>
              </MenuItem>
              {locations.data?.map((location) => (
                <MenuItem key={location.id} value={location.id}>
                  {location.name}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </Grid>
      </Grid>
      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>User</TableCell>
              <TableCell>Work Email</TableCell>
              <TableCell>In Charge Locations</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {filteredData?.map((admin) => (
              <TableRow key={admin.id}>
                <TableCell>
                  <Grid container alignItems="center" spacing={2}>
                    <Grid item>
                      <Avatar>{toShortName(admin.name)}</Avatar>
                    </Grid>
                    <Grid item>{admin.name}</Grid>
                  </Grid>
                </TableCell>
                <TableCell>{admin.email}</TableCell>
                <TableCell>
                  <Grid container alignItems="center" spacing={1}>
                    <Grid item>
                      <LocationOnIcon fontSize="small" />
                    </Grid>
                    <Grid item>
                      {admin.assignedLocations
                        .map((loc) => loc.name)
                        .join(", ")}
                    </Grid>
                  </Grid>
                </TableCell>
                <TableCell>
                  <Button
                    variant="contained"
                    color="error"
                    disabled={deactivateAdmin.isPending}
                    onClick={() => handleDeactivateAdmin(admin)}
                  >
                    DEACTIVATE
                  </Button>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </Container>
  );
}
