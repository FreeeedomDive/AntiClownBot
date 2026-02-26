import React from "react";
import { Chip, IconButton, TableCell, TableRow, Typography } from "@mui/material";
import DragIndicatorIcon from "@mui/icons-material/DragIndicator";
import { DraggableProvided } from "@hello-pangea/dnd";

interface Props {
  driver: string;
  index: number;
  draggable: boolean;
  provided?: DraggableProvided | null;
  points?: number;
}

export default function F1ChampionshipDriverDnDRow({
  driver,
  index,
  draggable,
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
        <Typography>{driver}</Typography>
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
