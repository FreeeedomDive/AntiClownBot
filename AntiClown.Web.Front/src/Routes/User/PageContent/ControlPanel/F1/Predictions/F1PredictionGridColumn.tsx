import React from "react";
import {Grid, Stack} from "@mui/material";

interface Props {
  index: number;
  children: React.ReactElement | React.ReactElement[];
}

export default function F1PredictionGridColumn({ index, children }: Props) {
  return (
    <Grid
      item
      key={`F1PredictionsColumn${index}`}
      xs={12}
      sm={12}
      md={12}
      lg={4}
      sx={{ display: "flex", justifyContent: "top", alignItems: "top" }}
    >
      <Stack direction={"column"} spacing={1} width={"100%"}>
        {children}
      </Stack>
    </Grid>
  );
}
