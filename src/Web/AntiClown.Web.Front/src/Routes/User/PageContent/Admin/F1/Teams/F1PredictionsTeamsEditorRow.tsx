import { F1TeamDto } from "../../../../../../Dto/F1Predictions/F1TeamDto";
import React, { useCallback, useState } from "react";
import {
  Avatar,
  Stack,
  TableCell,
  TableRow,
  TextField,
  Typography,
} from "@mui/material";
import { LoadingButton } from "@mui/lab";
import { Save } from "@mui/icons-material";
import F1PredictionsApi from "../../../../../../Api/F1PredictionsApi";
import { teamNameToLogo } from "../../../../../../Helpers/TeamNameToLogoHelper";

interface IProps {
  team: F1TeamDto;
}

export default function F1PredictionsTeamsEditorRow({ team }: IProps) {
  const [firstDriver, setFirstDriver] = useState<string>(team.firstDriver);
  const [secondDriver, setSecondDriver] = useState<string>(team.secondDriver);
  const [isSaving, setIsSaving] = useState(false);

  const saveTeam = useCallback(async () => {
    setIsSaving(true);
    await F1PredictionsApi.createOrUpdateTeam(
      team.name,
      firstDriver,
      secondDriver,
    );
    setIsSaving(false);
  }, [firstDriver, secondDriver, team.name]);

  return (
    <TableRow sx={{ "&:hover": { bgcolor: "action.hover" } }}>
      <TableCell sx={{ py: 1.5, px: 2 }}>
        <Stack direction="row" spacing={1.5} alignItems="center">
          <Avatar
            variant="rounded"
            alt={team.name}
            src={teamNameToLogo(team.name)}
            sx={{ width: 100, height: 34 }}
          />
          <Typography variant="body2" fontWeight={500}>
            {team.name}
          </Typography>
        </Stack>
      </TableCell>
      <TableCell sx={{ py: 1.5, px: 2 }}>
        <TextField
          size="small"
          value={firstDriver}
          onChange={(e) => setFirstDriver(e.target.value)}
        />
      </TableCell>
      <TableCell sx={{ py: 1.5, px: 2 }}>
        <TextField
          size="small"
          value={secondDriver}
          onChange={(e) => setSecondDriver(e.target.value)}
        />
      </TableCell>
      <TableCell sx={{ py: 1.5, px: 2, width: 56 }}>
        <LoadingButton
          loading={isSaving}
          disabled={isSaving}
          color="primary"
          size="small"
          variant="text"
          startIcon={<Save />}
          onClick={saveTeam}
        />
      </TableCell>
    </TableRow>
  );
}
