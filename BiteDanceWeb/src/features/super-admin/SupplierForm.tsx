import {
  Grid,
  Button,
  Typography,
  Avatar,
  Stack,
  Chip,
  Box,
} from "@mui/material";
import {
  FormContainer,
  TextFieldElement,
  MultiSelectElement,
  SelectElement,
} from "react-hook-form-mui";
import { DatePickerElement } from "react-hook-form-mui/date-pickers";
import { components } from "../../services/api.openapi";
import $api from "../../services/api";
import { useConfirm } from "material-ui-confirm";
import dayjs from "dayjs";
import toast from "react-hot-toast";
import { useNavigate } from "react-router-dom";
import { toShortName } from "../../utils";
import FullscreenLoading from "../../ui/FullscreenLoading";
import ErrorMessage from "../../ui/ErrorMessage";
import { ArrowBack, Cancel, Save } from "@mui/icons-material";

export default function SupplierForm({
  supplier,
}: {
  supplier: components["schemas"]["SupplierDto"] | undefined;
}) {
  const confirm = useConfirm();
  const navigate = useNavigate();
  // APIs
  const locations = $api.useQuery("get", "/swagger/Locations");
  const systemConfigs = $api.useQuery("get", "/swagger/System/configs");
  const createSupplier = $api.useMutation("post", "/swagger/Suppliers", {
    onSuccess: () => {
      toast.success("Supplier created successfully!");
      navigate("/suppliers");
    },
  });
  const updateSupplier = $api.useMutation("put", "/swagger/Suppliers/{id}", {
    onSuccess: () => toast.success("Supplier updated successfully!"),
  });
  const activateSupplier = $api.useMutation(
    "post",
    "/swagger/Suppliers/{id}/activate",
    {
      onSuccess: () => toast.success("Supplier activated successfully!"),
    }
  );
  const deactivateSupplier = $api.useMutation(
    "post",
    "/swagger/Suppliers/{id}/deactivate",
    {
      onSuccess: () => toast.success("Supplier deactivated successfully!"),
    }
  );

  const isEditMode = supplier !== undefined;
  const isLoading = locations.isLoading || systemConfigs.isLoading;
  const isError = locations.error || systemConfigs.error;
  const isPending =
    createSupplier.isPending ||
    updateSupplier.isPending ||
    activateSupplier.isPending ||
    deactivateSupplier.isPending;

  function action(
    obj:
      | components["schemas"]["CreateSupplierCommand"]
      | components["schemas"]["UpdateSupplierCommand"]
  ) {
    console.log(obj);
    const payload = {
      ...obj,
      country: "Viet Nam", // TODO: hardcoded country
      // @ts-expect-error C# api
      contractEndDate: obj.contractEndDate.format("YYYY-MM-DD"),
      // @ts-expect-error C# api
      contractStartDate: obj.contractStartDate.format("YYYY-MM-DD"),
    };
    if (isEditMode) {
      updateSupplier.mutate({
        params: { path: { id: supplier?.id } },
        // @ts-expect-error C# api
        body: payload,
      });
    } else {
      createSupplier.mutate({
        body: payload,
      });
    }
  }

  const handleActivate = () => {
    confirm({ description: "Are you sure you want to activate this supplier?" })
      .then(() => {
        if (supplier?.id === undefined) return;
        activateSupplier.mutate({
          params: { path: { id: supplier?.id } },
        });
      })
      .catch(() => {});
  };

  const handleDeactivate = () => {
    confirm({
      description: "Are you sure you want to deactivate this supplier?",
    })
      .then(() => {
        if (supplier?.id === undefined) return;
        deactivateSupplier.mutate({
          params: { path: { id: supplier?.id } },
        });
      })
      .catch(() => {});
  };

  if (isLoading) return <FullscreenLoading></FullscreenLoading>;
  if (isError)
    return (
      <ErrorMessage
        error={{
          locations: locations.error,
          systemConfigs: systemConfigs.error,
        }}
      ></ErrorMessage>
    );

  return (
    <Box padding={2}>
      <Button
        onClick={() => navigate("/suppliers")}
        startIcon={<ArrowBack />}
        sx={{ marginBottom: 2, marginLeft: 3 }}
      >
        Back
      </Button>
      <Grid container spacing={3}>
        <Grid item xs={12} md={2}>
          {/* Avatar / Activate status column */}
          <Stack spacing={2} alignItems="center">
            <Avatar sx={{ width: 150, height: 150 }}>
              {toShortName(supplier?.name)}
            </Avatar>
            {isEditMode && (
              <>
                <Chip
                  sx={{
                    backgroundColor: supplier?.isActive
                      ? "primary.main"
                      : "error.main",
                    color: "white",
                  }}
                  label={`Status: ${
                    supplier?.isActive ? "Active" : "Inactive"
                  }`}
                />
                <Button
                  variant="outlined"
                  onClick={
                    supplier?.isActive ? handleDeactivate : handleActivate
                  }
                  disabled={isPending}
                >
                  {supplier?.isActive ? "DEACTIVATE" : "ACTIVATE"}
                </Button>
              </>
            )}
          </Stack>
        </Grid>
        {/* Form column */}
        {/* Supplier information section */}
        <Grid item xs={12} md={10}>
          <Typography variant="h6" marginBottom={2}>
            SUPPLIER INFORMATION
          </Typography>

          <FormContainer
            // @ts-expect-error C# api
            defaultValues={
              supplier
                ? {
                    ...supplier,
                    locationIds: supplier.assignedLocations?.map((l) => l.id),
                    contractEndDate: dayjs(supplier.contractEndDate),
                    contractStartDate: dayjs(supplier.contractStartDate),
                  }
                : {}
            }
            onSuccess={action}
            disabled={isPending}
          >
            <Grid container spacing={2}>
              <Grid item xs={12} md={6}>
                <TextFieldElement name="name" label="Name" fullWidth required />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextFieldElement
                  name="address"
                  label="Address"
                  fullWidth
                  required
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextFieldElement
                  name="certificateOfBusinessNumber"
                  label="Certificate of business number"
                  fullWidth
                  required
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <SelectElement
                  name="baseLocation"
                  label="Base location"
                  options={systemConfigs.data?.allowedCities.map((city) => ({
                    id: city,
                    label: city,
                  }))}
                  fullWidth
                  required
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <DatePickerElement
                  sx={{ width: "100%" }}
                  name="contractStartDate"
                  label="Contract start date"
                  required
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <DatePickerElement
                  sx={{ width: "100%" }}
                  name="contractEndDate"
                  label="Contract end date"
                  required
                />
              </Grid>

              {/* Supplier contact section */}
              <Grid item xs={12}>
                <Typography variant="h6">SUPPLIER CONTACT</Typography>
              </Grid>
              <Grid item xs={12} md={6}>
                <TextFieldElement
                  name="email"
                  label="Email"
                  fullWidth
                  required
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextFieldElement
                  name="phoneNumber"
                  label="Phone number"
                  fullWidth
                  required
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextFieldElement
                  name="picName"
                  label="Person In-charge Name"
                  fullWidth
                  required
                />
              </Grid>
              <Grid item xs={12} md={6}>
                <TextFieldElement
                  name="picPhoneNumber"
                  label="Person In-charge Phone Number"
                  fullWidth
                  required
                />
              </Grid>

              {/* Supplier assigned location section */}
              <Grid item xs={12}>
                <Typography variant="h6">SUPPLIER ASSIGNED LOCATION</Typography>
              </Grid>
              <Grid item xs={12}>
                <MultiSelectElement
                  label="Locations"
                  name="locationIds"
                  showChips
                  itemKey="id"
                  itemLabel="label"
                  options={
                    locations.data?.map((l) => ({
                      id: l.id,
                      label: l.name,
                    })) ?? []
                  }
                  fullWidth
                />
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
                onClick={() => navigate("/suppliers")}
                startIcon={<Cancel />}
              >
                CANCEL
              </Button>
              <Button
                type="submit"
                variant="contained"
                size="large"
                disabled={isPending}
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
}
