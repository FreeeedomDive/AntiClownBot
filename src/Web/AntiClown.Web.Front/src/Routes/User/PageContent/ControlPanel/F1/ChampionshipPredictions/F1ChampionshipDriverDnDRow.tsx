import React from "react";
import {
  Box,
  Chip,
  IconButton,
  TableCell,
  TableRow,
  Typography,
} from "@mui/material";
import DragIndicatorIcon from "@mui/icons-material/DragIndicator";
import { DraggableProvided } from "@hello-pangea/dnd";
import { F1TeamDto } from "../../../../../../Dto/F1Predictions/F1TeamDto";
import F1TeamBadge from "../Predictions/F1TeamBadge";

interface Props {
  driver: string;
  index: number;
  draggable: boolean;
  teams: F1TeamDto[];
  provided?: DraggableProvided | null;
  points?: number;
}

export default function F1ChampionshipDriverDnDRow({
  driver,
  index,
  draggable,
  teams,
  provided = null,
  points,
}: Props) {
  return (
    <TableRow ref={provided?.innerRef} {...provided?.draggableProps}>
      <TableCell
        sx={{ padding: "1px", width: "10%" }}
        {...provided?.dragHandleProps}
      >
        <IconButton
          size="small"
          sx={{ cursor: "grab", opacity: draggable ? 1 : 0.2 }}
        >
          <DragIndicatorIcon />
        </IconButton>
      </TableCell>
      <TableCell sx={{ padding: "1px", width: "5%" }}>
        <Typography>{index + 1}</Typography>
      </TableCell>
      <TableCell sx={{ padding: "1px", width: "100%" }}>
        <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
          <F1TeamBadge driver={driver} teams={teams} />
          <Typography>{driver}</Typography>
        </Box>
      </TableCell>
      <TableCell sx={{ padding: "1px", width: "10%", textAlign: "right" }}>
        {points !== undefined && points > 0 && (
          <Chip
            label={`+${points}`}
            size="small"
            color="success"
            sx={{ fontWeight: "bold" }}
          />
        )}
      </TableCell>
    </TableRow>
  );
}
