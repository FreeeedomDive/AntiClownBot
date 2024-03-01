import React, { useEffect, useState } from "react";
import { Box } from "@mui/material";
import UserPageSideBar from "../SIdeBar/UserPageSideBar";
import UserPageContent from "../PageContent/UserPageContent";
import UsersApi from "../../../Api/UsersApi";
import { UserDto } from "../../../Dto/Users/UserDto";
import { useParams } from "react-router-dom";

const UserMainPage = () => {
  const { userId = "" } = useParams<"userId">();
  const sideBarWidth = 250;
  const [user, setUser] = useState<UserDto | null | undefined>(undefined);

  useEffect(() => {
    async function updateUser(userId: string): Promise<void> {
      const user = await UsersApi.get(userId);
      setUser(user);
    }

    updateUser(userId).catch(console.error);
  }, [userId]);

  return (
    <Box sx={{ display: "flex", minHeight: "100vh" }}>
      <Box
        component="nav"
        sx={{
          width: { sm: sideBarWidth },
          borderRight: 1,
          borderColor: "primary.main",
          flexShrink: { sm: 0 },
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
        <UserPageContent user={user} />
      </Box>
    </Box>
  );
};

export default UserMainPage;
