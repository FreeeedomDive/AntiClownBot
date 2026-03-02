import { useStore } from "../Stores";
import React from "react";
import { Avatar, Stack, Typography } from "@mui/material";
import MemberInfo from "../Routes/User/PageContent/UserOverview/MemberInfo";
import UserAchievements from "../Routes/User/PageContent/UserOverview/UserAchievements";

export default function UserOverview() {
  const { mobileUserContextStore } = useStore();
  const { discordMember, telegramUser } = mobileUserContextStore;

  return mobileUserContextStore.isUnknown || !telegramUser || !discordMember ? (
    <></>
  ) : (
    <Stack
      direction={"row"}
      alignItems={"center"}
      spacing="8px"
      sx={{
        marginLeft: "16px",
        marginTop: "8px",
        marginBottom: "8px",
        marginRight: "16px",
      }}
    >
      <MemberInfo />
      <UserAchievements />
    </Stack>
  );
}
