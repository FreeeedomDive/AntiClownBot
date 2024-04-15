import {DRIVERS} from "../../../../Dto/F1Predictions/F1DriversHelpers";
import F1RaceClassificationsElement from "./F1RaceClassificationsElement";
import {Stack} from "@mui/material";
import React, {useState} from "react";
import {HTML5Backend} from "react-dnd-html5-backend";
import {DndProvider} from "react-dnd";

export default function F1RaceClassifications() {
  const [drivers, setDrivers] = useState(DRIVERS);
  const moveItem = (dragIndex: number, hoverIndex: number) => {
    const draggedItem = drivers[dragIndex];
    const updatedItems = [...drivers];

    updatedItems.splice(dragIndex, 1);
    updatedItems.splice(hoverIndex, 0, draggedItem);

    console.log(updatedItems);
    setDrivers(updatedItems);
  };

  return (
    <Stack width={"20%"}>
      <DndProvider backend={HTML5Backend}>
        <div>
          {drivers.map((driver, position) => (
            <F1RaceClassificationsElement
              f1Driver={driver}
              position={position}
              onAddDnfDriver={() => {}}
              onRemoveDnfDriver={() => {}}
              moveItem={moveItem}
            />
          ))}
        </div>
      </DndProvider>
    </Stack>
  )
}