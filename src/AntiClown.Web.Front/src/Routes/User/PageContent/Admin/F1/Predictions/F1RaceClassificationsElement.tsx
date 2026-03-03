import React from "react";
import {
  Checkbox,
  FormControlLabel,
  TableCell,
  TableRow,
  Typography,
  IconButton,
} from "@mui/material";
import DragIndicatorIcon from "@mui/icons-material/DragIndicator";
import { DraggableProvided } from "@hello-pangea/dnd";

interface Props {
  f1Driver: string;
  index: number;
  isDnf: boolean;
  onToggleDnf: (checked: boolean) => void;
  provided: DraggableProvided;
}

export default function F1RaceClassificationsElement({
  f1Driver,
  index,
  isDnf,
  onToggleDnf,
  provided,
}: Props) {
  const position = index + 1;

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
        <Typography>{position}</Typography>
      </TableCell>

      <TableCell sx={{ padding: "1px", width: "100%" }}>
        <Typography>{f1Driver}</Typography>
      </TableCell>

      <TableCell sx={{ padding: "1px", width: "20%" }}>
        <FormControlLabel
          control={
            <Checkbox
              checked={isDnf}
              onChange={(x) => onToggleDnf(x.target.checked)}
            />
          }
          label={"DNF"}
        />
      </TableCell>
    </TableRow>
  );
}
