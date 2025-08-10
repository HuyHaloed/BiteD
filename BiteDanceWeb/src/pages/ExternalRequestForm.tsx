import {
  FormContainer,
  SelectElement,
  TextFieldElement,
  useForm,
} from "react-hook-form-mui";
import ReactDOM from "react-dom";
import {
  Button,
  Container,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
  Paper,
  Stack,
  Link
} from "@mui/material";
import QRCode from "react-qr-code";


import { useState, useEffect } from "react";
import { DatePickerElement } from "react-hook-form-mui/date-pickers";
import $api from "../services/api";
import { components } from "../services/api.openapi";
import FullscreenLoading from "../ui/FullscreenLoading";
import ErrorMessage from "../ui/ErrorMessage";
import dayjs from "dayjs";
import {Send } from "@mui/icons-material";
import toast from "react-hot-toast";
import { useDebounce } from "use-debounce";
import {
  RedCodeRequestStatus,
} from "../services/api.openapi";


export default function ExternalRequestForm() {
    const [pageNumber] = useState(0);
    const [rowsPerPage] = useState(10);
    const [emailFilter] = useState("");
    const [nameFilter] = useState("");
    const [debouncedEmailFilter] = useDebounce(emailFilter, 500);
    const [debouncedNameFilter] = useDebounce(nameFilter, 500);
    const [statusFilter] = useState<number>(-1);
  const departments = $api.useQuery("get", "/swagger/Departments");
  const me = $api.useQuery("get", "/swagger/Users/me", {
      params: { query: { includeAssignedLocations: false } },
    });
  const redCodeRequests = $api.useQuery("get", "/swagger/RedCodes/myrequests", {
      params: {
        query: {
          pageNumber: pageNumber + 1,
          pageSize: rowsPerPage,
          email: debouncedEmailFilter,
          name: debouncedNameFilter,
          status: statusFilter === -1 ? null : statusFilter,
        },
      },
    });
  const [Fullname, setRequesterRedCodeName] = useState("");
  const [Email, setRequesterRedCodeEmail] = useState("");
  const locations = $api.useQuery("get", "/swagger/Locations");
  const [formType, setFormType] = useState<"Form Request" | "History Request" | null>(null);
  const [selectedDepartmentId, setSelectedDepartmentId] = useState<string | undefined>();

  const selectedDepartment = departments?.data?.find(
  d => d.id === Number(selectedDepartmentId)
);
  useEffect(() => {
    setFormType("Form Request");
  }, []);
  const createExternalRequest = $api.useMutation(
    "post",
    "/swagger/RedCodes/requests",
    {
      onSuccess: () => {
        toast.success("Request submitted successfully");
        form.reset();
      },
    }
  );
  type FormValues = {
  fullName: string;
  workEmail: string;
  workLocationId: string;
  orderNumbers: number;
  checkInDate: Date;
  departmentId: number;
  departmentChargeCodeId: string;
  reasonForVisit: string;
};

const form = useForm<FormValues>();

  function downloadQrFromValue(value: string, fileName = "qr-code") {
  const svg = document.createElement("div");
  ReactDOM.render(<QRCode value={value} fgColor="#8B0000" bgColor="#ffffff" />, svg);

  setTimeout(() => {
    const qrSvg = svg.querySelector("svg");
    if (!qrSvg) return;

    const svgData = new XMLSerializer().serializeToString(qrSvg);
    const canvas = document.createElement("canvas");
    const ctx = canvas.getContext("2d");
    const img = new Image();

    img.onload = () => {
      const padding = 10;
      const qrWidth = img.width;
      const qrHeight = img.height;

      canvas.width = qrWidth + padding * 2;
      canvas.height = qrHeight + padding * 2;

      if (ctx) {
        ctx.fillStyle = "#FFFFFF";
        ctx.fillRect(0, 0, canvas.width, canvas.height);
        ctx.drawImage(img, padding, padding);
      }

      const pngFile = canvas.toDataURL("image/png");
      const downloadLink = document.createElement("a");
      downloadLink.href = pngFile;
      downloadLink.download = `${fileName}.png`;
      downloadLink.click();
    };

    img.src = `data:image/svg+xml;base64,${btoa(unescape(encodeURIComponent(svgData)))}`;
  }, 0);
}



  if (redCodeRequests.error || locations.error)
    return (
      <ErrorMessage
        error={{
          redCodeRequests: redCodeRequests.error,
          locations: locations.error,
        }}
      />
    );
   useEffect(() => {
  if (me.data) {
    setRequesterRedCodeName(me.data.name);
    setRequesterRedCodeEmail(me.data.email);
    form.reset({
      fullName: me.data.name,
      workEmail: me.data.email,
    });
  }
}, [me.data]);

  function submitForm(obj: components["schemas"]["RequestRedCodeCommand"]) {

    const formattedDate = dayjs(obj.checkInDate).format("YYYY-MM-DD");
    createExternalRequest.mutate({
      body: {
        ...obj,
        checkInDate: formattedDate,
        fullName: Fullname,
        workEmail: Email
      },
    });
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
    <Container>
      {createExternalRequest.isPending && <FullscreenLoading />}
      <Typography variant="h4" component="h1" my={4} align="center">
        REGISTER FOR GUEST
      </Typography>
      <div className="flex gap-4 mb-4 ">
      <Stack direction="row" spacing={2} justifyContent="center" alignItems="center">
        <Button  
          variant={formType === "Form Request" ? "contained" : "outlined"}

          sx={{ width: "150px", height: "40px" }}
          onClick={() => setFormType("Form Request")}
        >
          Form
        </Button>

        <Button  
          variant={formType === "History Request" ? "contained" : "outlined"}
          sx={{ width: "150px", height: "40px" }}
          onClick={() => setFormType("History Request")}
        >
          View Requests
        </Button>
      </Stack>
      </div>
      <div className="pb-4 "></div>
      {formType === "Form Request" && (
        <FormContainer
  // @ts-expect-error fix later
  formContext={form}
  onSuccess={submitForm}
  disabled={createExternalRequest.isPending}
>
  <Stack spacing={2} sx={{ pt: 2 }}>
    {/* 1. Full name */}
    <TextFieldElement name="fullName" label="Full name" required disabled />

    {/* 2. Work email */}
    <TextFieldElement
      name="workEmail"
      label="Work Email"
      required
      disabled
      type="email"
    />

    {/* 3. Order Location */}
    <SelectElement
      label="Order Location"
      name="workLocationId"
      required
      options={locations?.data?.map((location) => ({
        id: location.id,
        label: location.name,
      }))}
    />

    {/* 4. Order Number */}
    <TextFieldElement
      name="orderNumbers"
      label="Order Quantity"
      required
      type="number"
      inputProps={{ max: 1000, min: 1 }}
    />

    {/* 5. Check-in Date */}
    <DatePickerElement
      name="checkInDate"
      label="Check-in Date"
      required
    />

   {/* 6.1 Department */}
<SelectElement
  label="Department"
  name="departmentId"
  required
  options={departments?.data?.map((departments2) => ({
    id: departments2.id,
    label: departments2.name,
  }))}
  onChange={(val) => {
    setSelectedDepartmentId(val);
    form.setValue('departmentChargeCodeId', '');
     // Reset charge code khi đổi department
  }}
/>

{/* 6.2 Department Charge Code */}
<>
  <SelectElement
    label="Team"
    name="departmentChargeCodeId"
    required
    options={
      selectedDepartment?.chargeCodes?.map((code) => ({
        id: code.id,
        label: code.name,
      })) ?? []
    }
  />
</>

    {/* 7. Reason for visit */}
    <TextFieldElement
      name="GuestPurposeOfVisit"
      label="Reason for Order"
      required
      multiline
      rows={3}
    />

    <Button
      variant="contained"
      type="submit"
      startIcon={<Send />}
      disabled={createExternalRequest.isPending}
    >
      Submit
    </Button>
  </Stack>
</FormContainer>)}
<Typography variant="h4" component="h1" my={4} align="center">
        
        
      </Typography>
{formType === "History Request" && (
  
     <TableContainer component={Paper} sx={{ minWidth: 800 }}>
  <Table size="small">
    <TableHead>
      <TableRow>
        <TableCell>CheckInDate</TableCell>
        <TableCell>Order Quantity</TableCell>
        <TableCell>Status</TableCell>
        <TableCell>Red Code</TableCell>
        <TableCell>Submitted Date</TableCell>
        <TableCell>Location</TableCell>
        <TableCell>Team</TableCell>
      </TableRow>
    </TableHead>
    <TableBody>
      {redCodeRequests.isLoading && (
        <TableRow>
          <TableCell colSpan={7} align="center">
            <FullscreenLoading />
          </TableCell>
        </TableRow>
      )}
      {redCodeRequests.data?.items.map((r) => (
        <TableRow key={r.id}>
          <TableCell>{r.checkInDate}</TableCell>
          <TableCell>{r.orderNumbers}</TableCell>
          <TableCell>
            <Typography
              sx={{
                fontWeight: 100,
                color:
                  RedCodeRequestStatus[r.status] === "Submitted"
                    ? "black"
                    : RedCodeRequestStatus[r.status] === "Approved"
                    ? "green"
                    : "red",
              }}
            >
              {RedCodeRequestStatus[r.status]}
            </Typography>
          </TableCell>
          <TableCell>
            {RedCodeRequestStatus[r.status] === "Approved" &&
            r.redScanCode?.redCodeId ? (
              <Link
                component="button"
                underline="hover"
                onClick={() =>
                  downloadQrFromValue(
                    "r:" + r.redScanCode?.redCodeId,
                    `red-qrcode-` + r.checkInDate
                  )
                }
              >
                Download
              </Link>
            ) : (
              "-"
            )}
          </TableCell>
          <TableCell>
            {dayjs(r.created).format("DD/MM/YYYY HH:mm")}
          </TableCell>
          <TableCell>{r.workLocationName}</TableCell>
          <TableCell>{r.departmentChargeCode?.name}</TableCell>
        </TableRow>
      ))}
    </TableBody>
  </Table>
</TableContainer>


)}
    </Container>
  );
}
