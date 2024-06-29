import {useParams} from "react-router-dom";
import React, {useEffect, useState} from "react";
import {DiscordMemberDto} from "../../../../Dto/Users/DiscordMemberDto";
import DiscordMembersApi from "../../../../Api/DiscordMembersApi";
import {Avatar, Stack, Typography} from "@mui/material";
import { Loader } from "../../../../Components/Loader/Loader";

export default function UserOverview() {
  const {userId = ""} = useParams<"userId">();
  const [member, setMember] = useState<DiscordMemberDto | undefined>(undefined);
  const [loading, setLoading] = useState(true);

  async function updateUser(): Promise<void> {
    const member = await DiscordMembersApi.getMember(userId);
    setMember(member)
  }

  useEffect(() => {
    updateUser().catch(console.error).finally(() => setLoading(false))
  }, []);

  return <div>
    {
      loading && (
        <Loader />
      )
    }
    {
      member && (
        <Stack direction={"row"} alignItems={"center"} spacing="16px">
          <Avatar
            alt="Discord profile pic"
            src={member.avatarUrl}
            sx={{width: 128, height: 128}}
          />
          <Stack direction={"column"} spacing="8px">
            {member.serverName && <Typography variant={"h5"}>Имя на сервере: {member.serverName}</Typography>}
            {member.userName && <Typography variant={"h5"}>Имя в дискорде: {member.userName}</Typography>}
          </Stack>
        </Stack>
      )
    }
  </div>;
}