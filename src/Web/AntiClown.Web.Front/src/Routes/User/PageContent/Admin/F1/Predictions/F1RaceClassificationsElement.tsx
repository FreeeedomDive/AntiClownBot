import React from "react";
import {
  Box,
  Checkbox,
  FormControlLabel,
  TableCell,
  TableRow,
  Typography,
  IconButton,
} from "@mui/material";
import DragIndicatorIcon from "@mui/icons-material/DragIndicator";
import { DraggableProvided } from "@hello-pangea/dnd";
import { F1TeamDto } from "../../../../../../Dto/F1Predictions/F1TeamDto";
import F1TeamBadge from "../../../ControlPanel/F1/Predictions/F1TeamBadge";

interface Props {
  f1Driver: string;
  index: number;
  isDnf: boolean;
  onToggleDnf: (checked: boolean) => void;
  provided: DraggableProvided;
  teams: F1TeamDto[];
  showDnf?: boolean;
}

export default function F1RaceClassificationsElement({
  f1Driver,
  index,
  isDnf,
  onToggleDnf,
  provided,
  teams,
  showDnf = true,
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
        <Typography variant="body2">{position}</Typography>
      </TableCell>

      <TableCell sx={{ padding: "1px", width: "100%" }}>
        <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
          <F1TeamBadge driver={f1Driver} teams={teams} />
          <Typography variant="body2">{f1Driver}</Typography>
        </Box>
      </TableCell>

      {showDnf && (
        <TableCell sx={{ padding: "1px", width: "20%" }}>
          <FormControlLabel
            sx={{ mr: 0 }}
            control={
              <Checkbox
                size="small"
                sx={{ p: "4px" }}
                checked={isDnf}
                onChange={(x) => onToggleDnf(x.target.checked)}
              />
            }
            label={<Typography variant="body2">DNF</Typography>}
          />
        </TableCell>
      )}
    </TableRow>
  );
}
