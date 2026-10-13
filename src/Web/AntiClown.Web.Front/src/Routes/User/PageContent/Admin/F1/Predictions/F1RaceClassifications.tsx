import React from "react";
import F1RaceClassificationsElement from "./F1RaceClassificationsElement";
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
} from "@mui/material";
import {
  DragDropContext,
  Droppable,
  Draggable,
  DropResult,
} from "@hello-pangea/dnd";
import { F1TeamDto } from "../../../../../../Dto/F1Predictions/F1TeamDto";

interface Props {
  drivers: string[];
  setDrivers: (x: string[]) => void;
  dnfDrivers: Set<string>;
  setDnfDrivers: (x: Set<string>) => void;
  teams: F1TeamDto[];
  showDnf?: boolean;
  disabled?: boolean;
  highlightedDriver?: string;
}

export default function F1RaceClassifications({
  drivers,
  setDrivers,
  dnfDrivers,
  setDnfDrivers,
  teams,
  showDnf = true,
  disabled = false,
  highlightedDriver,
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
        <Table
          size="small"
          sx={{
            "& .MuiTableCell-root": { borderColor: "rgba(255,255,255,0.08)" },
          }}
        >
          <TableHead>
            <TableRow>
              <TableCell sx={{ width: 32, px: 0.5 }} />
              <TableCell
                sx={{
                  width: 40,
                  px: 0.5,
                  color: "text.secondary",
                  fontSize: "0.75rem",
                }}
              >
                Поз.
              </TableCell>
              <TableCell sx={{ color: "text.secondary", fontSize: "0.75rem" }}>
                Пилот
              </TableCell>
              {showDnf && (
                <TableCell
                  align="center"
                  sx={{ color: "text.secondary", fontSize: "0.75rem", px: 1 }}
                >
                  DNF
                </TableCell>
              )}
            </TableRow>
          </TableHead>
          <Droppable droppableId="admin-f1-drivers-dnd-list">
            {(provided) => (
              <TableBody ref={provided.innerRef} {...provided.droppableProps}>
                {drivers.map((driver, index) => (
                  <Draggable
                    key={driver}
                    draggableId={driver}
                    index={index}
                    isDragDisabled={disabled}
                  >
                    {(provided) => (
                      <F1RaceClassificationsElement
                        provided={provided}
                        f1Driver={driver}
                        index={index}
                        isDnf={dnfDrivers.has(driver)}
                        onToggleDnf={(checked) =>
                          handleToggleDnf(driver, checked)
                        }
                        teams={teams}
                        showDnf={showDnf}
                        disabled={disabled}
                        highlighted={driver === highlightedDriver}
                      />
                    )}
                  </Draggable>
                ))}
                {provided.placeholder}
              </TableBody>
            )}
          </Droppable>
        </Table>
      </TableContainer>
    </DragDropContext>
  );
}
