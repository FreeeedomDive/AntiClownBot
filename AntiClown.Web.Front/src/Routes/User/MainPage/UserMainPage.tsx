import React from "react";
import { observer } from "mobx-react-lite";
import { Box } from "@mui/material";
import UserPageSideBar from "../SIdeBar/UserPageSideBar";
import UserPageContent from "../PageContent/UserPageContent";

const UserMainPage = observer(() => {
  const sideBarWidth = 250;
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
        aria-label="mailbox folders"
      >
        <UserPageSideBar />
      </Box>

      <Box
        component="main"
        sx={{
          flexGrow: 1,
          p: "16px",
          width: { sm: `calc(100% - ${sideBarWidth}px)` },
        }}
      >
        <UserPageContent />
      </Box>
    </Box>
  );
});

export default UserMainPage;
