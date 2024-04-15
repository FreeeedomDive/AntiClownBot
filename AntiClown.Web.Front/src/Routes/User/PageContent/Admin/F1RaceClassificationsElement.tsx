import {F1DriverDto} from "../../../../Dto/F1Predictions/F1DriverDto";
import React, {useState} from "react";
import {Checkbox, FormControlLabel, Stack, Typography} from "@mui/material";
import {useDrag, useDrop} from "react-dnd";

interface Props {
  f1Driver: F1DriverDto;
  position: number;
  onAddDnfDriver: () => void;
  onRemoveDnfDriver: () => void
  moveItem: (dragIndex: number, hoverIndex: number) => void;
}

interface DriverItem {
  f1Driver: F1DriverDto;
  position: number;
}

export default function F1RaceClassificationsElement({f1Driver, position, onAddDnfDriver, onRemoveDnfDriver, moveItem}: Props) {
  const [isChecked, setIsChecked] = useState(false);
  const [, ref] = useDrag({
    type: 'ITEM',
    item: { f1Driver, position },
  });

  const [, drop] = useDrop({
    accept: 'ITEM',
    hover(item: DriverItem, _) {
      if (!ref) {
        return;
      }

      const dragIndex = item.position;
      const hoverIndex = position;
      moveItem(dragIndex, hoverIndex);
      item.position = hoverIndex;
    },
  });

  return (
    <Stack
      ref={(node) => ref(drop(node))}
      direction="row"
      alignItems={"center"}
      justifyContent={"space-between"}
    >
      <Typography>{f1Driver}</Typography>
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
    </Stack>
  );
}