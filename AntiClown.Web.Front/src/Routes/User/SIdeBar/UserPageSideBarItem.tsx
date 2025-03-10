import { Badge, ListItem, ListItemButton, ListItemText } from "@mui/material";
import React from "react";
import { useLocation, useNavigate } from "react-router-dom";

interface Props {
  key: string;
  link: string;
  text: string;
  nesting?: number;
  showBadge?: boolean;
  onClick?: (() => void) | null;
}

export default function UserPageSideBarItem({
  key,
  link,
  text,
  nesting = 1,
  showBadge = false,
  onClick = null
}: Props) {
  const navigate = useNavigate();
  const location = useLocation();

  return (
    <ListItem key={key} disablePadding>
      <ListItemButton
        sx={{ pl: nesting * 2 }}
        onClick={onClick ?? (() => navigate(link))}
        selected={location.pathname === link}
      >
        {showBadge ? (
          <Badge
            anchorOrigin={{
              vertical: "top",
              horizontal: "left",
            }}
            variant="dot"
            color="warning"
          >
            <ListItemText primary={text} />
          </Badge>
        ) : (
          <ListItemText primary={text} />
        )}
      </ListItemButton>
    </ListItem>
  );
}
