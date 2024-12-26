import {F1TeamDto} from "../../../../../Dto/F1Predictions/F1TeamDto";
import React, {useCallback, useState} from "react";
import {FormControl, OutlinedInput, TableCell, TableRow, Typography} from "@mui/material";
import {LoadingButton} from "@mui/lab";
import {Save} from "@mui/icons-material";
import F1PredictionsApi from "../../../../../Api/F1PredictionsApi";

interface IProps {
  team: F1TeamDto;
}

export default function F1PredictionsTeamsEditorRow({team}: IProps) {
  const [firstDriver, setFirstDriver] = useState<string>(team.firstDriver);
  const [secondDriver, setSecondDriver] = useState<string>(team.secondDriver);
  const [isSaving, setIsSaving] = useState(false);

  const saveTeam = useCallback(async () => {
    setIsSaving(true)
    await F1PredictionsApi.createOrUpdateTeam(team.name, firstDriver, secondDriver)
    setIsSaving(false)
  }, [firstDriver, secondDriver, team.name]);

  return (
    <TableRow>
      <TableCell sx={{padding: '1px'}}>
        <Typography>{team.name}</Typography>
      </TableCell>
      <TableCell sx={{padding: '1px'}}>
        <FormControl>
          <OutlinedInput
            id="outlined-adornment-weight"
            aria-describedby="outlined-weight-helper-text"
            value={firstDriver}
            onChange={(event) => {
              setFirstDriver(event.target.value)
            }}
          />
        </FormControl>
      </TableCell>
      <TableCell sx={{padding: '1px'}}>
        <FormControl>
          <OutlinedInput
            id="outlined-adornment-weight"
            aria-describedby="outlined-weight-helper-text"
            value={secondDriver}
            onChange={(event) => {
              setSecondDriver(event.target.value)
            }}
          />
        </FormControl>
      </TableCell>
      <TableCell sx={{padding: '1px'}}>
        <LoadingButton
          loading={isSaving}
          disabled={isSaving}
          color="primary"
          size="large"
          variant="text"
          startIcon={<Save/>}
          onClick={saveTeam}
        />
      </TableCell>
    </TableRow>
  )
}