import { Avatar, Skeleton, Stack, Typography } from "@mui/material";
import React, { useEffect, useState } from "react";
import { DiscordMemberDto } from "../../../../Dto/Users/DiscordMemberDto";
import DiscordMembersApi from "../../../../Api/DiscordMembersApi";

interface Props {
  userId: string;
}

export default function MemberInfo({ userId }: Props) {
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
        <Stack direction="row" alignItems="center" spacing="16px">
          <Skeleton
            variant="rounded"
            sx={{ width: { xs: 80, sm: 128 }, height: { xs: 80, sm: 128 } }}
          />
          <Stack direction="column" spacing="8px" sx={{ minWidth: 0 }}>
            <Skeleton sx={{ width: { xs: 160, sm: 256 }, maxWidth: "100%" }} />
            <Skeleton sx={{ width: { xs: 160, sm: 256 }, maxWidth: "100%" }} />
          </Stack>
        </Stack>
      )}
      {!loading && member && (
        <Stack direction="row" alignItems="center" spacing="16px">
          <Avatar
            alt="Discord profile pic"
            src={member.avatarUrl}
            sx={{
              width: { xs: 80, sm: 128 },
              height: { xs: 80, sm: 128 },
              flexShrink: 0,
            }}
          />
          <Stack direction="column" spacing="8px" sx={{ minWidth: 0 }}>
            {member.serverName && (
              <Typography
                variant="h5"
                sx={{
                  fontSize: { xs: "1rem", sm: "1.5rem" },
                  overflowWrap: "anywhere",
                }}
              >
                Имя на сервере: {member.serverName}
              </Typography>
            )}
            {member.userName && (
              <Typography
                variant="h5"
                sx={{
                  fontSize: { xs: "1rem", sm: "1.5rem" },
                  overflowWrap: "anywhere",
                }}
              >
                Имя в дискорде: {member.userName}
              </Typography>
            )}
          </Stack>
        </Stack>
      )}
    </>
  );
}
