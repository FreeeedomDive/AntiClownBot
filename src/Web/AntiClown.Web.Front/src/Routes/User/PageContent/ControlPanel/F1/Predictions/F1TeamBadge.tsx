import React from "react";
import { Box } from "@mui/material";
import { F1TeamDto } from "../../../../../../Dto/F1Predictions/F1TeamDto";
import { getTeamForDriver, getTeamInfo } from "../../../../../../Dto/F1Predictions/F1DriversHelpers";

interface Props {
  driver: string;
  teams: F1TeamDto[];
}

export default function F1TeamBadge({ driver, teams }: Props) {
  const team = getTeamForDriver(driver, teams);
  if (!team) return null;

  const info = getTeamInfo(team.name);
  if (!info) return null;

  return (
    <Box
      component="span"
      sx={{
        display: "inline-flex",
        alignItems: "center",
        justifyContent: "center",
        backgroundColor: info.color,
        color: info.textColor ?? "#fff",
        borderRadius: "4px",
        px: 0.75,
        py: 0.125,
        fontSize: "0.6rem",
        fontWeight: "bold",
        letterSpacing: "0.05em",
        lineHeight: 1.5,
        flexShrink: 0,
        userSelect: "none",
      }}
    >
      {info.trigram}
    </Box>
  );
}
