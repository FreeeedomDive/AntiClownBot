import React from "react";
import { IconButton, TableCell, TableRow, Typography } from "@mui/material";
import DragIndicatorIcon from "@mui/icons-material/DragIndicator";
import { DraggableProvided } from "@hello-pangea/dnd";

interface Props {
  driver: string;
  index: number;
  provided: DraggableProvided;
}

export default function F1ChampionshipDriverDnDRow({
  driver,
  index,
  provided,
}: Props) {
  return (
    <TableRow ref={provided.innerRef} {...provided.draggableProps}>
      <TableCell
        sx={{ padding: "1px", width: "10%" }}
        {...provided.dragHandleProps}
      >
        <IconButton size="small" sx={{ cursor: "grab" }}>
          <DragIndicatorIcon />
        </IconButton>
      </TableCell>
      <TableCell sx={{ padding: "1px", width: "5%" }}>
        <Typography>{index + 1}</Typography>
      </TableCell>
      <TableCell sx={{ padding: "1px", width: "100%" }}>
        <Typography>{driver}</Typography>
      </TableCell>
    </TableRow>
  );
}
