import { F1RaceDto } from "../../Dto/F1Predictions/F1RaceDto";
import {
  Alert,
  Button,
  ButtonGroup,
  Divider,
  SelectChangeEvent,
  Snackbar,
  Stack,
  Typography,
} from "@mui/material";
import React, { useCallback, useEffect, useMemo, useState } from "react";
import {
  F1SafetyCarPredictionDto,
  F1SafetyCarsPredictionObject,
} from "../../Dto/F1Predictions/F1SafetyCarsPredictionDto";
import { useStore } from "../../Stores";
import { F1TeamDto } from "../../Dto/F1Predictions/F1TeamDto";
import F1PredictionsApi from "../../Api/F1PredictionsApi";
import { AddPredictionResultDto } from "../../Dto/F1Predictions/AddPredictionResultDto";
import F1PredictionsTenthPlaceSelect from "../../Routes/User/PageContent/ControlPanel/F1/Predictions/Selections/F1PredictionsTenthPlaceSelect";
import F1PredictionsDnfSelect, {
  DNFList,
} from "../../Routes/User/PageContent/ControlPanel/F1/Predictions/Selections/F1PredictionsDnfSelect";
import F1PredictionsIncidentsSelect from "../../Routes/User/PageContent/ControlPanel/F1/Predictions/Selections/F1PredictionsIncidentsSelect";
import F1PredictionsFirstPlaceLeadSelect from "../../Routes/User/PageContent/ControlPanel/F1/Predictions/Selections/F1PredictionsFirstPlaceLeadSelect";
import F1PredictionsTeamsSelect from "../../Routes/User/PageContent/ControlPanel/F1/Predictions/Selections/F1PredictionsTeamsSelect";

interface Props {
  f1Race: F1RaceDto;
}

export default function F1PredictionStepMaster({ f1Race }: Props) {
  const maxSteps = 5;
  const [step, setStep] = useState(1);
  const { mobileUserContextStore } = useStore();
  const userId = mobileUserContextStore.user?.id;
  const [currentF1Race, setCurrentF1Race] = useState<F1RaceDto>(f1Race);
  const [teams, setTeams] = useState<F1TeamDto[]>([]);
  const [isSaving, setIsSaving] = useState(false);
  const [savePredictionResult, setSavePredictionResult] =
    useState<AddPredictionResultDto | null>(null);

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

  const [isDNFNobody, setIsDNFNobody] = useState(() => {
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
    if (step === 1) {
      return !!selected10Position;
    }

    if (step === 2) {
      return isDNFNobody || new Set(dnfList).size === 5;
    }

    if (step === 3) {
      return true;
    }

    if (step === 4) {
      return firstPlaceLead && Number(firstPlaceLead) > 0;
    }

    return selectedDriversFromTeams.size === 10;
  }, [
    dnfList,
    firstPlaceLead,
    isDNFNobody,
    selected10Position,
    selectedDriversFromTeams.size,
    step,
  ]);

  const saveF1Prediction = useCallback(async () => {
    if (!userId || !isValid) {
      return;
    }

    if (step < maxSteps) {
      setStep(step + 1);
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
    step,
    userId,
  ]);

  return !mobileUserContextStore.isUnknown &&
    !!mobileUserContextStore.user?.id ? (
    <Stack
      spacing={1}
      direction="column"
      alignItems={"center"}
      justifyContent={"space-between"}
    >
      {!currentF1Race.isOpened && (
        <Alert severity="warning">Предсказания закрыты</Alert>
      )}
      {step === 1 && (
        <F1PredictionsTenthPlaceSelect
          selected10Position={selected10Position}
          setSelected10Position={setSelected10Position}
          teams={teams}
        />
      )}
      {step === 2 && (
        <F1PredictionsDnfSelect
          isDNFNobody={isDNFNobody}
          setIsDNFNobody={setIsDNFNobody}
          dnfList={dnfList}
          setDnfList={setDnfList}
          teams={teams}
        />
      )}
      {step === 3 && (
        <F1PredictionsIncidentsSelect
          selectedSafetyCarPrediction={selectedSafetyCarPrediction}
          setSelectedSafetyCarPrediction={setSafetyCarPrediction}
        />
      )}
      {step === 4 && (
        <F1PredictionsFirstPlaceLeadSelect
          firstPlaceLead={firstPlaceLead}
          setFirstPlaceLead={setFirstPlaceLead}
        />
      )}
      {step === 5 && (
        <F1PredictionsTeamsSelect
          selectedDriversFromTeams={selectedDriversFromTeams}
          setSelectedDriversFromTeams={setSelectedDriversFromTeams}
          teams={teams}
        />
      )}
      <ButtonGroup size="large" fullWidth>
        <Button
          variant={"outlined"}
          color={"error"}
          onClick={() => {
            setStep(step - 1);
          }}
          disabled={step === 1}
        >
          Назад
        </Button>
        <Button
          variant={step === maxSteps ? "contained" : "outlined"}
          color={"info"}
          onClick={saveF1Prediction}
          disabled={
            !isValid ||
            isSaving ||
            (step === maxSteps && !currentF1Race.isOpened)
          }
        >
          {isSaving
            ? "Сохранение..."
            : step === maxSteps
              ? "Сохранить"
              : "Далее"}
        </Button>
      </ButtonGroup>

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
    </Stack>
  ) : (
    <></>
  );
}
