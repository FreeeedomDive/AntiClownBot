import { F1BingoCardDto } from "../../../../../../Dto/F1Bingo/F1BingoCardDto";
import {
  Button,
  Stack,
  TableCell,
  TableRow,
  Typography,
} from "@mui/material";
import { LoadingButton } from "@mui/lab";
import { Save } from "@mui/icons-material";
import React, { useCallback } from "react";
import F1BingoApi from "../../../../../../Api/F1BingoApi";

interface Props {
  card: F1BingoCardDto;
}

export default function F1BingoCardsEditorRow({ card }: Props) {
  const [isSaving, setIsSaving] = React.useState(false);
  const [repeatsCount, setRepeatsCount] = React.useState(card.completedRepeats);

  const saveTeam = useCallback(async () => {
    setIsSaving(true);
    await F1BingoApi.updateCard(card.id, {
      newRepeatsCount: repeatsCount,
    });
    setIsSaving(false);
  }, [card.id, repeatsCount]);

  return (
    <TableRow>
      <TableCell sx={{ padding: "1px" }}>
        <Typography>{card.description}</Typography>
      </TableCell>
      <TableCell sx={{ padding: "1px" }}>
        <Stack direction={"row"}>
          <Button
            variant="contained"
            color="error"
            disabled={repeatsCount <= 0}
            onClick={() => setRepeatsCount(repeatsCount - 1)}
          >
            <Typography variant="h4">-</Typography>
          </Button>
          <Typography margin={"auto"}>
            {repeatsCount}/{card.totalRepeats}
          </Typography>
          <Button
            variant="contained"
            color="success"
            disabled={repeatsCount >= card.totalRepeats}
            onClick={() => setRepeatsCount(repeatsCount + 1)}
          >
            <Typography variant="h4">+</Typography>
          </Button>
        </Stack>
      </TableCell>
      <TableCell sx={{ padding: "1px" }}>
        <LoadingButton
          loading={isSaving}
          disabled={isSaving}
          color="primary"
          size="large"
          variant="text"
          startIcon={<Save />}
          onClick={saveTeam}
        />
      </TableCell>
    </TableRow>
  );
}
