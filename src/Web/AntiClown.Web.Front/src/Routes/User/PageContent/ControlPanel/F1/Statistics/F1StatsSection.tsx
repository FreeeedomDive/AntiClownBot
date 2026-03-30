import { Grid, Typography } from "@mui/material";
import React from "react";

interface Props {
  title: string;
  columns: 1 | 2 | 3;
  children: React.ReactNode[];
}

export default function F1StatsSection({ title, columns, children }: Props) {
  const xs = (12 / columns) as 4 | 6 | 12;

  return (
    <Grid container spacing={2} sx={{ width: "100%" }}>
      <Grid item xs={12}>
        <Typography variant="subtitle1" fontWeight={600} color="text.secondary">
          {title}
        </Typography>
      </Grid>
      {children.map((child, i) => (
        <Grid key={i} item xs={xs}>
          {child}
        </Grid>
      ))}
    </Grid>
  );
}
