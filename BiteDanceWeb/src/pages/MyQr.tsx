import React, { useState } from "react";
import { Tabs, Tab, Box } from "@mui/material";
import GreenQr from "../features/my-qr/GreenQr";
import GuestManagement from "../features/my-qr/GuestManagement";
import BlueQr from "../features/my-qr/BlueQr";
import $api from "../services/api";

interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

function TabPanel(props: TabPanelProps) {
  const { children, value, index, ...other } = props;

  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`simple-tabpanel-${index}`}
      aria-labelledby={`simple-tab-${index}`}
      {...other}
    >
      {value === index && <Box sx={{ p: 3 }}>{children}</Box>}
    </div>
  );
}

function a11yProps(index: number) {
  return {
    id: `simple-tab-${index}`,
    "aria-controls": `simple-tabpanel-${index}`,
  };
}

export default function MyQr() {
  const [value, setValue] = useState(0);
  const me = $api.useQuery("get", "/swagger/Users/me");

  return (
    <Box sx={{ width: "100%" }}>
      <Box sx={{ borderBottom: 1, borderColor: "divider" }}>
        <Tabs
          value={value}
          onChange={(_, newValue) => setValue(newValue)}
          aria-label="qr tabs"
          variant="fullWidth"
        >
          <Tab label="PRE-ORDER" {...a11yProps(0)} />
          <Tab label="PERSONAL" {...a11yProps(1)} />
          {me.data?.isManager && <Tab label="GUEST" {...a11yProps(2)} />}
        </Tabs>
      </Box>
      <TabPanel value={value} index={0}>
        <GreenQr />
      </TabPanel>
      <TabPanel value={value} index={1}>
        <BlueQr />
      </TabPanel>
      {me.data?.isManager && (
        <TabPanel value={value} index={2}>
          <GuestManagement />
        </TabPanel>
      )}
    </Box>
  );
}
