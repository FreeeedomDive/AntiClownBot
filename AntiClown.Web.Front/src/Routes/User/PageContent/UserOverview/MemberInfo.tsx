import { Avatar, Stack, Typography } from "@mui/material";
import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { DiscordMemberDto } from "../../../../Dto/Users/DiscordMemberDto";
import DiscordMembersApi from "../../../../Api/DiscordMembersApi";
import { Skeleton } from "@mui/material";

export default function MemberInfo() {
  const { userId = "" } = useParams<"userId">();
  const [member, setMember] = useState<DiscordMemberDto | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function updateUser(): Promise<void> {
      const member = await DiscordMembersApi.getMember(userId);
      setMember(member);
    }

    updateUser()
      .catch(console.error)
      .finally(() => setLoading(false));
  }, [userId]);

  return (
    <>
      {loading && (
        <Stack direction="row" alignItems="center" spacing={16}>
          <Skeleton variant="rounded" sx={{ width: 128, height: 128 }} />
          <Stack direction="column" spacing="8px">
            <Skeleton sx={{ width: 256 }} />
            <Skeleton sx={{ width: 256 }} />
          </Stack>
        </Stack>
      )}
      {!loading && member && (
        <Stack direction="row" alignItems="center" spacing="16px">
          <Avatar
            alt="Discord profile pic"
            src={member.avatarUrl}
            sx={{ width: 128, height: 128 }}
          />
          <Stack direction="column" spacing="8px">
            {member.serverName && (
              <Typography variant="h5">
                Имя на сервере: {member.serverName}
              </Typography>
            )}
            {member.userName && (
              <Typography variant="h5">
                Имя в дискорде: {member.userName}
              </Typography>
            )}
          </Stack>
        </Stack>
      )}
    </>
  );
}
