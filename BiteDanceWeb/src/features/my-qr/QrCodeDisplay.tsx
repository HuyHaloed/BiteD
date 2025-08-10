import QRCode from "react-qr-code";
import { useRef } from "react";
import { Stack, Button } from "@mui/material";
import DownloadIcon from "@mui/icons-material/Download";

interface QrCodeProps {
  value: string;
  fgColor: string;
  fileName: string;
}

interface GreenQrCodeDisplayProps {
  userId: string;
}

interface BlueQrCodeDisplayProps {
  userId: string;
}

function QrCodeWidget({ value, fgColor, fileName: type }: QrCodeProps) {
  const qrRef = useRef<HTMLDivElement>(null);

  const downloadQrCode = () => {
  if (qrRef.current) {
    const svg = qrRef.current.querySelector("svg");
    if (svg) {
      const svgData = new XMLSerializer().serializeToString(svg);
      const canvas = document.createElement("canvas");
      const ctx = canvas.getContext("2d");
      const img = new Image();

      img.onload = () => {
        const padding = 10; // số pixel padding (viền trắng)
        const qrWidth = img.width;
        const qrHeight = img.height;

        canvas.width = qrWidth + padding * 2;
        canvas.height = qrHeight + padding * 2;

        if (ctx) {
          // Vẽ nền trắng toàn bộ canvas
          ctx.fillStyle = "#FFFFFF";
          ctx.fillRect(0, 0, canvas.width, canvas.height);

          // Vẽ QR code vào chính giữa canvas (thêm viền trắng)
          ctx.drawImage(img, padding, padding);
        }

        // Xuất file PNG
        const pngFile = canvas.toDataURL("image/png");
        const downloadLink = document.createElement("a");
        downloadLink.href = pngFile;
        downloadLink.download = `${type}-qr-code.png`;
        downloadLink.click();
      };

      img.src = `data:image/svg+xml;base64,${btoa(unescape(encodeURIComponent(svgData)))}`;
    }
  }
};


  return (
        <Stack ref={qrRef} direction="column" spacing={2} alignItems="center">
      <QRCode value={value} fgColor={fgColor} bgColor="#FFFFFF" // Đảm bảo nền trắng
  style={{ background: "#fff"}} />
      <Button
        variant="outlined"
        onClick={downloadQrCode}
        startIcon={<DownloadIcon />}
      >
        Download QR Code
      </Button>
    </Stack>
  );
}

export function GreenQrCodeDisplay({ userId }: GreenQrCodeDisplayProps) {
  return (
    <QrCodeWidget value={`g:${userId}`} fgColor="#00712D" fileName="green" />
  );
}

export function BlueQrCodeDisplay({ userId }: BlueQrCodeDisplayProps) {
  return (
    <QrCodeWidget value={`b:${userId}`} fgColor="#2525a8" fileName="blue" />
  );
}
