import {
  Box,
  Button,
  ButtonGroup,
  Container,
  Grid,
  MenuItem,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Typography,
  Paper,
  TablePagination,
  IconButton,
} from "@mui/material";
import { useState } from "react";
import RedCodeDialog from "../features/redcode/RedCodeDialog";
import $api from "../services/api";
import {
  RedCodeRequestStatus,
} from "../services/api.openapi";
import dayjs from "dayjs";
import { useDebounce } from "use-debounce";
import ErrorMessage from "../ui/ErrorMessage";
import FullscreenLoading from "../ui/FullscreenLoading";
import { getEnumValues } from "../utils";
import toast from "react-hot-toast";
import { useConfirm } from "material-ui-confirm";
import RequestDetails from "../features/redcode/RequestDetails";
import {
  Block,
  CancelOutlined,
  CheckCircleOutline,
  InfoOutlined,
} from "@mui/icons-material";

function RedCodeRequests() {
  const confirm = useConfirm();
  const [pageNumber, setPageNumber] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);
  const [emailFilter, setEmailFilter] = useState("");
  const [nameFilter, setNameFilter] = useState("");
  const [debouncedEmailFilter] = useDebounce(emailFilter, 500);
  const [debouncedNameFilter] = useDebounce(nameFilter, 500);
  const [statusFilter, setStatusFilter] = useState<number>(-1);
  const redCodeRequests = $api.useQuery("get", "/swagger/RedCodes/requests", {
    params: {
      query: {
        pageNumber: pageNumber + 1,
        pageSize: rowsPerPage,
        email: debouncedEmailFilter,
        name: debouncedNameFilter,
        status: statusFilter === -1 ? null : statusFilter
      },
    },
  });
  const locations = $api.useQuery("get", "/swagger/Locations");
  const approveRedCodeRequest = $api.useMutation(
    "post",
    "/swagger/RedCodes/requests/approve",
    {
      onSuccess: () => {
        toast.success("Request approved");
      },
    }
  );
  const rejectRedCodeRequest = $api.useMutation(
    "post",
    "/swagger/RedCodes/requests/reject",
    {
      onSuccess: () => {
        toast.success("Request rejected");
      },
    }
  );
  const disableRedCode = $api.useMutation(
    "post",
    "/swagger/RedCodes/redcodes/disable",
    {
      onSuccess: () => {
        toast.success("Red code disabled");
      },
    }
  );
  const isPending =
    approveRedCodeRequest.isPending ||
    rejectRedCodeRequest.isPending ||
    disableRedCode.isPending;

  const [dialogState, setDialogState] = useState({
    open: false,
    title: "",
    description: "",
    action: null as null | ((note: string) => void),
  });

  const handleDialogClose = () => {
    setDialogState((prevState) => ({ ...prevState, open: false }));
  };

  const handleDialogSubmit = (note: string) => {
    if (dialogState.action) {
      dialogState.action(note);
    }
    setDialogState((prevState) => ({ ...prevState, open: false }));
  };

  const openDialog = (
    title: string,
    description: string,
    action: (note: string) => void
  ) => {
    setDialogState({
      open: true,
      title,
      description,
      action,
    });
  };

  if (redCodeRequests.error || locations.error)
    return (
      <ErrorMessage
        error={{
          redCodeRequests: redCodeRequests.error,
          locations: locations.error,
        }}
      />
    );

  function handleApproveRedCodeRequest(id: number) {
    openDialog(
      "Approve Red Code Request",
      "Enter a note for the approval (optional)",
      (note) => approveRedCodeRequest.mutate({ body: { id: id, note: note } })
    );
  }

  function handleRejectRedCodeRequest(id: number) {
    openDialog(
      "Reject Red Code Request",
      "Enter a note for the rejection",
      (note) => rejectRedCodeRequest.mutate({ body: { id: id, note: note } })
    );
  }

  function handleDisableRedCode(id: number) {
    openDialog(
      "Disable Red Code",
      "Enter a reason for the disabling",
      (reason) =>
        disableRedCode.mutate({
          body: { redCodeRequestId: id, reason: reason },
        })
    );
  }

  return (
    <Container>
      {isPending && <FullscreenLoading />}
      <Box my={4}>
        <Typography variant="h4" component="h1" gutterBottom align="center">
          Manage <strong>Red Code Requests</strong>
        </Typography>
      </Box>
      <Grid container spacing={2} mb={2}>
        <Grid item xs={12} sm={3}>
          <TextField
            fullWidth
            label="Name"
            variant="outlined"
            value={nameFilter}
            onChange={(e) => setNameFilter(e.target.value)}
          />
        </Grid>
        <Grid item xs={12} sm={3}>
          <TextField
            fullWidth
            label="Email"
            variant="outlined"
            value={emailFilter}
            onChange={(e) => setEmailFilter(e.target.value)}
          />
        </Grid>
        <Grid item xs={12} sm={3}>
          <TextField
            fullWidth
            variant="outlined"
            label="Status"
            value={statusFilter}
            onChange={(e) => setStatusFilter(Number(e.target.value))}
            select
          >
            <MenuItem value={-1}>All</MenuItem>
            {getEnumValues(RedCodeRequestStatus).map(({ key, value }) => (
              <MenuItem key={key} value={Number(key)}>
                {value}
              </MenuItem>
            ))}
          </TextField>
        </Grid>
        <Grid item xs={12} sm={3} justifyContent="center" display="flex">
          <Button
            variant="outlined"
            onClick={() => {
              setNameFilter("");
              setEmailFilter("");
              setStatusFilter(-1);
            }}
          >
            Clear Filters
          </Button>
        </Grid>
      </Grid>
      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Submitted Date</TableCell>
              <TableCell>Request By</TableCell>
              <TableCell>Number of Orders</TableCell>
              <TableCell>Location</TableCell>
              <TableCell>CheckInDate</TableCell>
              <TableCell>Charge Code</TableCell>
              <TableCell>Action</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {redCodeRequests.isLoading && (
              <TableRow>
                <TableCell colSpan={9} align="center">
                  <FullscreenLoading />
                </TableCell>
              </TableRow>
            )}
            {redCodeRequests.data?.items.map((r) => (
              <TableRow key={r.id}>
                <TableCell>
                  {dayjs(r.created).format("DD/MM/YYYY HH:mm")}
                </TableCell>
                <TableCell>{r.fullName}<br/> ({r.workEmail})</TableCell>

                <TableCell>{r.orderNumbers}</TableCell>
                <TableCell>{r.workLocationName}</TableCell>
                <TableCell>{r.checkInDate}
                </TableCell>
                <TableCell>{r.departmentChargeCode?.name}</TableCell>
                <TableCell>
                  <ButtonGroup
                    variant="outlined"
                    size="small"
                    sx={{ margin: 1, borderRadius: 0 }}
                  >
                    <IconButton
                      color="info"
                      onClick={() =>
                        confirm({
                          title: "View Details",
                          hideCancelButton: true,
                          content: <RequestDetails request={r} />,
                        })
                      }
                    >
                      <InfoOutlined />
                    </IconButton>
                    {r.status === RedCodeRequestStatus.Submitted && (
                      <>
                        <IconButton
                          color="error"
                          disabled={isPending}
                          onClick={() => handleRejectRedCodeRequest(r.id)}
                        >
                          <CancelOutlined />
                        </IconButton>
                        <IconButton
                          color="success"
                          disabled={isPending}
                          onClick={() => handleApproveRedCodeRequest(r.id)}
                        >
                          <CheckCircleOutline />
                        </IconButton>
                      </>
                    )}
                    {r.status === RedCodeRequestStatus.Approved && (
                      <IconButton
                        color="warning"
                        disabled={isPending}
                        onClick={() => handleDisableRedCode(r.id)}
                      >
                        <Block />
                      </IconButton>
                    )}
                  </ButtonGroup>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
      <TablePagination
        component="div"
        count={redCodeRequests.data?.totalCount || 0}
        page={pageNumber}
        onPageChange={(_, newPage) => setPageNumber(newPage)}
        rowsPerPage={rowsPerPage}
        onRowsPerPageChange={(event) =>
          setRowsPerPage(parseInt(event.target.value, 10))
        }
      />
      <RedCodeDialog
        open={dialogState.open}
        onClose={handleDialogClose}
        onSubmit={handleDialogSubmit}
        title={dialogState.title}
        description={dialogState.description}
      />
    </Container>
  );
}

export default RedCodeRequests;
