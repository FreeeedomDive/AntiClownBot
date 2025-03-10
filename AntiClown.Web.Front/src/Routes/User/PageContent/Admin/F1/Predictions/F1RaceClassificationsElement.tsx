import React, { useState } from "react";
import {
  Button,
  ButtonGroup,
  Checkbox,
  FormControlLabel,
  TableCell,
  TableRow,
  Typography,
} from "@mui/material";
import ArrowUpwardIcon from "@mui/icons-material/ArrowUpward";
import ArrowDownwardIcon from "@mui/icons-material/ArrowDownward";

interface Props {
  f1Driver: string;
  index: number;
  isDnf: boolean;
  onAddDnfDriver: () => void;
  onRemoveDnfDriver: () => void;
  moveUp: () => void;
  moveDown: () => void;
}

export default function F1RaceClassificationsElement({
  f1Driver,
  index,
  isDnf,
  onAddDnfDriver,
  onRemoveDnfDriver,
  moveUp,
  moveDown,
}: Props) {
  const [isChecked, setIsChecked] = useState(isDnf);
  const position = index + 1;

  return (
    <TableRow>
      <TableCell sx={{ padding: "1px" }}>
        <Typography>{position}</Typography>
      </TableCell>
      <TableCell sx={{ padding: "1px" }}>
        <ButtonGroup size="medium">
          <Button
            variant="contained"
            color="success"
            disabled={position === 1}
            onClick={() => moveUp()}
          >
            <ArrowUpwardIcon />
          </Button>
          <Button
            variant="contained"
            color="error"
            disabled={position === 20}
            sx={{ marginLeft: "4px" }}
            onClick={() => moveDown()}
          >
            <ArrowDownwardIcon />
          </Button>
        </ButtonGroup>
      </TableCell>
      <TableCell sx={{ padding: "1px" }}>
        <Typography>{f1Driver}</Typography>
      </TableCell>
      <TableCell sx={{ padding: "1px" }}>
        <FormControlLabel
          control={
            <Checkbox
              checked={isChecked}
              onChange={(x) => {
                const isDnf = x.target.checked;
                if (isDnf) {
                  onAddDnfDriver();
                } else {
                  onRemoveDnfDriver();
                }
                setIsChecked(x.target.checked);
              }}
            />
          }
          label={"DNF"}
        />
      </TableCell>
    </TableRow>
  );
}
