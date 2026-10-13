import React from "react";
import { Box } from "@mui/material";
import { F1TeamDto } from "../../../../../../../Dto/F1Predictions/F1TeamDto";
import F1TeamBadge from "../F1TeamBadge";

interface Props {
  driver: string;
  teams: F1TeamDto[];
  qualifyingGrid?: string[];
}

export default function F1DriverOption({
  driver,
  teams,
  qualifyingGrid,
}: Props) {
  const position = (qualifyingGrid?.indexOf(driver) ?? -1) + 1;

  return (
    <Box sx={{ display: "flex", gap: 1, alignItems: "center", minWidth: 0 }}>
      {position > 0 && (
        <Box
          component="span"
          sx={{
            minWidth: 30,
            px: 0.5,
            py: 0.125,
            borderRadius: 0.5,
            bgcolor: "rgba(144,202,249,0.1)",
            color: "primary.main",
            textAlign: "center",
            fontSize: "0.7rem",
            fontVariantNumeric: "tabular-nums",
          }}
        >
          P{position}
        </Box>
      )}
      <F1TeamBadge driver={driver} teams={teams} />
      <Box
        component="span"
        sx={{
          overflow: "hidden",
          textOverflow: "ellipsis",
          whiteSpace: "nowrap",
        }}
      >
        {driver}
      </Box>
    </Box>
  );
}
