import {F1DriverDto} from "../../../../Dto/F1Predictions/F1DriverDto";
import {Box, Typography} from "@mui/material";
import React from "react";
import {useDrag} from "react-dnd";

interface Props {
  f1Driver: F1DriverDto
}

export default function F1RaceClassificationsElement({f1Driver}: Props) {
  const [{ opacity }, dragRef] = useDrag(
    () => ({
      type: "card",
      item: { f1Driver },
      collect: (monitor) => ({
        opacity: monitor.isDragging() ? 0.5 : 1
      })
    }),
    []
  )
  return (
    <Box
      ref={dragRef}
      style={{ opacity }}
      component="nav"
      sx={{
        border: 1,
        borderColor: "primary.main",
      }}
    >
      <Typography>{f1Driver}</Typography>
    </Box>
  )
}