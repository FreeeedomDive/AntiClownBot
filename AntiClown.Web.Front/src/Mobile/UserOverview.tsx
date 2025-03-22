import { useStore } from "../Stores";
import React from "react";
import { Avatar, Stack, Typography } from "@mui/material";

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
      <Avatar
        alt={discordMember.serverName ?? discordMember.userName ?? "Discord profile pic"}
        src={discordMember.avatarUrl}
        sx={{ width: 128, height: 128 }}
      />
      <Stack direction={"column"} spacing="8px">
        {discordMember.serverName && (
          <Typography variant={"body1"}>
            <b>Имя на сервере:</b> {discordMember.serverName}
          </Typography>
        )}
        {discordMember.userName && (
          <Typography variant={"body1"}>
            <b>Имя в дискорде:</b> {discordMember.userName}
          </Typography>
        )}
      </Stack>
    </Stack>
  );
}
