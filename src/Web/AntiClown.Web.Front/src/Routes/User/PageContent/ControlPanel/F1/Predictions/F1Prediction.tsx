import { useParams } from "react-router-dom";
import React, { useCallback, useEffect, useMemo, useState } from "react";
import {
  Alert,
  Fab,
  Grid,
  Skeleton,
  Snackbar,
  Stack,
  Typography,
} from "@mui/material";
import {
  F1SafetyCarPredictionDto,
  F1SafetyCarsPredictionObject,
} from "../../../../../../Dto/F1Predictions/F1SafetyCarsPredictionDto";
import { F1RaceDto } from "../../../../../../Dto/F1Predictions/F1RaceDto";
import F1PredictionsApi from "../../../../../../Api/F1PredictionsApi";
import { AddPredictionResultDto } from "../../../../../../Dto/F1Predictions/AddPredictionResultDto";
import { Save } from "@mui/icons-material";
import { F1TeamDto } from "../../../../../../Dto/F1Predictions/F1TeamDto";
import F1PredictionsTenthPlaceSelect from "./Selections/F1PredictionsTenthPlaceSelect";
import F1PredictionsDnfSelect, {
  DNFList,
} from "./Selections/F1PredictionsDnfSelect";
import F1PredictionsIncidentsSelect from "./Selections/F1PredictionsIncidentsSelect";
import F1PredictionsFirstPlaceLeadSelect from "./Selections/F1PredictionsFirstPlaceLeadSelect";
import F1PredictionGridColumn from "./F1PredictionGridColumn";
import F1PredictionsDriverPositionSelect from "./Selections/F1PredictionsDriverPositionSelect";
import F1QualifyingGridView from "./F1QualifyingGridView";

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
  const [predictionData, setPredictionData] = useState<{
    f1Race: F1RaceDto;
    teams: F1TeamDto[];
  }>();

  useEffect(() => {
    Promise.all([
      F1PredictionsApi.read(f1Race.id, userId),
      F1PredictionsApi.getActiveTeams(),
    ])
      .then(([loadedRace, teams]) =>
        setPredictionData({ f1Race: loadedRace, teams }),
      )
      .catch(console.error);
  }, [f1Race.id, userId]);

  if (!predictionData) {
    return null;
  }

  return (
    <F1PredictionForm
      key={`${predictionData.f1Race.id}-${userId ?? ""}`}
      f1Race={predictionData.f1Race}
      teams={predictionData.teams}
      userId={userId}
    />
  );
}

interface F1PredictionFormProps extends Props {
  teams: F1TeamDto[];
  userId: string | undefined;
}

function F1PredictionForm({ f1Race, teams, userId }: F1PredictionFormProps) {
  const userPrediction = useMemo(() => {
    return f1Race.predictions.find(
      (prediction) => prediction.userId === userId,
    );
  }, [f1Race, userId]);

  const firstDriver = teams[0]?.firstDriver ?? "";
  const [selected10Position, setSelected10Position] = useState<string>(
    () => userPrediction?.tenthPlacePickedDriver ?? firstDriver,
  );
  const [isDNFNobody, setIsDNFNobody] = useState<boolean>(
    () => userPrediction?.dnfPrediction?.noDnfPredicted ?? false,
  );
  const [dnfList, setDnfList] = useState<DNFList>(() =>
    userPrediction?.dnfPrediction.dnfPickedDrivers?.length === 5
      ? (userPrediction.dnfPrediction.dnfPickedDrivers as DNFList)
      : ([
          firstDriver,
          firstDriver,
          firstDriver,
          firstDriver,
          firstDriver,
        ] as DNFList),
  );

  const [selectedSafetyCarPrediction, setSafetyCarPrediction] =
    useState<F1SafetyCarPredictionDto>(
      userPrediction?.safetyCarsPrediction ?? F1SafetyCarsPredictionObject.Zero,
    );
  const [firstPlaceLead, setFirstPlaceLead] = useState<string>(
    String(userPrediction?.firstPlaceLeadPrediction ?? ""),
  );
  const [driverPosition, setDriverPosition] = useState<number | null>(
    userPrediction?.driverPositionPrediction ?? null,
  );

  const isValid = useMemo(() => {
    if (!isDNFNobody && new Set(dnfList).size !== 5) {
      return false;
    }

    if (!firstPlaceLead || Number(firstPlaceLead) < 0) {
      return false;
    }

    return driverPosition && 1 <= driverPosition && driverPosition <= 22;
  }, [dnfList, firstPlaceLead, isDNFNobody, driverPosition]);

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
    userId,
    driverPosition,
  ]);

  return (
    <Stack direction={"row"} width={"100%"}>
      <Grid
        container
        spacing={1}
        sx={{ width: "100%", height: "100%", margin: "auto" }}
      >
        <F1PredictionGridColumn index={0}>
          {f1Race.qualifyingGrid && f1Race.qualifyingGrid.length > 0 ? (
            <F1QualifyingGridView grid={f1Race.qualifyingGrid} teams={teams} />
          ) : (
            <Stack direction="column" spacing={1} sx={{ opacity: 0.4, mt: 1 }}>
              <Typography variant="h6" align="center">
                Стартовая решётка
              </Typography>
              <Typography variant="body2" color="text.secondary" align="center">
                Результаты квалификации появятся здесь
              </Typography>
              {Array.from({ length: 22 }).map((_, i) => (
                <Skeleton
                  key={i}
                  variant="rectangular"
                  height={24}
                  sx={{ borderRadius: 1 }}
                />
              ))}
            </Stack>
          )}
        </F1PredictionGridColumn>

        <F1PredictionGridColumn index={1}>
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
        </F1PredictionGridColumn>

        <F1PredictionGridColumn index={2}>
          <F1PredictionsIncidentsSelect
            selectedSafetyCarPrediction={selectedSafetyCarPrediction}
            setSelectedSafetyCarPrediction={setSafetyCarPrediction}
          />
          <F1PredictionsFirstPlaceLeadSelect
            firstPlaceLead={firstPlaceLead}
            setFirstPlaceLead={setFirstPlaceLead}
          />
          <F1PredictionsDriverPositionSelect
            driver={f1Race.conditions?.positionPredictionDriver ?? ""}
            teams={teams}
            selectedPosition={driverPosition}
            setSelectedPosition={setDriverPosition}
          />
        </F1PredictionGridColumn>
      </Grid>

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
