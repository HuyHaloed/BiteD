import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import Test from "./pages/Test.tsx";
import DefaultRedirect from "./pages/DefaultRedirect"; 
import {
  MutationCache,
  QueryClient,
  QueryClientProvider,
} from "@tanstack/react-query";
import { BrowserRouter, Route, Routes } from "react-router-dom";
import ProtectedRoute from "./ui/ProtectedRoute.tsx";
import AppLayout from "./ui/AppLayout.tsx";
import PageNotFound from "./pages/PageNotFound.tsx";
import Locations from "./pages/Locations.tsx";
import { createTheme, CssBaseline, ThemeProvider } from "@mui/material";
import Location from "./pages/Location.tsx";
import NewLocation from "./pages/NewLocation.tsx";
import Suppliers from "./pages/Suppliers.tsx";
import Supplier from "./pages/Supplier.tsx";
import NewSupplier from "./pages/NewSupplier.tsx";
import { LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import Admins from "./pages/Admins.tsx";
import { ConfirmProvider } from "material-ui-confirm";
import ExternalRequestForm from "./pages/ExternalRequestForm.tsx";
import RedCodeRequests from "./pages/RedCodeRequests.tsx";
import RedCodeRequestsSummary from "./pages/RedCodeRequestsSummary.tsx";
import DailyReports from "./pages/DailyReports.tsx";
import { AuthProvider } from "react-oidc-context";
import { oidcConfig } from "./services/auth.ts";
import SetupMonthlyMenu from "./pages/SetupMonthlyMenu.tsx";
import Order from "./pages/Order.tsx";
import UpdateProfile from "./pages/UpdateProfile.tsx";
import MyQr from "./pages/MyQr.tsx";
import QrCodeScannerPage from "./pages/QrCodeScannerPage.tsx";
import { Toaster } from "react-hot-toast";

// Create a custom theme
const theme = createTheme({
  components: {
    MuiButton: {
      styleOverrides: {
        root: {
          borderRadius: "50px", // Set the border radius to make buttons rounded
        },
      },
    },
  },
  palette: {
    primary: {
      main: "#00712D",
    },
    secondary: {
      main: "#FFDD33",
    },
  },
});

const queryClient = new QueryClient({
  mutationCache: new MutationCache({
    onSuccess: () => {
      // Invalidate everything
      // https://tkdodo.eu/blog/automatic-query-invalidation-after-mutations
      queryClient.invalidateQueries();
    },
  }),
});

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <CssBaseline />
    <LocalizationProvider dateAdapter={AdapterDayjs}>
      <ConfirmProvider>
        <Toaster position="top-center"></Toaster>
        <AuthProvider {...oidcConfig}>
          <QueryClientProvider client={queryClient}>
            {/* <ReactQueryDevtools initialIsOpen={false} /> */}
            <ThemeProvider theme={theme}>
              <BrowserRouter>
                <Routes>
                  <Route
                    element={
                      <ProtectedRoute>
                        <AppLayout />
                      </ProtectedRoute>
                    }
                  >
                    <Route index element={<DefaultRedirect />} />
                    <Route path="locations" element={<Locations />} />
                    <Route path="location/:locationId" element={<Location />} />
                    <Route path="new-location" element={<NewLocation />} />
                    <Route path="suppliers" element={<Suppliers />} />
                    <Route path="supplier/:supplierId" element={<Supplier />} />
                    <Route path="new-supplier" element={<NewSupplier />} />
                    <Route path="admins" element={<Admins />} />
                    <Route
                      path="setup-monthly-menu"
                      element={<SetupMonthlyMenu />}
                    />
                    <Route
                      path="red-code-requests"
                      element={<RedCodeRequests />}
                    />
                    <Route
                      path="red-code-requests-summary"
                      element={<RedCodeRequestsSummary />}
                    />
                    <Route
                      path="weekly-report"
                      element={<DailyReports />}
                    />
                    <Route path="order" element={<Order />} />
                    <Route path="update-profile" element={<UpdateProfile />} />
                    <Route path="my-qr" element={<MyQr />} />
                    <Route path="qr-scanner" element={<QrCodeScannerPage />} />
                    <Route path="test" element={<Test />} />
                    <Route path="external" element={<ExternalRequestForm />} />
                  </Route>
                  <Route path="*" element={<PageNotFound />} />
                </Routes>
              </BrowserRouter>
            </ThemeProvider>
          </QueryClientProvider>
        </AuthProvider>
      </ConfirmProvider>
    </LocalizationProvider>
  </StrictMode>
);
