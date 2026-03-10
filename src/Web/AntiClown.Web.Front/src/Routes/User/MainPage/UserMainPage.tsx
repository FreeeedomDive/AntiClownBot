import React, { useEffect, useState } from "react";
import { Box } from "@mui/material";
import UserPageSideBar from "../SideBar/UserPageSideBar";
import UserPageContentRouter from "../PageContent/UserPageContentRouter";
import UsersApi from "../../../Api/UsersApi";
import { UserDto } from "../../../Dto/Users/UserDto";
import { Navigate, useParams } from "react-router-dom";
import RightsApi from "../../../Api/RightsApi";
import { useStore } from "../../../Stores";

export default function UserMainPage() {
  const { authStore, rightsStore } = useStore();
  const { userId = "" } = useParams<"userId">();
  const sideBarWidth = 250;
  const [user, setUser] = useState<UserDto | null | undefined>(undefined);
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
    <Box sx={{ display: "flex", minHeight: "100vh" }}>
      <Box
        component="nav"
        sx={{
          width: { sm: sideBarWidth },
          flexShrink: { sm: 0 },
          bgcolor: "#000000",
          position: "sticky",
          top: 0,
          height: "100vh",
          overflowY: "auto",
        }}
      >
        <UserPageSideBar user={user} />
      </Box>

      <Box
        component="main"
        sx={{
          flexGrow: 1,
          p: "16px",
          width: { sm: `calc(100% - ${sideBarWidth}px)` },
        }}
      >
        <UserPageContentRouter user={user} />
      </Box>
    </Box>
  );
}
