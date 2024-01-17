import {useParams} from "react-router-dom";
import React, {useEffect, useState} from "react";
import {DiscordMemberDto} from "../../../../Dto/Users/DiscordMemberDto";
import DiscordMembersApi from "../../../../Api/DiscordMembersApi";
import {Avatar, CircularProgress, Stack, Typography} from "@mui/material";
import "./UserOverview.css";

export default function UserOverview() {
  const {userId = ""} = useParams<"userId">();
  const [user, setUser] = useState<DiscordMemberDto | undefined>(undefined);
  const [loading, setLoading] = useState(true);

  async function updateUser(): Promise<void> {
    const user = await DiscordMembersApi.getMember(userId);
    setUser(user)
  }

  useEffect(() => {
    updateUser().catch(console.error).finally(() => setLoading(false))
  }, []);

  return <div>
    {
      loading && (
        <div className={"loadingContainer"}>
          <CircularProgress color="inherit" size={64}/>
        </div>
      )
    }
    {
      user && (
        <Stack direction={"row"} alignItems={"center"} spacing="16px">
          <Avatar
            alt="Discord profile pic"
            src={user.avatarUrl}
            sx={{width: 128, height: 128}}
          />
          <Stack direction={"column"} spacing="8px">
            {user.serverName && < Typography variant={"h5"}>Имя на сервере: {user.serverName}</Typography>}
            {user.userName && <Typography variant={"h5"}>Имя в дискорде: {user.userName}</Typography>}
          </Stack>
        </Stack>
      )
    }
  </div>;
}