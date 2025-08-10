import React, { useEffect, useState } from "react";
import {
  Container,
  Typography,
  TextField,
  MenuItem,
  Button,
  Grid,
  Box,
  Paper,
  Select,
  InputLabel,
  Dialog,
  DialogContent,
  DialogTitle,
  Stack,
} from "@mui/material";
import {
  CloudDone,
  Delete,
  DoneAll,
  DownloadOutlined,
  Preview,
  Publish,
  UploadFile,
} from "@mui/icons-material";
import $api, { $fetchClient } from "../services/api";
import FullscreenLoading from "../ui/FullscreenLoading";
import ErrorMessage from "../ui/ErrorMessage";
import { MenuStatus } from "../services/api.openapi";
import { useConfirm } from "material-ui-confirm";
import MonthlyMenuTable from "../features/menu/MonthlyMenuTable";

const monthNames = [
  "Jan",
  "Feb",
  "Mar",
  "Apr",
  "May",
  "Jun",
  "Jul",
  "Aug",
  "Sep",
  "Oct",
  "Nov",
  "Dec",
];

const SetupMonthlyMenu = () => {
  const user = $api.useQuery("get", "/swagger/Users/me", {
    params: { query: { includeAssignedLocations: true } },
  });
  const locations = $api.useQuery("get", "/swagger/Locations");

  const [selectedYear, setSelectedYear] = useState<number>(
    new Date().getFullYear()
  );
  const years = [2024, 2025, 2026];

  const [selectedMonth, setSelectedMonth] = useState<number>(
    new Date().getMonth() + 1
  );

  const [selectedLocationId, setSelectedLocationId] = useState<number | null>(
    null
  );
  const selectedLocation = locations.data?.find(
    (location) => location.id === selectedLocationId
  );

  const [previewMenu, setPreviewMenu] = useState<boolean>(false);

  const createMonthlyMenu = $api.useMutation("post", "/swagger/MonthlyMenus");
  const deleteMonthlyMenu = $api.useMutation(
    "delete",
    "/swagger/MonthlyMenus/{id}/delete"
  );
  const publishMonthlyMenu = $api.useMutation(
    "post",
    "/swagger/MonthlyMenus/{id}/publish"
  );

  const monthlyMenusStatus = $api.useQuery(
    "get",
    "/swagger/MonthlyMenus/status",
    {
      params: {
        query: {
          locationId: selectedLocationId!,
          year: selectedYear,
        },
      },
    },
    { enabled: selectedLocationId !== null && selectedYear !== null }
  );

  const statusOfCurrentMonthMenu = monthlyMenusStatus.data?.find(
    (menu) => menu.month === selectedMonth
  );

  const currentMonthlyMenu = $api.useQuery(
    "get",
    "/swagger/MonthlyMenus",
    {
      params: {
        query: {
          locationId: selectedLocationId!,
          year: selectedYear,
          month: selectedMonth,
        },
      },
    },
    {
      enabled: previewMenu && selectedLocationId !== null,
    }
  );

  const confirm = useConfirm();

  const isPending =
    createMonthlyMenu.isPending ||
    deleteMonthlyMenu.isPending ||
    publishMonthlyMenu.isPending;

  const handleDeleteMenu = () => {
    confirm({ description: "Are you sure you want to delete this menu?" })
      .then(() => {
        if (statusOfCurrentMonthMenu?.monthlyMenuId == null) return;
        deleteMonthlyMenu.mutate({
          params: { path: { id: statusOfCurrentMonthMenu?.monthlyMenuId } },
        });
      })
      .catch(() => {});
  };

  const handlePublishMenu = () => {
    confirm({ description: "Are you sure you want to publish this menu?" })
      .then(() => {
        if (statusOfCurrentMonthMenu?.monthlyMenuId == null) return;
        publishMonthlyMenu.mutate({
          params: { path: { id: statusOfCurrentMonthMenu?.monthlyMenuId } },
        });
      })
      .catch(() => {});
  };

  const [isDownloading, setIsDownloading] = useState<boolean>(false);

  async function handleDownloadMonthlyMenuTemplate() {
    setIsDownloading(true);
    try {
      if (selectedLocationId == null) return;
      const result = await $fetchClient.GET("/swagger/MonthlyMenus/template", {
        params: {
          query: {
            locationId: selectedLocationId,
            year: selectedYear,
            month: selectedMonth,
          },
        },
        parseAs: "blob",
      });

      const blob = result.data;
      if (!blob) return;
      const url = URL.createObjectURL(blob);

      // Create a link element
      const link = document.createElement("a");
      link.href = url;
      link.setAttribute(
        "download",
        `${selectedYear}-${selectedMonth}-${selectedLocation?.name}-MonthlyMenuTemplate.xlsx`
      );

      // Append the link to the body
      document.body.appendChild(link);

      // Programmatically click the link to trigger the download
      link.click();

      // Remove the link from the document
      link.parentNode?.removeChild(link);

      // Release the object URL
      URL.revokeObjectURL(url);
    } finally {
      setIsDownloading(false);
    }
  }

  async function handleDownloadCheckinsReport() {
    setIsDownloading(true);
    try {
      if (selectedLocationId == null) return;
      const result = await $fetchClient.GET("/swagger/Checkins/export", {
        params: {
          query: {
            locationId: selectedLocationId,
            year: selectedYear,
            month: selectedMonth,
          },
        },
        parseAs: "blob",
      });

      const blob = result.data;
      if (!blob) return;
      const url = URL.createObjectURL(blob);

      // Create a link element
      const link = document.createElement("a");
      link.href = url;
      link.setAttribute(
        "download",
        `${selectedYear}-${selectedMonth}-${selectedLocation?.name}-CheckinsReport.xlsx`
      );

      // Append the link to the body
      document.body.appendChild(link);

      // Programmatically click the link to trigger the download
      link.click();

      // Remove the link from the document
      link.parentNode?.removeChild(link);

      // Release the object URL
      URL.revokeObjectURL(url);
    } finally {
      setIsDownloading(false);
    }
  }

  // Select default location
  useEffect(() => {
    const firstAssignedLocationId = user.data?.assignedLocations?.[0]?.id;
    if (firstAssignedLocationId) {
      setSelectedLocationId(firstAssignedLocationId);
    }
  }, [user.data]);

  const handleFileChange = async (
    event: React.ChangeEvent<HTMLInputElement>
  ) => {
    if (event.target.files?.[0] !== null && selectedLocationId !== null) {
      // event.preventDefault();
      createMonthlyMenu.mutate({
        params: {
          query: {
            locationId: selectedLocationId,
            month: selectedMonth,
            year: selectedYear,
          },
        },
        body: {
          file: "",
        },
        bodySerializer: () => {
          const formData = new FormData();
          if (event.target.files && event.target.files[0]) {
            formData.set("file", event.target.files[0]);
          }
          return formData;
        },
      });
    }
  };

  if (monthlyMenusStatus.isLoading || user.isLoading || locations.isLoading) {
    return <FullscreenLoading />;
  }
  if (monthlyMenusStatus.isError || user.isError || locations.isError) {
    return (
      <ErrorMessage
        error={{
          monthlyMenusStatus: monthlyMenusStatus.error,
          user: user.error,
          locations: locations.error,
          createMonthlyMenu: createMonthlyMenu.error,
          deleteMonthlyMenu: deleteMonthlyMenu.error,
          publishMonthlyMenu: publishMonthlyMenu.error,
        }}
      />
    );
  }

  return (
    <Container maxWidth="md">
      {isPending && <FullscreenLoading />}
      <Box my={4}>
        <Typography variant="h4" align="center" gutterBottom>
          Set Up <strong>Monthly Menu</strong>
        </Typography>
      </Box>
      <Grid container spacing={2} justifyContent="center">
        <Grid item xs={12} md={2}>
          <InputLabel id="year-label">Year</InputLabel>
          <Select
            labelId="year-label"
            fullWidth
            variant="outlined"
            defaultValue={selectedYear}
            onChange={(e) =>
              setSelectedYear(parseInt(e.target.value.toString()))
            }
          >
            {years.map((year) => (
              <MenuItem key={year} value={year}>
                {year}
              </MenuItem>
            ))}
          </Select>
        </Grid>
        <Grid item xs={12} md={3}>
          <InputLabel id="location-label">Location</InputLabel>
          <Select
            labelId="location-label"
            fullWidth
            variant="outlined"
            value={selectedLocationId ?? ""}
            onChange={(e) =>
              setSelectedLocationId(
                e.target.value ? parseInt(e.target.value?.toString()) : null
              )
            }
          >
            {user.data?.assignedLocations.map((location) => (
              <MenuItem key={location.id} value={location.id}>
                {location.name}
              </MenuItem>
            ))}
          </Select>
        </Grid>
        <Grid item xs={12} md={3}>
          <InputLabel id="supplier-label">Supplier</InputLabel>
          <TextField
            fullWidth
            variant="outlined"
            value={selectedLocation?.supplierName ?? "No supplier"}
            disabled
          />
        </Grid>
        <Grid item xs={12} md={4} display="flex" justifyContent="center">
          <Button
            variant="outlined"
            startIcon={<DownloadOutlined />}
            onClick={handleDownloadMonthlyMenuTemplate}
            disabled={isDownloading}
          >
            {isDownloading ? "Downloading..." : "Download Template"}
          </Button>
        </Grid>
      </Grid>
      <Box display="flex" justifyContent="center" mt={2}>
        {monthNames.map((month, index) => (
          <Button
            key={month}
            variant={selectedMonth - 1 === index ? "contained" : "outlined"}
            color="primary"
            onClick={() => setSelectedMonth(index + 1)}
            style={{
              margin: "0 5px",
              backgroundColor:
                selectedMonth - 1 === index ? "#FFD700" : undefined,
              color: selectedMonth - 1 === index ? "green" : undefined,
              borderRadius: "50%",
              width: "70px",
              height: "70px",
              minWidth: "70px",
            }}
            disabled={
              new Date().getFullYear() > selectedYear ||
              (new Date().getFullYear() === selectedYear &&
                new Date().getMonth() + 1 > index + 1 &&
                monthlyMenusStatus.data?.[index].status === MenuStatus.NoMenu)
            }
          >
            <p>
              {month}
              <br />
              {monthlyMenusStatus.data?.[index].status == MenuStatus.NoMenu
                ? "❌"
                : monthlyMenusStatus.data?.[index].status == MenuStatus.Uploaded
                ? "⬆️"
                : monthlyMenusStatus.data?.[index].status ==
                  MenuStatus.Published
                ? "✅"
                : ""}
            </p>
          </Button>
        ))}
      </Box>
      {statusOfCurrentMonthMenu?.status === MenuStatus.NoMenu && (
        <Paper variant="outlined" sx={{ mt: 4, p: 4, textAlign: "center" }}>
          <UploadFile fontSize="large" />
          <Typography variant="h6" gutterBottom>
            Upload monthly menu
          </Typography>
          <Typography variant="body2" color="textSecondary" gutterBottom>
            Excel (XLSX) File Only
          </Typography>
          <Button
            variant="contained"
            component="label"
            color="primary"
            startIcon={<UploadFile />}
            disabled={createMonthlyMenu.isPending}
          >
            {createMonthlyMenu.isPending ? "Uploading..." : "CHOOSE FILE"}
            <input
              type="file"
              hidden
              onChange={handleFileChange}
              accept=".xlsx"
            />
          </Button>
        </Paper>
      )}
      {statusOfCurrentMonthMenu?.status === MenuStatus.Uploaded && (
        <Paper variant="outlined" sx={{ mt: 4, p: 4, textAlign: "center" }}>
          <CloudDone fontSize="large" />
          <Typography variant="h6" gutterBottom sx={{ mb: 2 }}>
            Menu uploaded
          </Typography>
          <Stack direction="row" spacing={2} justifyContent="center">
            <Button
              variant="contained"
              color="info"
              onClick={() => setPreviewMenu(true)}
              startIcon={<Preview />}
            >
              PREVIEW MENU
            </Button>
            {selectedYear > new Date().getFullYear() ||
            (selectedYear === new Date().getFullYear() &&
              selectedMonth > new Date().getMonth() + 1) ? (
              <>
                <Button
                  variant="contained"
                  color="error"
                  onClick={handleDeleteMenu}
                  disabled={deleteMonthlyMenu.isPending}
                  startIcon={<Delete />}
                >
                  {deleteMonthlyMenu.isPending ? "Deleting..." : "DELETE MENU"}
                </Button>
                <Button
                  variant="contained"
                  color="primary"
                  onClick={handlePublishMenu}
                  disabled={publishMonthlyMenu.isPending}
                  startIcon={<Publish />}
                >
                  {publishMonthlyMenu.isPending ? "Publishing..." : "PUBLISH"}
                </Button>
              </>
            ) : null}
          </Stack>
        </Paper>
      )}
      {statusOfCurrentMonthMenu?.status === MenuStatus.Published && (
        <Paper variant="outlined" sx={{ mt: 4, p: 4, textAlign: "center" }}>
          <DoneAll fontSize="large" />
          <Typography variant="h6" gutterBottom>
            Menu published
          </Typography>
          <Stack direction="row" spacing={2} justifyContent="center">
            <Button
              variant="contained"
              color="info"
              onClick={() => setPreviewMenu(true)}
              startIcon={<Preview />}
            >
              PREVIEW MENU
            </Button>
            <Button
              variant="contained"
              color="primary"
              onClick={handleDownloadCheckinsReport}
              startIcon={<DownloadOutlined />}
              disabled={isDownloading}
            >
              EXPORT CHECKINS REPORT
            </Button>
          </Stack>
        </Paper>
      )}
      <Dialog
        open={previewMenu}
        onClose={() => setPreviewMenu(false)}
        fullWidth
        maxWidth="lg"
      >
        <DialogTitle>Preview Menu</DialogTitle>
        <DialogContent>
          {currentMonthlyMenu.data ? (
            <MonthlyMenuTable data={currentMonthlyMenu.data} />
          ) : currentMonthlyMenu.isLoading ? (
            <Typography>Loading...</Typography>
          ) : (
            <Typography>No data available</Typography>
          )}
        </DialogContent>
      </Dialog>
    </Container>
  );
};

export default SetupMonthlyMenu;
