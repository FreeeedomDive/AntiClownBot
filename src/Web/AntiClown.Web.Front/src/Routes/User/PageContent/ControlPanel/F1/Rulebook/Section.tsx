import React from "react";
import { Box, Stack, Typography } from "@mui/material";

interface Props {
  title: string;
  id?: string;
  children: React.ReactNode;
}

export function Section({ title, id, children }: Props) {
  return (
    <Box id={id}>
      <Typography variant="h5" sx={{ fontWeight: "bold", mb: 3 }}>
        {title}
      </Typography>
      <Stack spacing={2}>{children}</Stack>
    </Box>
  );
}
