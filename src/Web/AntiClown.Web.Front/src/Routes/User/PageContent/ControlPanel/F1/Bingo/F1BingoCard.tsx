import { F1BingoCardDto } from "../../../../../../Dto/F1Bingo/F1BingoCardDto";
import React from "react";
import {
  Tooltip,
  Card,
  CardContent,
  Typography,
  Box,
  IconButton,
} from "@mui/material";
import { InfoOutlined } from "@mui/icons-material";

interface Props {
  card: F1BingoCardDto;
}

const PROBABILITY_COLORS = {
  Low: "darkred",
  Medium: "#F4D06F",
  High: "darkgreen",
};

export default function F1BingoCard({ card }: Props) {
  const {
    description,
    explanation,
    probability,
    totalRepeats,
    completedRepeats,
    isCompleted,
  } = card;

  return (
    <Card
      sx={{
        width: "100%",
        height: "100%",
        border: `4px solid ${PROBABILITY_COLORS[probability]}`,
        backgroundColor: `transparent`,
        position: "relative",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        textAlign: "center",
        borderRadius: 2,
      }}
    >
      <CardContent>
        <Typography variant="h6" sx={{ fontSize: 14, fontWeight: "bold" }}>
          {description}
        </Typography>
        {totalRepeats > 1 && (
          <Box display="flex" justifyContent="center" mt={1}>
            {[...Array(totalRepeats)].map((_, index) => (
              <Box
                key={index}
                sx={{
                  width: 32,
                  height: 32,
                  borderRadius: "50%",
                  backgroundColor:
                    index < completedRepeats ? "gray" : "transparent",
                  border: `1px solid gray`,
                  margin: "2px",
                }}
              />
            ))}
          </Box>
        )}
      </CardContent>
      {isCompleted && (
        <Box
          sx={{
            position: "absolute",
            width: "100%",
            height: "100%",
            top: 0,
            left: 0,
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            "&::before": {
              content: "''",
              position: "absolute",
              width: "150%",
              height: "4px",
              backgroundColor: PROBABILITY_COLORS[probability],
              transform: "rotate(45deg)",
            },
            "&::after": {
              content: "''",
              position: "absolute",
              width: "150%",
              height: "4px",
              backgroundColor: PROBABILITY_COLORS[probability],
              transform: "rotate(-45deg)",
            },
          }}
        />
      )}
      {explanation && (
        <Tooltip title={explanation} arrow>
          <IconButton
            sx={{
              position: "absolute",
              top: 8,
              right: 8,
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
      )}
    </Card>
  );
}
