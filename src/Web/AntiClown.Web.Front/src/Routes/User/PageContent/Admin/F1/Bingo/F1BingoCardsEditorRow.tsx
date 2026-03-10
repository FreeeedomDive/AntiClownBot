import { F1BingoCardDto } from "../../../../../../Dto/F1Bingo/F1BingoCardDto";
import { Box, IconButton, Stack, TableCell, TableRow, Typography } from "@mui/material";
import { LoadingButton } from "@mui/lab";
import { Add, Remove, Save } from "@mui/icons-material";
import React, { useCallback } from "react";
import F1BingoApi from "../../../../../../Api/F1BingoApi";

interface Props {
  card: F1BingoCardDto;
}

export default function F1BingoCardsEditorRow({ card }: Props) {
  const [isSaving, setIsSaving] = React.useState(false);
  const [repeatsCount, setRepeatsCount] = React.useState(card.completedRepeats);

  const saveCard = useCallback(async () => {
    setIsSaving(true);
    await F1BingoApi.updateCard(card.id, {
      newRepeatsCount: repeatsCount,
    });
    setIsSaving(false);
  }, [card.id, repeatsCount]);

  return (
    <TableRow sx={{ "&:hover": { bgcolor: "action.hover" } }}>
      <TableCell sx={{ py: 1.5, px: 2 }}>
        <Typography variant="body2">{card.description}</Typography>
      </TableCell>
      <TableCell sx={{ py: 1.5, px: 2 }}>
        <Stack direction="row" spacing={1} alignItems="center">
          <IconButton
            size="small"
            color="error"
            disabled={repeatsCount <= 0}
            onClick={() => setRepeatsCount(repeatsCount - 1)}
          >
            <Remove fontSize="small" />
          </IconButton>
          <Box
            sx={{
              px: 2,
              py: 0.5,
              bgcolor: "action.selected",
              borderRadius: 1,
              minWidth: 64,
              textAlign: "center",
            }}
          >
            <Typography variant="body2" fontWeight={600}>
              {repeatsCount} / {card.totalRepeats}
            </Typography>
          </Box>
          <IconButton
            size="small"
            color="success"
            disabled={repeatsCount >= card.totalRepeats}
            onClick={() => setRepeatsCount(repeatsCount + 1)}
          >
            <Add fontSize="small" />
          </IconButton>
        </Stack>
      </TableCell>
      <TableCell sx={{ py: 1.5, px: 2, width: 56 }}>
        <LoadingButton
          loading={isSaving}
          disabled={isSaving}
          color="primary"
          size="small"
          variant="text"
          startIcon={<Save />}
          onClick={saveCard}
        />
      </TableCell>
    </TableRow>
  );
}
