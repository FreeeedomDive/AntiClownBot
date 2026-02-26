import React from "react";
import {
  IconButton,
  Stack,
  Table,
  TableBody,
  TableContainer, Tooltip,
  Typography
} from "@mui/material";
import {
  DragDropContext,
  Draggable,
  Droppable,
  DropResult,
} from "@hello-pangea/dnd";
import F1ChampionshipDriverDnDRow from "./F1ChampionshipDriverDnDRow";
import { InfoOutlined } from "@mui/icons-material";

interface Props {
  title: string;
  description: string;
  droppableId: string;
  drivers: string[];
  setDrivers: (drivers: string[]) => void;
  editable: boolean;
  disabled: boolean;
}

export default function F1ChampionshipDriverDnDList({
  title,
  description,
  droppableId,
  drivers,
  setDrivers,
  editable,
  disabled,
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
      <Stack direction="row" spacing={1}>
        <Typography variant="h6">{title}</Typography>
        <Tooltip title={description} arrow>
          <IconButton
            sx={{
              padding: 1,
              minWidth: "auto",
              width: 30,
              height: 30,
              borderRadius: "50%",
              backgroundColor: "transparent",
            }}
          >
            <InfoOutlined sx={{ fontSize: 18 }} />
          </IconButton>
        </Tooltip>
      </Stack>
      {!editable ? (
        <TableContainer>
          <Table>
            <TableBody>
              {drivers.map((driver, index) => (
                <F1ChampionshipDriverDnDRow
                  key={`${droppableId}-${driver}`}
                  driver={driver}
                  index={index}
                  draggable={editable}
                />
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
                              key={`${droppableId}-${driver}`}
                              provided={provided}
                              driver={driver}
                              index={index}
                              draggable={editable}
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
