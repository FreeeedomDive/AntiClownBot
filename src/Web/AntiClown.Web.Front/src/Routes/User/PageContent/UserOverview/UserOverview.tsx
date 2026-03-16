import React from "react";
import { Stack } from "@mui/material";
import UserAchievements from "./UserAchievements";
import MemberInfo from "./MemberInfo";
import { useParams } from "react-router-dom";

export default function UserOverview() {
  const { userId = "" } = useParams<"userId">();
  return (
    <Stack direction={"column"} spacing={2}>
      <MemberInfo userId={userId} />
      <UserAchievements userId={userId} />
    </Stack>
  );
}
