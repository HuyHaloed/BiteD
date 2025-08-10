import { Box } from "@mui/material";
import { useState } from "react";
import {
  Scanner as ScannerComp,
  boundingBox,
  useDevices,
  IDetectedBarcode,
} from "@yudiel/react-qr-scanner";

export default function QrScanner({
  onScan,
  paused,
}: {
  onScan: (codes: IDetectedBarcode[]) => void;
  paused: boolean;
}) {
  const [deviceId, setDeviceId] = useState<string | undefined>(undefined);

  const devices = useDevices();
  return (
    <Box>
      <Box sx={{ display: "flex", justifyContent: "center" }}>
        <select onChange={(e) => setDeviceId(e.target.value)}>
          <option value={undefined}>Change camera</option>
          {
          devices.map((device, index) => (
            <option key={index} value={device.deviceId}>
              {device.label}
            </option>
          ))
          }
        </select>
      </Box>
      <ScannerComp
        formats={["qr_code"]}
        constraints={{
          deviceId: deviceId,
        }}
        onScan={(detectedCodes) => {
          onScan(detectedCodes);
        }}
        onError={(error) => {
          console.log(`onError: ${error}'`);
        }}
        components={{
          audio: true,
          onOff: true,
          torch: true,
          zoom: true,
          finder: true,
          tracker: boundingBox,
        }}
        allowMultiple={true}
        scanDelay={2000}
        paused={paused}
      />
    </Box>
  );
}
