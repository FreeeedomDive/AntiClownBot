import { DiscordMemberDto } from "../../Dto/Users/DiscordMemberDto";
import { Typography } from "@mui/material";
import { useNavigate } from "react-router-dom";

interface IProps {
  member: DiscordMemberDto;
}

export default function DiscordMember({ member }: IProps) {
  const navigate = useNavigate();
  return (
    <Typography
      onClick={() => navigate(`/user/${member.userId}`)}
      sx={{
        "&:hover": {
          textDecoration: "underline",
          cursor: "pointer",
        },
      }}
    >
      {member.serverName ?? member.userName}
    </Typography>
  );
}
