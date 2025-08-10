import {
  Grid,
  Button,
  Typography,
  Avatar,
  Stack,
  TextField,
} from "@mui/material";
import {
  FormContainer,
  TextFieldElement,
  SelectElement,
  SwitchElement,
} from "react-hook-form-mui";
import FullscreenLoading from "../ui/FullscreenLoading";
import ErrorMessage from "../ui/ErrorMessage";
import $api from "../services/api";
import { components } from "../services/api.openapi";
import toast from "react-hot-toast";
import { toShortName } from "../utils";
import { Save } from "@mui/icons-material";

export default function UpdateProfile() {
  const me = $api.useQuery("get", "/swagger/Users/me");
  const locations = $api.useQuery("get", "/swagger/Locations");
  const updateProfile = $api.useMutation("put", "/swagger/Users/me");

  function action(obj: components["schemas"]["UpdateProfileCommand"]) {
    updateProfile.mutate(
      {
        body: obj,
      },
      { onSuccess: () => toast.success("Profile updated") }
    );
  }

  if (locations.isLoading || me.isLoading)
    return <FullscreenLoading></FullscreenLoading>;
  if (locations.error || me.error)
    return (
      <ErrorMessage
        error={{ location: locations.error, me: me.error }}
      ></ErrorMessage>
    );

  return (
    <Grid container spacing={3}>
      <Grid item xs={12} md={4}>
        <Stack spacing={2} alignItems="center">
          <Avatar sx={{ width: 150, height: 150 }}>
            {toShortName(me.data?.name)}
          </Avatar>
        </Stack>
      </Grid>

      <Grid item xs={12} md={8}>
        <FormContainer
          defaultValues={me.data}
          onSuccess={action}
          disabled={updateProfile.isPending}
        >
          <Grid container spacing={2}>
            <Grid item xs={12}>
              <Typography variant="h6" textAlign="center">
                USER INFORMATION
              </Typography>
            </Grid>

            <Grid item xs={12} md={6}>
              <TextField
                label="Email"
                value={me.data?.email}
                fullWidth
                disabled
              />
            </Grid>

            <Grid item xs={12} md={6}>
              <TextFieldElement name="name" label="Name" fullWidth required />
            </Grid>

            <Grid item xs={12} md={6}>
              <SelectElement
                label="Preferred location"
                name="preferredLocationId"
                fullWidth
                options={
                  locations?.data?.map((l) => ({
                    id: l.id,
                    label: l.name,
                  })) ?? []
                }
              />
            </Grid>

            <Grid item xs={12} md={6}>
              <SwitchElement
                name="allowNotifications"
                label="Allow email notifications"
              />
            </Grid>

            <Grid item xs={12} md={6}>
              <SwitchElement name="isVegetarian" label="Vegetarian" />
            </Grid>

            <Grid item xs={12}>
              <Button
                variant="contained"
                color="success"
                fullWidth
                type="submit"
                startIcon={<Save />}
                disabled={updateProfile.isPending}
              >
                SAVE
              </Button>
            </Grid>
          </Grid>
        </FormContainer>
      </Grid>
    </Grid>
  );
}
