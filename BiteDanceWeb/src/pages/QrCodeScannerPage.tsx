//QRCodeScannerPage.tsx
import React, { useEffect, useRef, useState } from "react";
import {
  Box,
  Typography,
  Grid,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
} from "@mui/material";
import QrScanResultCard from "../features/checkin/QrScanResultCard";
import { components } from "../services/api.openapi";
import $api from "../services/api";
import FullscreenLoading from "../ui/FullscreenLoading";
import CurrentTime from "../features/checkin/CurrentTime";
import QrScanner from "../features/checkin/QrScanner";
import { IDetectedBarcode } from "@yudiel/react-qr-scanner";
import { useDebounce } from "use-debounce";
import {
  RedCodeRequestStatus,
} from "../services/api.openapi";


enum ScanStatus {
  Success = "Success",
  Error = "Error",
  Warming = "Warming",
}
const today = new Date();
const yyyy = today.getFullYear();
const mm = String(today.getMonth() + 1).padStart(2, '0'); // Tháng bắt đầu từ 0
const dd = String(today.getDate()).padStart(2, '0');

const today_formatted = `${yyyy}-${mm}-${dd}`;

const QrCodeScannerPage: React.FC = () => {
  // Checkin audio
  const audioSucessRef = useRef<HTMLAudioElement>(null);
  const audioErrorRef = useRef<HTMLAudioElement>(null);
  const audioWarningRef = useRef<HTMLAudioElement>(null);
  // Admin info
  const me = $api.useQuery("get", "/swagger/Users/me", {
    params: { query: { includeAssignedLocations: true } }
  });

  // Checkin results
  const [scannedResults, setScannedResults] = useState<
    components["schemas"]["CheckinResult"][]
  >([]);
  const [numberOfPreOrder, setNumberOfPreOrder] = useState<number>(0);
  const [numberOfGreenScanner, setNumberOfGreenScanner] = useState<number>(0);
  const [numberOfBlueScanner, setNumberOfBlueScanner] = useState<number>(0);
  const [numberOfRedScanner, setNumberOfRedScanner] = useState<number>(0);
  const [numberOfApprovedRedCode, setNumberOfApproveRedCode] = useState<number>(0);
  const [numberOfAllScanner, setNumberOfAllScanner] = useState<number>(0);

    // Location
  const [selectedLocation, setSelectedLocation] = useState<number | null>(null);

  

   // get Checkin history

useEffect(() => {
  setNumberOfAllScanner(numberOfRedScanner+numberOfGreenScanner);
});
  
  // Recent scan status
  const [recentScanStatus, setRecentScanStatus] = useState<{
    status: ScanStatus | null;
    message: string;
  }>({ status: null, message: "" });

  const UserSupplier = $api.useQuery(
  "get",
  "/swagger/Suppliers/SupplierMapping/{UserId}",
  {
    params: { path: { UserId: me.data?.id ?? "" } }, // fallback tránh undefined
    enabled: !!me.data?.id, // chỉ gọi API khi đã có ID
  }
);


  useEffect(() => {
    if (UserSupplier.data && UserSupplier.data.assignedLocations.length > 0) {
      setSelectedLocation(UserSupplier.data.assignedLocations[0].id);
    }
  }, [UserSupplier.data]);

   const Checkin = $api.useQuery("get", "/swagger/Reports/checkin/dailyReport", {
    params: { query: { reportDate: today_formatted, locationId: parseInt(selectedLocation?.toString() ?? "0") } }
  });
  // get All order
  const Order = $api.useQuery("get", "/swagger/Reports/order/dailyReport", {
    params: { query: { reportDate: today_formatted, locationId: parseInt(selectedLocation?.toString() ?? "0")} }
  });

const [pageNumber] = useState(0);
      const [rowsPerPage] = useState(10);
      const [emailFilter] = useState("");
      const [nameFilter] = useState("");
      const [debouncedEmailFilter] = useDebounce(emailFilter, 500);
      const [debouncedNameFilter] = useDebounce(nameFilter, 500);
      const [statusFilter] = useState<number>(-1);
  
      const redCodeRequests = $api.useQuery("get", "/swagger/RedCodes/requests", {
        params: {
          query: {
            pageNumber: pageNumber + 1,
            pageSize: rowsPerPage,
            email: debouncedEmailFilter,
            name: debouncedNameFilter,
            status: statusFilter === -1 ? null : statusFilter,
            ReportDate: today_formatted,
            LocationId: parseInt(selectedLocation?.toString() ?? "0")
          },
        },
      });
useEffect(() => {
  if (redCodeRequests.data) {
    setNumberOfApproveRedCode(redCodeRequests.data?.items.filter(
  (r) => r.status === RedCodeRequestStatus.Approved
).reduce((sum, r) => sum + (r.orderNumbers ?? 0), 0) ?? 0);
  }
}, [Checkin.data]);

useEffect(() => {
  if (Order.data && typeof Order.data.numberOfPreOrder === "number") {
    setNumberOfPreOrder(Order.data.numberOfPreOrder + numberOfApprovedRedCode);
  }
}, [Order.data, numberOfApprovedRedCode]);

useEffect(() => {
  if (Checkin.data && typeof Checkin.data.numberOfGreenScanned === "number") {
    setNumberOfGreenScanner(Checkin.data.numberOfGreenScanned);
  }
}, [Checkin.data]);

useEffect(() => {
  if (Checkin.data && typeof Checkin.data.numberOfBlueScanned === "number") {
    setNumberOfBlueScanner(Checkin.data.numberOfBlueScanned);
  }
}, [Checkin.data]);

useEffect(() => {
  if (Checkin.data && typeof Checkin.data.numberOfRedScanned === "number") {
    setNumberOfRedScanner(Checkin.data.numberOfRedScanned);
  }
}, [Checkin.data]);






  const checkinGreenQr = $api.useMutation("post", "/swagger/Checkins/green", {
    onSuccess: (data) => {
      addNewCheckinResult(data);
    }
  });
  const checkinBlueQr = $api.useMutation("post", "/swagger/Checkins/blue", {
    onSuccess: (data) => {
      addNewCheckinResult(data);
    }
  });
  const checkinRedQr = $api.useMutation("post", "/swagger/Checkins/red", {
    onSuccess: (data) => {
      addNewCheckinResult(data);
    }
  });
  // Checkin pending status
  const isPending =
    checkinGreenQr.isPending ||
    checkinBlueQr.isPending ||
    checkinRedQr.isPending;
  const [pausedByDelay, setPausedByDelay] = useState(false);

  function addNewCheckinResult(result: components["schemas"]["CheckinResult"]) {
    setScannedResults((prevResults) => {
      const newResults = [...prevResults, result];
      return newResults;
    });
    let temp = result.message === "Order has already been scanned.";
    setRecentScanStatus({
      status: result.isSuccess ? ScanStatus.Success : temp ? ScanStatus.Warming : ScanStatus.Error,
      message: result.message,
    });
  }

  const handleScan = (result: IDetectedBarcode[] | undefined | null) => {
    if (isPending || pausedByDelay ) return; // not working

    const text = result?.[0]?.rawValue;
    if (!text) {
      return;
    } 
    
      
    const data = text.split(":");

    if (data.length !== 2) {
      return;
    }
    const payload = data[1];
    console.log(data[0]);
    


    // bug: "g" != "g", "g" from QR code != "g" in string comparison, use includes as workaround
    if (data[0].includes("g")) {
      const locId = parseInt(selectedLocation?.toString() ?? "0");
    console.log("Scanning GREEN QR → LocationId:", locId);
    checkinGreenQr.mutate({
        body: { locationId: parseInt(selectedLocation?.toString() ?? "0"), userId: payload },
      });

     if (recentScanStatus.status === ScanStatus.Success) {
      //setNumberOfGreenScanner(numberOfGreenScanner+1);
     }
    } else if (data[0].includes("b")) {
      const locId = parseInt(selectedLocation?.toString() ?? "0");
    console.log("Scanning GREEN QR → LocationId:", locId);
      checkinBlueQr.mutate({
        body: { locationId: parseInt(selectedLocation?.toString() ?? "0"), userId: payload },
      });
      /*if (recentScanStatus.status === ScanStatus.Success) {
      setNumberOfBlueScanner(numberOfBlueScanner+1);
     }*/
      
    } else if (data[0].includes("r")) {
      checkinRedQr.mutate({
        body: {
          locationId: parseInt(selectedLocation?.toString() ?? "0"),
          scanCodeId: payload,
        },
      });  
      if (recentScanStatus.status === ScanStatus.Success) {
      //setNumberOfRedScanner(numberOfRedScanner+1);
     }
      
    }
    setPausedByDelay(true);
    setTimeout(() => {
      setPausedByDelay(false);
    }, 2000); // 5000ms = 5s
  };

  // Play audio based on scan status and clear recent scan status
  useEffect(() => {
    if (recentScanStatus.status) {
      // Play audio
      const audio =
        recentScanStatus.status === ScanStatus.Success
          ? audioSucessRef
          : recentScanStatus.status === ScanStatus.Error ? audioErrorRef : audioWarningRef;
      if (audio.current) {
        audio.current.currentTime = 0;
        audio.current.play();
      }

      // Clear recent scan status after 3 seconds
      const timer = setTimeout(() => {
        setRecentScanStatus({ status: null, message: "" });
      }, 2000); // Reset after 3 seconds

      return () => clearTimeout(timer); // Cleanup the timeout on component unmount or when recentScanStatus changes
    }
  }, [recentScanStatus]);

  if (me.isLoading) return <FullscreenLoading />;

  return (
    <Box sx={{ display: "flex", height: "100vh", padding: 2 }}>
      {/* Audio for checkin success and error */}
      <audio
        ref={audioSucessRef}
        preload="auto"
        src="/ding-correct-gfx-sounds-1-00-02.mp3"
      />
      <audio
        ref={audioErrorRef}
        preload="auto"
        src="/meme-fail-alert-locran-1-00-01.mp3"
      />
      <audio
        ref={audioWarningRef}
        preload="auto"
        src="/alert-sound-230091.mp3"
      />

      {/* Left Side - QR Code Scanner */}
      <Box
        component="div"
        sx={{
          width: "50%",
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          backgroundColor:
            recentScanStatus.status === ScanStatus.Success
              ? "lightgreen"
              : recentScanStatus.status === ScanStatus.Error
              ? "lightcoral"
              : recentScanStatus.status === ScanStatus.Warming
              ? "orange"
              :"#f0f0f0" ,
          padding: 2,
        }}
      >
        <Typography variant="h4" gutterBottom>
          Scan <strong>QR Code</strong>
        </Typography>
        <Typography variant="body1" gutterBottom>
          Align QR Code within the frame to scan
        </Typography>
        <Box sx={{ width: "60vh", aspectRatio: "1/1"}}>
          <QrScanner onScan={handleScan} paused={isPending || pausedByDelay} />
        </Box>
        {recentScanStatus.status && (
          <Typography
            variant="h3"
            color="white"
            sx={{ marginTop: 5, textAlign: "center" }}
          >
            {recentScanStatus.message}
          </Typography>
        )}
      </Box>

      {/* Right Side - Checkin Tracking */}
      <Box sx={{ width: "50%", padding: 2 }}>
        <FormControl fullWidth sx={{ marginBottom: 2 }}>
          <InputLabel id="location-select-label">Select Location</InputLabel>
          <Select
            labelId="location-select-label"
            value={selectedLocation}
            onChange={(e) => {
              setSelectedLocation(parseInt(e.target.value?.toString() ?? "0")); 
            }}
            label="Select Location"
          >
            {UserSupplier.data?.assignedLocations.map((location) => (
              <MenuItem key={location.id} value={location.id}>
                {location.name}
              </MenuItem>
            ))}
          </Select>
        </FormControl>
        {/* <Stack direction="row" spacing={2} paddingY={2}>
          <Button
            variant="outlined"
            color="primary"
            onClick={insertDummyCheckinResult}
          >
            Insert Dummy
          </Button>
          <Button
            variant="outlined"
            color="primary"
            onClick={() =>
              setRecentScanStatus({
                status: ScanStatus.Error,
                message: "Scan failed",
              })
            }
          >
            Fail
          </Button>
          <Button
            variant="outlined"
            color="primary"
            onClick={() =>
              setRecentScanStatus({
                status: ScanStatus.Success,
                message: "Scan successful",
              })
            }
          >
            Success
          </Button>
        </Stack> */}
        <Typography variant="h5" gutterBottom>
          RECENT CHECKINS ({scannedResults.length})  &nbsp;
          <span style={{ fontWeight: 'normal', fontSize: '0.9em' }}>
            <strong>({numberOfAllScanner}/{numberOfPreOrder}, Blue: {numberOfBlueScanner})</strong>
          </span>
        </Typography>
        <Typography variant="h5" gutterBottom>
          <CurrentTime />
        </Typography>
        <Box sx={{ height: "calc(100vh - 400px)", overflow: "auto" }}>
          <Grid container spacing={2}>
            {scannedResults
              .slice(-50) // Limit to 50 results
              .slice()
              .reverse()
              .map((item) => (
                <QrScanResultCard key={item.scannedAt} result={item} />
              ))}
          </Grid>
        </Box>
      </Box>
    </Box>
  );
};

export default QrCodeScannerPage;

