import { useStore } from "../Stores";
import React from "react";
import { Stack } from "@mui/material";
import MemberInfo from "../Routes/User/PageContent/UserOverview/MemberInfo";
import UserAchievements from "../Routes/User/PageContent/UserOverview/UserAchievements";

export default function UserOverview() {
  const { mobileUserContextStore } = useStore();
  const { discordMember, telegramUser, user } = mobileUserContextStore;

  const unknownUser = Boolean(
    mobileUserContextStore.isUnknown ||
      !telegramUser ||
      !discordMember ||
      !user,
  );

  return unknownUser ? (
    <></>
  ) : (
    <Stack direction={"column"} spacing="16px">
      <MemberInfo userId={user!.id} />
      <UserAchievements userId={user!.id} />
    </Stack>
  );
}
