import React from "react";
import { Stack } from "@mui/material";
import UserAchievements from "./UserAchievements";
import MemberInfo from "./MemberInfo";

export default function UserOverview() {
  return (
    <Stack direction={"column"} spacing="16px">
      <MemberInfo />
      <UserAchievements />
    </Stack>
  );
}
