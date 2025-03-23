import {Card, CardContent, Stack, Typography} from "@mui/material";
import React from "react";

interface Props {
  title: string;
  children: React.ReactNode | React.ReactNode[];
}

export default function F1PredictionsSelectCard({ children, title }: Props) {
  return (
    <Card
      sx={{
        width: "100%",
        //height: "100%",
        backgroundColor: `transparent`,
        alignItems: "center",
        justifyContent: "center",
        textAlign: "center",
        borderRadius: 4,
      }}
    >
      <CardContent sx={{ width: "100%" }}>
        <Stack
          spacing={1}
          direction="column"
          alignItems={"center"}
          textAlign={"start"}
        >
          <Typography variant="body1">{title}</Typography>
          {children}
        </Stack>
      </CardContent>
    </Card>
  );
}
