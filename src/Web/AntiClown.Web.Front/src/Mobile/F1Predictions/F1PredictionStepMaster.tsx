import { F1RaceDto } from "../../Dto/F1Predictions/F1RaceDto";
import {
  Accordion,
  AccordionDetails,
  AccordionSummary,
  Alert,
  Button,
  ButtonGroup,
  Skeleton,
  Snackbar,
  Stack,
  Typography,
} from "@mui/material";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
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
import F1PredictionsDriverPositionSelect from "../../Routes/User/PageContent/ControlPanel/F1/Predictions/Selections/F1PredictionsDriverPositionSelect";
import F1QualifyingGridView from "../../Routes/User/PageContent/ControlPanel/F1/Predictions/F1QualifyingGridView";

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
      const [result, teams] = await Promise.all([
        F1PredictionsApi.read(f1Race.id),
        F1PredictionsApi.getActiveTeams(),
      ]);

      setCurrentF1Race(result);
      setTeams(teams);
    }

    load().catch(console.error);
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
  const [driverPosition, setDriverPosition] = useState<number | null>(
    userPrediction?.driverPositionPrediction ?? null
  );

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

    return driverPosition && 1 <= driverPosition && driverPosition <= 22;
  }, [
    dnfList,
    firstPlaceLead,
    isDNFNobody,
    selected10Position,
    step,
    driverPosition,
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
      tenthPlacePickedDriver: selected10Position,
      dnfPrediction: {
        noDnfPredicted: isDNFNobody,
        dnfPickedDrivers: isDNFNobody ? null : dnfList,
      },
      driverPositionPrediction: driverPosition!,
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
    selectedSafetyCarPrediction,
    step,
    userId,
    driverPosition,
  ]);

  return !mobileUserContextStore.isUnknown &&
    !!mobileUserContextStore.user?.id ? (
    <Stack
      spacing={1}
      direction="column"
      alignItems={"center"}
      justifyContent={"space-between"}
    >
      <Accordion defaultExpanded>
        <AccordionSummary expandIcon={<ExpandMoreIcon />}>
          <Typography variant="h6">Стартовая решётка</Typography>
        </AccordionSummary>
        <AccordionDetails sx={{ p: 0 }}>
          {currentF1Race.qualifyingGrid && currentF1Race.qualifyingGrid.length > 0 ? (
            <F1QualifyingGridView
              grid={currentF1Race.qualifyingGrid}
              teams={teams}
              showTitle={false}
            />
          ) : (
            <Stack spacing={0.5} sx={{ px: 2, pb: 1, opacity: 0.4 }}>
              <Typography variant="body2" color="text.secondary" align="center">
                Результаты квалификации появятся здесь
              </Typography>
              {Array.from({ length: 22 }).map((_, i) => (
                <Skeleton key={i} variant="rectangular" height={22} sx={{ borderRadius: 1 }} />
              ))}
            </Stack>
          )}
        </AccordionDetails>
      </Accordion>

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
        <F1PredictionsDriverPositionSelect
          driver={f1Race.conditions?.positionPredictionDriver ?? ""}
          teams={teams}
          selectedPosition={driverPosition}
          setSelectedPosition={setDriverPosition}
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
