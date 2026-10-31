import React, { useEffect, useState } from "react";
import {
  AppBar,
  Box,
  Drawer,
  IconButton,
  Toolbar,
  Typography,
} from "@mui/material";
import { Menu } from "@mui/icons-material";
import UserPageSideBar from "../SideBar/UserPageSideBar";
import UserPageContentRouter from "../PageContent/UserPageContentRouter";
import UsersApi from "../../../Api/UsersApi";
import { UserDto } from "../../../Dto/Users/UserDto";
import { Navigate, useParams } from "react-router-dom";
import RightsApi from "../../../Api/RightsApi";
import { useStore } from "../../../Stores";
import { SideBarProvider } from "../SideBar/SideBarContext";

const SIDE_BAR_WIDTH = 250;

export default function UserMainPage() {
  const { authStore, rightsStore } = useStore();
  const { userId = "" } = useParams<"userId">();
  const [user, setUser] = useState<UserDto | null | undefined>(undefined);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const hasToken = !!authStore.userToken;

  useEffect(() => {
    if (!hasToken) return;

    async function updateUser(userId: string): Promise<void> {
      const user = await UsersApi.get(userId);
      setUser(user);
    }

    async function getUserRights(userId: string): Promise<void> {
      const rights = await RightsApi.getUserRights(userId);
      rightsStore.setRights(rights);
    }

    updateUser(userId).catch(console.error);
    getUserRights(userId).catch(console.error);
  }, [hasToken, rightsStore, userId]);

  if (!hasToken) {
    return <Navigate to={`/auth`} />;
  }

  return (
    <SideBarProvider>
      <Box sx={{ minHeight: "100dvh" }}>
        <AppBar
          position="sticky"
          color="default"
          elevation={0}
          sx={{
            display: { xs: "block", md: "none" },
            bgcolor: "#000000",
            backgroundImage: "none",
          }}
        >
          <Toolbar variant="dense">
            <IconButton
              edge="start"
              color="inherit"
              aria-label="Открыть меню"
              onClick={() => setIsMobileMenuOpen(true)}
            >
              <Menu />
            </IconButton>
            <Typography variant="h6" component="div">
              Clown City
            </Typography>
          </Toolbar>
        </AppBar>

        <Drawer
          open={isMobileMenuOpen}
          onClose={() => setIsMobileMenuOpen(false)}
          ModalProps={{ keepMounted: true }}
          sx={{
            display: { xs: "block", md: "none" },
            "& .MuiDrawer-paper": {
              width: SIDE_BAR_WIDTH,
              bgcolor: "#000000",
              backgroundImage: "none",
            },
          }}
        >
          <Box onClick={() => setIsMobileMenuOpen(false)}>
            <UserPageSideBar user={user} />
          </Box>
        </Drawer>

        <Box sx={{ display: "flex", minHeight: { md: "100dvh" } }}>
          <Box
            component="nav"
            sx={{
              display: { xs: "none", md: "block" },
              width: SIDE_BAR_WIDTH,
              flexShrink: 0,
              bgcolor: "#000000",
              position: "sticky",
              top: 0,
              height: "100dvh",
              overflowY: "auto",
            }}
          >
            <UserPageSideBar user={user} />
          </Box>

          <Box
            component="main"
            sx={{
              flexGrow: 1,
              minWidth: 0,
              p: { xs: 1, sm: 2 },
              width: { xs: "100%", md: `calc(100% - ${SIDE_BAR_WIDTH}px)` },
            }}
          >
            <UserPageContentRouter user={user} />
          </Box>
        </Box>
      </Box>
    </SideBarProvider>
  );
}
