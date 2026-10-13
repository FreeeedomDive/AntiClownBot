import React from "react";
import {
  Box,
  Checkbox,
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
  disabled?: boolean;
  highlighted?: boolean;
}

export default function F1RaceClassificationsElement({
  f1Driver,
  index,
  isDnf,
  onToggleDnf,
  provided,
  teams,
  showDnf = true,
  disabled = false,
  highlighted = false,
}: Props) {
  const position = index + 1;

  return (
    <TableRow
      ref={provided.innerRef}
      {...provided.draggableProps}
      sx={{
        bgcolor: isDnf
          ? "rgba(255,167,38,0.07)"
          : position === 10 && showDnf
            ? "rgba(144,202,249,0.06)"
            : "transparent",
        "&:hover": { bgcolor: "rgba(255,255,255,0.05)" },
      }}
    >
      <TableCell sx={{ p: 0.5, width: 32 }} {...provided.dragHandleProps}>
        <IconButton
          size="small"
          disabled={disabled}
          aria-label={`Переместить ${f1Driver}`}
          sx={{ cursor: "grab", color: "text.disabled" }}
        >
          <DragIndicatorIcon fontSize="small" />
        </IconButton>
      </TableCell>

      <TableCell sx={{ p: 0.5, width: 40 }}>
        <Typography
          variant="body2"
          sx={{
            fontVariantNumeric: "tabular-nums",
            color:
              position === 10 && showDnf ? "primary.main" : "text.secondary",
            fontWeight: position === 10 && showDnf ? 700 : 400,
          }}
        >
          {position}
        </Typography>
      </TableCell>

      <TableCell sx={{ py: 0.5, px: 1 }}>
        <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
          <F1TeamBadge driver={f1Driver} teams={teams} />
          <Typography
            variant="body2"
            sx={{
              color: highlighted ? "primary.main" : "text.primary",
              fontWeight: highlighted ? 600 : 400,
            }}
            title={highlighted ? "Пилот для прогноза позиции" : undefined}
          >
            {f1Driver}
          </Typography>
        </Box>
      </TableCell>

      {showDnf && (
        <TableCell align="center" sx={{ py: 0, px: 1, width: 48 }}>
          <Checkbox
            size="small"
            sx={{ p: "4px" }}
            checked={isDnf}
            disabled={disabled}
            color="warning"
            inputProps={{ "aria-label": `DNF ${f1Driver}` }}
            onChange={(x) => onToggleDnf(x.target.checked)}
          />
        </TableCell>
      )}
    </TableRow>
  );
}
