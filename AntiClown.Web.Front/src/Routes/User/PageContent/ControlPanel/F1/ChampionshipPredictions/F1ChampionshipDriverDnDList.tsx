import React from "react";
import {
  IconButton,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableRow,
  Typography,
} from "@mui/material";
import DragIndicatorIcon from "@mui/icons-material/DragIndicator";
import {
  DragDropContext,
  Draggable,
  Droppable,
  DropResult,
} from "@hello-pangea/dnd";
import F1ChampionshipDriverDnDRow from "./F1ChampionshipDriverDnDRow";

interface Props {
  title: string;
  droppableId: string;
  drivers: string[];
  setDrivers: (drivers: string[]) => void;
  disabled?: boolean;
}

export default function F1ChampionshipDriverDnDList({
  title,
  droppableId,
  drivers,
  setDrivers,
  disabled = false,
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

  return (
    <Stack direction="column" spacing={1} sx={disabled ? { opacity: 0.4 } : {}}>
      <Typography variant="h6">{title}</Typography>
      {disabled ? (
        <TableContainer>
          <Table>
            <TableBody>
              {drivers.map((driver, index) => (
                <TableRow key={driver}>
                  <TableCell sx={{ padding: "1px", width: "10%" }}>
                    <IconButton size="small" disabled>
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
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      ) : (
        <DragDropContext onDragEnd={onDragEnd}>
          <TableContainer>
            <Table>
              <Droppable droppableId={droppableId}>
                {(provided) => (
                  <>
                    <TableBody
                      ref={provided.innerRef}
                      {...provided.droppableProps}
                    >
                      {drivers.map((driver, index) => (
                        <Draggable
                          key={driver}
                          draggableId={`${droppableId}-${driver}`}
                          index={index}
                        >
                          {(provided) => (
                            <F1ChampionshipDriverDnDRow
                              provided={provided}
                              driver={driver}
                              index={index}
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
      )}
    </Stack>
  );
}
