import { useState, useEffect } from "react";
import {
  Box,
  Button,
  Grid,
  Typography,
  Avatar,
  Chip,
  TextField,
  Stack,
} from "@mui/material";
import {
  CheckboxElement,
  FormContainer,
  SelectElement,
  TextFieldElement,
} from "react-hook-form-mui";
import { components, LocationType } from "../../services/api.openapi";
import $api, { $fetchClient } from "../../services/api";
import toast from "react-hot-toast";
import { useConfirm } from "material-ui-confirm";
import { getEnumValues, toShortName } from "../../utils";
import ErrorMessage from "../../ui/ErrorMessage";
import FullscreenLoading from "../../ui/FullscreenLoading";
import { ArrowBack, Cancel, Save } from "@mui/icons-material";
import { useNavigate } from "react-router-dom";

const extractAdminEmails = (
  admins: components["schemas"]["AdminDto"][] | undefined
) => admins?.map((a) => a.email ?? "") ?? [];

const LocationScreen = ({
  location,
}: {
  location: components["schemas"]["LocationDto"] | undefined;
}) => {
  const confirm = useConfirm();
  const navigate = useNavigate();
  // APIs
  const suppliers = $api.useQuery("get", "/swagger/Suppliers");
  const systemConfigs = $api.useQuery("get", "/swagger/System/configs");
  const createLocation = $api.useMutation("post", "/swagger/Locations", {
    onSuccess: () => {
      toast.success("Location created successfully!");
      navigate("/locations");
    },
  });
  const updateLocation = $api.useMutation("put", "/swagger/Locations/{id}", {
    onSuccess: () => toast.success("Location updated successfully!"),
  });
  const activateLocation = $api.useMutation(
    "post",
    "/swagger/Locations/{id}/activate",
    {
      onSuccess: () => toast.success("Location activated successfully!"),
    }
  );
  const deactivateLocation = $api.useMutation(
    "post",
    "/swagger/Locations/{id}/deactivate",
    {
      onSuccess: () => toast.success("Location deactivated successfully!"),
    }
  );

  const isLoading = suppliers.isLoading || systemConfigs.isLoading;
  const isError = suppliers.error || systemConfigs.error;
  const isPending =
    createLocation.isPending ||
    updateLocation.isPending ||
    activateLocation.isPending ||
    deactivateLocation.isPending;
  // Assign admin states
  const [adminEmail, setAdminEmail] = useState<string>("");
  const [adminList, setAdminList] = useState<string[]>(
    extractAdminEmails(location?.admins)
  );

  useEffect(() => {
    setAdminList(extractAdminEmails(location?.admins));
  }, [location?.admins]);

  const isEditMode = location !== undefined;

  function handleDeleteAdmin(adminToDelete: string) {
    setAdminList((admins) => admins.filter((admin) => admin !== adminToDelete));
  }

  async function handleAddAdmin() {
    if (
      adminList.map((a) => a.toLowerCase()).includes(adminEmail.toLowerCase())
    ) {
      alert("admin already assigned");
      return;
    }

    const user = await $fetchClient.GET("/swagger/Users/{email}", {
      params: { path: { email: adminEmail } },
    });
    if (user.error) {
      alert("User not found");
      return;
    }
    setAdminList((v) => [...v, adminEmail]);
    setAdminEmail("");
  }

  function handleFormSubmit(
    obj:
      | components["schemas"]["CreateLocationCommand"]
      | components["schemas"]["UpdateLocationCommand"]
  ) {
    console.log(obj);
    if (isEditMode) {
      if (location?.id === undefined) return;
      updateLocation.mutate({
        params: { path: { id: location.id } },
        body: {
          ...obj,
          id: location.id,
          adminEmails: adminList,
          type: Number(obj.type),
        },
      });
    } else {
      createLocation.mutate({
        body: { ...obj, adminEmails: adminList, type: Number(obj.type) },
      });
    }
  }

  function handleActivate() {
    confirm({ description: "Are you sure you want to activate this location?" })
      .then(() => {
        if (location?.id === undefined) return;
        activateLocation.mutate({
          params: { path: { id: location.id } },
        });
      })
      .catch(() => {});
  }

  function handleDeactivate() {
    confirm({
      description: "Are you sure you want to deactivate this location?",
    })
      .then(() => {
        if (location?.id === undefined) return;
        deactivateLocation.mutate({
          params: { path: { id: location.id } },
        });
      })
      .catch(() => {});
  }

  if (isLoading) return <FullscreenLoading />;
  if (isError)
    return (
      <ErrorMessage
        error={{
          supplier: suppliers.error,
          systemConfigs: systemConfigs.error,
        }}
      />
    );

  return (
    <Box padding={2}>
      <Button
        onClick={() => navigate("/locations")}
        startIcon={<ArrowBack />}
        sx={{ marginBottom: 2, marginLeft: 3 }}
      >
        Back
      </Button>
      <Grid container spacing={3}>
        {/* Avatar / Activate status column */}
        <Grid item xs={12} md={2}>
          <Stack spacing={2} alignItems="center">
            <Avatar sx={{ width: 150, height: 150 }}>
              {toShortName(location?.name)}
            </Avatar>
            {isEditMode && (
              <>
                <Chip
                  sx={{
                    backgroundColor: location?.isActive
                      ? "primary.main"
                      : "error.main",
                    color: "white",
                  }}
                  label={`Status: ${
                    location?.isActive ? "Active" : "Inactive"
                  }`}
                />
                <Button
                  variant="outlined"
                  onClick={
                    location?.isActive ? handleDeactivate : handleActivate
                  }
                  disabled={isPending}
                >
                  {location?.isActive ? "DEACTIVATE" : "ACTIVATE"}
                </Button>
              </>
            )}
          </Stack>
        </Grid>
        {/* Form column */}
        {/* Location information section */}
        <Grid item xs={12} md={10}>
          <Typography variant="h6" marginBottom={2}>
            LOCATION INFORMATION
          </Typography>
          <FormContainer
            defaultValues={location ?? {}}
            onSuccess={handleFormSubmit}
            disabled={isPending}
          >
            <Grid container spacing={2}>
              <Grid item xs={6}>
                <TextFieldElement fullWidth name="name" label="Name" required />
              </Grid>
              <Grid item xs={6}>
                <SelectElement
                  fullWidth
                  name="country"
                  label="Country"
                  required
                  options={systemConfigs.data?.allowedCountries.map(
                    (country) => ({
                      id: country,
                      label: country,
                    })
                  )}
                />
              </Grid>
              <Grid item xs={6}>
                <SelectElement
                  fullWidth
                  label="Type"
                  name="type"
                  required
                  options={getEnumValues(LocationType).map(
                    ({ key, value }) => ({
                      id: key,
                      label: value,
                    })
                  )}
                />
              </Grid>
              <Grid item xs={6}>
                <SelectElement
                  fullWidth
                  name="city"
                  label="City"
                  required
                  options={systemConfigs.data?.allowedCities.map((city) => ({
                    id: city,
                    label: city,
                  }))}
                />
              </Grid>
              <Grid item xs={12}>
                <TextFieldElement
                  fullWidth
                  name="description"
                  label="Description"
                  required
                  multiline
                  rows={4}
                />
              </Grid>
            </Grid>
            <Grid container spacing={2} marginTop={5}>
              {/* Service time section */}
              <Grid item xs={12} md={6}>
                <Typography variant="h6" marginBottom={2}>
                  SERVICE TIME
                </Typography>
                <Grid container spacing={2}>
                  <Grid item xs={6}>
                    <Typography variant="subtitle1">Shift</Typography>
                    <CheckboxElement name="enableShift1" label="Shift 1" />
                    <CheckboxElement name="enableShift2" label="Shift 2" />
                    <CheckboxElement name="enableShift3" label="Shift 3" />
                  </Grid>
                  <Grid item xs={6}>
                    <Typography variant="subtitle1">Day</Typography>
                    <CheckboxElement
                      name="enableWeekday"
                      label="Weekday (Mon - Fri)"
                    />
                    <CheckboxElement
                      name="enableWeekend"
                      label="Weekend (Sat - Sun)"
                    />
                  </Grid>
                </Grid>
              </Grid>

              {/* Administration section */}
              <Grid item xs={12} md={6}>
                <Typography variant="h6" marginBottom={2}>
                  ADMINISTRATION
                </Typography>
                <SelectElement
                  fullWidth
                  label="Supplier"
                  name="supplierId"
                  options={suppliers.data?.map((s) => ({
                    id: s.id,
                    label: s.name,
                  }))}
                />
                <Grid container spacing={1} alignItems="center" marginTop={2}>
                  <Grid item xs={9}>
                    <TextField
                      fullWidth
                      label="Admin Email"
                      value={adminEmail}
                      onChange={(e) => setAdminEmail(e.target.value)}
                      placeholder="Enter admin email"
                      type="email"
                    />
                  </Grid>
                  <Grid item xs={3}>
                    <Button
                      variant="contained"
                      onClick={handleAddAdmin}
                      disabled={!adminEmail.trim()}
                    >
                      Assign
                    </Button>
                  </Grid>
                </Grid>
                <Grid container spacing={1} flexWrap="wrap" marginTop={2}>
                  {adminList.map((admin, index) => (
                    <Grid item key={index}>
                      <Chip
                        label={admin}
                        onDelete={() => handleDeleteAdmin(admin)}
                      />
                    </Grid>
                  ))}
                </Grid>
              </Grid>
            </Grid>
            <Box
              sx={{
                display: "flex",
                justifyContent: "flex-end",
                marginTop: 3,
                gap: 2,
              }}
            >
              <Button
                type="submit"
                variant="outlined"
                size="large"
                disabled={isPending}
                onClick={() => navigate("/locations")}
                startIcon={<Cancel />}
              >
                CANCEL
              </Button>
              <Button
                type="submit"
                variant="contained"
                size="large"
                disabled={isPending}
                sx={{ backgroundColor: "green", color: "white" }}
                startIcon={<Save />}
              >
                SAVE
              </Button>
            </Box>
          </FormContainer>
        </Grid>
      </Grid>
    </Box>
  );
};

export default LocationScreen;
