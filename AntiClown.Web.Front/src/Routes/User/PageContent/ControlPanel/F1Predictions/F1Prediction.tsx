import { useParams } from "react-router-dom";
import React, { useCallback, useEffect, useMemo, useState } from "react";
import {
  Alert,
  Button,
  ButtonGroup,
  Checkbox,
  Fab,
  FormControl,
  FormControlLabel,
  InputAdornment,
  MenuItem,
  OutlinedInput,
  Radio,
  RadioGroup,
  Select,
  SelectChangeEvent,
  Snackbar,
  Stack,
  Typography,
} from "@mui/material";
import {
  F1SafetyCarPredictionDto,
  F1SafetyCarsPredictionObject,
} from "../../../../../Dto/F1Predictions/F1SafetyCarsPredictionDto";
import { F1RaceDto } from "../../../../../Dto/F1Predictions/F1RaceDto";
import F1PredictionsApi from "../../../../../Api/F1PredictionsApi";
import { AddPredictionResultDto } from "../../../../../Dto/F1Predictions/AddPredictionResultDto";
import { Save } from "@mui/icons-material";
import { F1TeamDto } from "../../../../../Dto/F1Predictions/F1TeamDto";
import F1PredictionsTenthPlaceSelect from "./F1PredictionsTenthPlaceSelect";
import F1PredictionsDnfSelect, { DNFList } from "./F1PredictionsDnfSelect";
import F1PredictionsIncidentsSelect from "./F1PredictionsIncidentsSelect";
import F1PredictionsFirstPlaceLeadSelect from "./F1PredictionsFirstPlaceLeadSelect";
import F1PredictionsTeamsSelect from "./F1PredictionsTeamsSelect";

const firstColumnWidth = 150;
const teamButtonWidth = 150;

const fabStyle = {
  position: "absolute",
  bottom: 16,
  right: 16,
};

interface Props {
  f1Race: F1RaceDto;
}

export default function F1Prediction({ f1Race }: Props) {
  const { userId } = useParams<"userId">();
  const [currentF1Race, setCurrentF1Race] = useState<F1RaceDto>(f1Race);
  const [teams, setTeams] = useState<F1TeamDto[]>([]);

  useEffect(() => {
    async function load() {
      const result = await F1PredictionsApi.read(f1Race.id);
      setCurrentF1Race(result);

      const teams = await F1PredictionsApi.getActiveTeams();
      setTeams(teams);
    }

    load();
  }, [f1Race.id]);

  const userPrediction = useMemo(() => {
    return currentF1Race.predictions.find(
      (prediction) => prediction.userId === userId,
    );
  }, [currentF1Race, userId]);

  const [selected10Position, setSelected10Position] = useState<string>(
    userPrediction?.tenthPlacePickedDriver ?? teams[0]?.firstDriver,
  );

  const [isDNFNobody, setIsDNFNobody] = useState<boolean>(() => {
    return userPrediction?.dnfPrediction?.noDnfPredicted ?? false;
  });

  const [dnfList, setDnfList] = useState<DNFList>(() => {
    if (userPrediction?.dnfPrediction.dnfPickedDrivers?.length === 5) {
      return userPrediction.dnfPrediction.dnfPickedDrivers as DNFList;
    }

    return [
      teams[0]?.firstDriver,
      teams[0]?.firstDriver,
      teams[0]?.firstDriver,
      teams[0]?.firstDriver,
      teams[0]?.firstDriver,
    ];
  });
  const [selectedSafetyCarPrediction, setSafetyCarPrediction] =
    useState<F1SafetyCarPredictionDto>(
      userPrediction?.safetyCarsPrediction ?? F1SafetyCarsPredictionObject.Zero,
    );
  const [firstPlaceLead, setFirstPlaceLead] = useState<string>(
    String(userPrediction?.firstPlaceLeadPrediction ?? ""),
  );
  const [selectedDriversFromTeams, setSelectedDriversFromTeams] = useState<
    Set<string>
  >(() => {
    const initialArray = (() => userPrediction?.teamsPickedDrivers)();

    return new Set(initialArray ?? []);
  });

  const isValid = useMemo(() => {
    if (!isDNFNobody && new Set(dnfList).size !== 5) {
      return false;
    }

    if (!firstPlaceLead || Number(firstPlaceLead) < 0) {
      return false;
    }

    return selectedDriversFromTeams.size === 10;
  }, [dnfList, firstPlaceLead, isDNFNobody, selectedDriversFromTeams.size]);

  const [isSaving, setIsSaving] = useState(false);
  const [savePredictionResult, setSavePredictionResult] =
    useState<AddPredictionResultDto | null>(null);
  const saveF1Prediction = useCallback(async () => {
    if (!userId || !isValid) {
      return;
    }

    setIsSaving(true);

    const result = await F1PredictionsApi.addPrediction(f1Race.id, {
      userId,
      raceId: f1Race.id,
      firstPlaceLeadPrediction: Number(firstPlaceLead),
      safetyCarsPrediction: selectedSafetyCarPrediction,
      teamsPickedDrivers: Array.from(selectedDriversFromTeams.values()),
      tenthPlacePickedDriver: selected10Position,
      dnfPrediction: {
        noDnfPredicted: isDNFNobody,
        dnfPickedDrivers: isDNFNobody ? null : dnfList,
      },
    });

    setSavePredictionResult(result);
    setIsSaving(false);
  }, [
    dnfList,
    f1Race.id,
    firstPlaceLead,
    isDNFNobody,
    isValid,
    selected10Position,
    selectedDriversFromTeams,
    selectedSafetyCarPrediction,
    userId,
  ]);

  return (
    <Stack direction={"row"} width={"100%"}>
      <Stack padding={1} spacing={2}>
        <F1PredictionsTenthPlaceSelect
          selected10Position={selected10Position}
          setSelected10Position={setSelected10Position}
          teams={teams}
        />

        <F1PredictionsDnfSelect
          isDNFNobody={isDNFNobody}
          setIsDNFNobody={setIsDNFNobody}
          dnfList={dnfList}
          setDnfList={setDnfList}
          teams={teams}
        />
      </Stack>
      <Stack flexGrow={1} padding={1} spacing={2}>
        <F1PredictionsIncidentsSelect
          selectedSafetyCarPrediction={selectedSafetyCarPrediction}
          setSelectedSafetyCarPrediction={setSafetyCarPrediction}
        />

        <F1PredictionsFirstPlaceLeadSelect
          firstPlaceLead={firstPlaceLead}
          setFirstPlaceLead={setFirstPlaceLead}
        />
      </Stack>

      <Stack flexGrow={2} padding={1} spacing={2}>
        <F1PredictionsTeamsSelect
          selectedDriversFromTeams={selectedDriversFromTeams}
          setSelectedDriversFromTeams={setSelectedDriversFromTeams}
          teams={teams}
        />
      </Stack>
      <Snackbar
        open={savePredictionResult !== null}
        autoHideDuration={3000}
        onClose={() => setSavePredictionResult(null)}
      >
        <Alert
          severity={
            savePredictionResult === AddPredictionResultDto.Success
              ? "success"
              : "warning"
          }
        >
          {savePredictionResult === AddPredictionResultDto.Success
            ? "Предсказания успешно сохранены"
            : "Предсказания на эту гонку уже закрыты"}
        </Alert>
      </Snackbar>
      <Fab
        disabled={!isValid || isSaving}
        variant="extended"
        sx={fabStyle}
        onClick={saveF1Prediction}
        color={"success"}
      >
        <Save />
        {isSaving ? <>Сохранение...</> : <>Сохранить</>}
      </Fab>
    </Stack>
  );
}
