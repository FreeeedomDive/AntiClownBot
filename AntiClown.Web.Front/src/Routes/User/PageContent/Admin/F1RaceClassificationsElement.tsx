import React, {useState} from "react";
import {
  Button,
  Checkbox,
  FormControlLabel, Stack,
  TableCell, TableRow,
  Typography
} from "@mui/material";
import ArrowUpwardIcon from '@mui/icons-material/ArrowUpward';
import ArrowDownwardIcon from '@mui/icons-material/ArrowDownward';
import {F1DriverDto} from "../../../../Dto/F1Predictions/F1DriverDto";

interface Props {
  f1Driver: F1DriverDto;
  index: number;
  onAddDnfDriver: () => void;
  onRemoveDnfDriver: () => void;
  moveUp: () => void;
  moveDown: () => void;
}

export default function F1RaceClassificationsElement(
  {
    f1Driver,
    index,
    onAddDnfDriver,
    onRemoveDnfDriver,
    moveUp,
    moveDown
  }: Props
) {
  const [isChecked, setIsChecked] = useState(false);
  const position = index + 1;

  return (
    <TableRow>
      <TableCell sx={{ padding: '1px'}}>
        <Typography>{position}</Typography>
      </TableCell>
      <TableCell sx={{ padding: '1px'}}>
        <Button
          variant="contained"
          color="success"
          disabled={position === 1}
          onClick={() => moveUp()}
        >
          <ArrowUpwardIcon/>
        </Button>
      </TableCell>
      <TableCell sx={{ padding: '1px'}}>
        <Button
          variant="contained"
          color="error"
          disabled={position === 20}
          sx={{marginLeft: '4px'}}
          onClick={() => moveDown()}
        >
          <ArrowDownwardIcon/>
        </Button>
      </TableCell>
      <TableCell sx={{ padding: '1px'}}>
        <Typography>{f1Driver}</Typography>
      </TableCell>
      <TableCell sx={{ padding: '1px'}}>
        <FormControlLabel
          control={<Checkbox checked={isChecked} onChange={x => {
            const isDnf = x.target.checked;
            if (isDnf) {
              onAddDnfDriver();
            } else {
              onRemoveDnfDriver();
            }
            setIsChecked(x.target.checked)
          }}/>}
          label={"DNF"}
        />
      </TableCell>
    </TableRow>
  )
}