import {
  Box,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
} from "@mui/material";
import React from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { MAIN_COLOR } from "../../../Helpers/Colors";

interface Props {
  sidebarKey: string;
  link: string;
  text: string;
  icon?: React.ReactNode;
  nesting?: number;
  showBadge?: boolean;
  onClick?: (() => void) | null;
}

export default function UserPageSideBarItem({
  sidebarKey,
  link,
  text,
  icon,
  nesting = 1,
  showBadge = false,
  onClick = null,
}: Props) {
  const navigate = useNavigate();
  const location = useLocation();

  return (
    <ListItem key={sidebarKey} disablePadding>
      <ListItemButton
        sx={{
          pl: nesting * 2,
          mx: 1,
          borderRadius: 1.5,
          "&.Mui-selected": {
            bgcolor: MAIN_COLOR,
            "&:hover": { bgcolor: MAIN_COLOR },
          },
        }}
        onClick={onClick ?? (() => navigate(link))}
        selected={location.pathname === link}
      >
        {icon && (
          <ListItemIcon sx={{ minWidth: 32, "& .MuiSvgIcon-root": { fontSize: 18 } }}>
            {icon}
          </ListItemIcon>
        )}
        <ListItemText
          primary={text}
          primaryTypographyProps={{ fontSize: "0.82rem" }}
        />
        {showBadge && (
          <Box
            sx={{
              width: 7,
              height: 7,
              borderRadius: "50%",
              bgcolor: "warning.main",
              flexShrink: 0,
              ml: 1,
            }}
          />
        )}
      </ListItemButton>
    </ListItem>
  );
}
