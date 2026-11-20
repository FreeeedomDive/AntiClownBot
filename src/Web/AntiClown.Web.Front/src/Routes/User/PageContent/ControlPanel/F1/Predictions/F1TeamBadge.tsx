import React, { useContext } from "react";
import { Box } from "@mui/material";
import { F1TeamDto } from "../../../../../../Dto/F1Predictions/F1TeamDto";
import {
  getTeamForDriver,
  getTeamInfo,
} from "../../../../../../Dto/F1Predictions/F1DriversHelpers";
import F1RaceSeasonContext from "./F1RaceSeasonContext";

interface Props {
  driver: string;
  teams: F1TeamDto[];
  formerTeam?: boolean;
}

export default function F1TeamBadge({ driver, teams, formerTeam }: Props) {
  const season = useContext(F1RaceSeasonContext);
  const team = getTeamForDriver(driver, teams);
  if (!team) return null;

  if (
    !team.badge &&
    season !== undefined &&
    season !== new Date().getFullYear()
  )
    return null;
  const info = team.badge
    ? {
        trigram: team.badge,
        color: team.backgroundColor,
        textColor: team.textColor,
      }
    : getTeamInfo(team.name);
  if (!info) return null;

  return (
    <Box
      component="span"
      title={formerTeam ? `${team.name} · не в текущем составе` : undefined}
      sx={{
        display: "inline-flex",
        alignItems: "center",
        justifyContent: "center",
        backgroundColor: info.color,
        opacity: formerTeam ? 0.6 : undefined,
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
