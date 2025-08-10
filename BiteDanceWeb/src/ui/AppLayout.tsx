import { NavLink, Outlet } from "react-router-dom";
import { useAuth } from "react-oidc-context";
import $api from "../services/api";
import {
  AppBar,
  Toolbar,
  Button,
  IconButton,
  Box,
  Menu,
  MenuItem,
  BottomNavigation,
  BottomNavigationAction,
  Paper,
} from "@mui/material";
import AccountCircleIcon from "@mui/icons-material/AccountCircle";
import GlobalLoading from "./GlobalLoading";
import React from "react";
import { useMediaQuery, useTheme } from "@mui/material";
import { Person, QrCode, RestaurantMenu } from "@mui/icons-material";

const navLinks = [
  { name: "Locations", link: "/locations", requireSuperAdmin: true  },
  { name: "Suppliers", link: "/suppliers", requireSuperAdmin: true  },
  { name: "Admins", link: "/admins", requireSuperAdmin: true  },
  { name: "Requests", link: "/red-code-requests", requireAdmin: true},
  {
    name: "Requests Summary",
    link: "/red-code-requests-summary",
    requireAdmin: true,
  },
  { name: "Reports", link: "/weekly-report", requireAdmin: true},
  { name: "Menu", link: "/setup-monthly-menu", requireAdmin: true},
  { name: "QR Scanner", link: "/qr-scanner", requireSupplier: true },
  { name: "My QR", link: "/my-qr" },
  { name: "Order", link: "/order" },
  
  //{ name: "External Request", link: "/external" },
  // { name: "Test", link: "/test" },
];

export default function AppLayout() {
  const user = $api.useQuery("get", "/swagger/Users/me", {
    query: { includeAssignedLocations: true },
  });
  const auth = useAuth();
  const [profilePopupAnchor, setProfilePopupAnchor] =
    React.useState<null | HTMLElement>(null);
  const isProfilePopupOpen = Boolean(profilePopupAnchor);
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down("sm"));
  const [bottomNavValue, setBottomNavValue] = React.useState(0);

  const handleCloseProfilePopup = () => {
    setProfilePopupAnchor(null);
  };

  return (
    <>
      <GlobalLoading />
      {/* // Show app bar on desktop, bottom nav on mobile */}
      {!isMobile ? (
        <AppBar position="static" sx={{ zIndex: 1000 }}>
          <Toolbar>
            <img src={"/logo.png"} alt="logo" />
            <Box
              style={{
                marginLeft: "auto",
                display: "flex",
                alignItems: "center",
              }}
            >
              {navLinks
                .filter((i) => {
                    const isSupplier = user.data?.isSupplier;

                    // Nếu là supplier → chỉ hiện QR Scanner
                    if (isSupplier) {
                      return i.link === "/qr-scanner";
                    }

                    // Nếu không phải supplier → lọc theo quyền admin/superadmin như cũ
                    return (
                      (!i.requireSuperAdmin || user.data?.isSuperAdmin) &&
                      (!i.requireAdmin || user.data?.isAdmin) && !i.requireSupplier
                    );
                  })
                .map((i) => (
                  <Button
                    color="inherit"
                    key={i.name}
                    component={NavLink}
                    to={i.link}
                    // @ts-expect-error forward props
                    style={({ isActive }) => ({
                      fontWeight: isActive ? "bold" : "normal",
                      borderBottom: isActive ? "2px solid white" : "none",
                      borderRadius: "0px",
                    })}
                  >
                    {i.name}
                  </Button>
                ))}
              <IconButton
                color="inherit"
                onClick={(e) => setProfilePopupAnchor(e.currentTarget)}
              >
                <AccountCircleIcon />
              </IconButton>
              <Menu
                anchorEl={profilePopupAnchor}
                open={isProfilePopupOpen}
                onClose={handleCloseProfilePopup}
              >
                {/* <MenuItem>
                  <Debug obj={user.data}>User</Debug>
                </MenuItem> */}
                <MenuItem
                  component={NavLink}
                  to="/update-profile"
                  onClick={handleCloseProfilePopup}
                >
                  <Person style={{ marginRight: 8 }} />
                  Update Profile
                </MenuItem>
                <MenuItem
                  onClick={() => {
                    auth.removeUser();
                    handleCloseProfilePopup();
                  }}
                >
                  <AccountCircleIcon style={{ marginRight: 8 }} />
                  Log out
                </MenuItem>
              </Menu>
            </Box>
          </Toolbar>
        </AppBar>
      ) : (
        <Paper
          sx={{
            position: "fixed",
            bottom: 0,
            left: 0,
            right: 0,
            zIndex: 1000,
          }}
          elevation={3}
        >
          <BottomNavigation
            value={bottomNavValue}
            onChange={(_, newValue) => {
              setBottomNavValue(newValue);
            }}
            showLabels
            sx={{
              backgroundColor: "secondary.main",
            }}
          >
            <BottomNavigationAction
              label="Order"
              icon={<RestaurantMenu />}
              component={NavLink}
              to="/order"
            />
            <BottomNavigationAction
              label="My QR"
              icon={<QrCode />}
              component={NavLink}
              to="/my-qr"
            />
            <BottomNavigationAction
              label="Settings"
              icon={<Person />}
              component={NavLink}
              to="/update-profile"
            />
          </BottomNavigation>
        </Paper>
      )}
      <Box padding={2} marginBottom={isMobile ? 10 : 0}>
        {/* extra margin for mobile bottom nav */}
        <Outlet />
      </Box>
    </>
  );
}
