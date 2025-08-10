import {
  FormContainer,
  SelectElement,
  TextFieldElement,
  useForm,
} from "react-hook-form-mui";
import { DatePickerElement } from "react-hook-form-mui/date-pickers";
import { Button, Stack, Typography } from "@mui/material";
import $api from "../../services/api";
import { components, RedCodeRequesterRole } from "../../services/api.openapi";
import ErrorMessage from "../../ui/ErrorMessage";
import FullscreenLoading from "../../ui/FullscreenLoading";
import dayjs from "dayjs";
import { Send } from "@mui/icons-material";
import toast from "react-hot-toast";

export default function GuestRequestForm() {
  const departments = $api.useQuery("get", "/swagger/Departments");
  const locations = $api.useQuery("get", "/swagger/Locations");
  const createGuestRequest = $api.useMutation(
    "post",
    "/swagger/RedCodes/requests",
    {
      onSuccess: () => {
        toast.success("Guest request submitted successfully");
        form.reset();
      },
    }
  );

  const form = useForm({
    defaultValues: {
      fullName: "",
      workEmail: "",
      guestDateOfArrival: dayjs(),
      guestContactNumber: "",
      guestPurposeOfVisit: "",
      guestTitle: "",
      workLocationId: undefined,
      departmentId: undefined,
      departmentChargeCodeId: undefined,
      role: RedCodeRequesterRole.Guests,
    },
  });
  const selectedDepartment = form.watch("departmentId");

  function submitForm(obj: components["schemas"]["RequestRedCodeCommand"]) {
    const formattedObj = {
      ...obj
    };
    createGuestRequest.mutate({ body: formattedObj });
  }

  if (departments.isLoading || locations.isLoading)
    return <FullscreenLoading />;
  if (departments.error || locations.error)
    return (
      <ErrorMessage
        error={{
          departments: departments.error,
          locations: locations.error,
        }}
      />
    );

  return (
    <Stack spacing={2}>
      {createGuestRequest.isPending && <FullscreenLoading />}
      <Typography variant="h5" textAlign="center">
        REGISTER FOR DEPARTMENT GUEST
      </Typography>
      <FormContainer
        // @ts-expect-error fix later
        formContext={form}
        onSuccess={submitForm}
        disabled={createGuestRequest.isPending}
      >
        <Stack spacing={2}>
          <TextFieldElement name={"fullName"} label="Full name" required />
          <TextFieldElement
            name="workEmail"
            label="Email"
            required
            type="email"
          />
          <TextFieldElement
            name="guestContactNumber"
            label="Contact Number"
            required
          />
          <TextFieldElement
            name="guestPurposeOfVisit"
            label="Purpose of Visit"
            required
          />
          <TextFieldElement name="guestTitle" label="Title" required />
          <DatePickerElement
            name={"guestDateOfArrival"}
            label={"Date of arrival"}
            required
          />
          <SelectElement
            label="Work location"
            name="workLocationId"
            required
            options={locations?.data?.map((location) => ({
              id: location.id,
              label: location.name,
            }))}
          />
          <SelectElement
            label="Department"
            name="departmentId"
            required
            options={departments?.data?.map((department) => ({
              id: department.id,
              label: department.name,
            }))}
          />
          <SelectElement
            label="Department charge code"
            name="departmentChargeCodeId"
            required
            disabled={!selectedDepartment}
            options={departments?.data
              ?.find((department) => department.id === selectedDepartment)
              ?.chargeCodes.map((chargeCode) => ({
                id: chargeCode.id,
                label: chargeCode.name,
              }))}
          />
          <Button
            type={"submit"}
            variant="contained"
            startIcon={<Send />}
            disabled={createGuestRequest.isPending}
          >
            Submit
          </Button>
        </Stack>
      </FormContainer>
    </Stack>
  );
}
