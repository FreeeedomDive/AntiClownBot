import {useParams} from "react-router-dom";
import React, {useEffect, useState} from "react";
import {DiscordMemberDto} from "../../../Dto/Users/DiscordMemberDto";
import DiscordMembersApi from "../../../Api/DiscordMembersApi";
import {Avatar} from "@mui/material";

export default function UserOverview(){
  const { userId = "" } = useParams<"userId">();
  const [user, setUser] = useState<DiscordMemberDto | undefined>(undefined);

  async function updateUser(): Promise<void> {
    const user = await DiscordMembersApi.getMember(userId);
    setUser(user)
  }

  useEffect(() => {
    updateUser().catch(console.error)
  }, []);

  return <div>
    {
      user && (
        <div>
          <Avatar
            alt="Discord profile pic"
            src={user.avatarUrl}
            sx={{ width: 128, height: 128 }}
          />
          <div>Имя на сервере: {user.serverName}</div>
          <div>Имя в дискорде: {user.userName}</div>
        </div>
      )
    }
  </div>;
}