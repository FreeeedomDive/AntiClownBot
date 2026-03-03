import React from "react";
import F1RaceClassificationsElement from "./F1RaceClassificationsElement";
import { Table, TableBody, TableContainer } from "@mui/material";
import {
  DragDropContext,
  Droppable,
  Draggable,
  DropResult,
} from "@hello-pangea/dnd";

interface Props {
  drivers: string[];
  setDrivers: (x: string[]) => void;
  dnfDrivers: Set<string>;
  setDnfDrivers: (x: Set<string>) => void;
}

export default function F1RaceClassifications({
  drivers,
  setDrivers,
  dnfDrivers,
  setDnfDrivers,
}: Props) {
  const onDragEnd = (result: DropResult) => {
    if (!result.destination) {
      return;
    }

    const newDrivers = Array.from(drivers);
    const [reorderedItem] = newDrivers.splice(result.source.index, 1);
    newDrivers.splice(result.destination.index, 0, reorderedItem);

    setDrivers(newDrivers);
  };

  const handleToggleDnf = (driver: string, isChecked: boolean) => {
    const newDnfs = new Set(dnfDrivers);
    if (isChecked) {
      newDnfs.add(driver);
    } else {
      newDnfs.delete(driver);
    }
    setDnfDrivers(newDnfs);
  };

  return (
    <DragDropContext onDragEnd={onDragEnd}>
      <TableContainer>
        <Table>
          <Droppable droppableId="admin-f1-drivers-dnd-list">
            {(provided) => (
              <>
                <TableBody ref={provided.innerRef} {...provided.droppableProps}>
                  {drivers.map((driver, index) => (
                    <Draggable key={driver} draggableId={driver} index={index}>
                      {(provided) => (
                        <F1RaceClassificationsElement
                          provided={provided}
                          f1Driver={driver}
                          index={index}
                          isDnf={dnfDrivers.has(driver)}
                          onToggleDnf={(checked) =>
                            handleToggleDnf(driver, checked)
                          }
                        />
                      )}
                    </Draggable>
                  ))}
                </TableBody>
                {provided.placeholder}
              </>
            )}
          </Droppable>
        </Table>
      </TableContainer>
    </DragDropContext>
  );
}
