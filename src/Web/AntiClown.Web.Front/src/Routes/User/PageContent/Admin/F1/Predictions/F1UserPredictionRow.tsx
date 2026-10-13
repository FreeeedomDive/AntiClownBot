import React, { useState } from "react";
import {
  Avatar,
  Box,
  Checkbox,
  FormControl,
  FormHelperText,
  MenuItem,
  Select,
  Stack,
  TableCell,
  TableRow,
  TextField,
  Typography,
} from "@mui/material";
import { LoadingButton } from "@mui/lab";
import { Check, Save } from "@mui/icons-material";
import F1PredictionsApi from "../../../../../../Api/F1PredictionsApi";
import { AddPredictionResultDto } from "../../../../../../Dto/F1Predictions/AddPredictionResultDto";
import { F1PredictionDto } from "../../../../../../Dto/F1Predictions/F1PredictionDto";
import { F1TeamDto } from "../../../../../../Dto/F1Predictions/F1TeamDto";
import {
  F1SafetyCarPredictionDto,
  F1SafetyCarsPredictionDto,
} from "../../../../../../Dto/F1Predictions/F1SafetyCarsPredictionDto";
import { DiscordMemberDto } from "../../../../../../Dto/Users/DiscordMemberDto";
import F1TeamBadge from "../../../ControlPanel/F1/Predictions/F1TeamBadge";

interface Props {
  prediction: F1PredictionDto;
  member?: DiscordMemberDto;
  teams: F1TeamDto[];
  drivers: string[];
  maxPosition: number;
  showPosition: boolean;
  hidden: boolean;
  disabled: boolean;
  onSavingChange: (saving: boolean) => void;
  onDirtyChange: (userId: string, dirty: boolean) => void;
  onSaved: (prediction: F1PredictionDto) => void;
}

type PredictionDraft = Omit<F1PredictionDto, "firstPlaceLeadPrediction"> & {
  firstPlaceLeadPrediction: string;
};
const NOBODY = "__nobody__";
const INCIDENT_LABELS: Record<F1SafetyCarPredictionDto, string> = {
  Zero: "0",
  One: "1",
  Two: "2",
  ThreePlus: "3+",
};

export default function F1UserPredictionRow({
  prediction,
  member,
  teams,
  drivers,
  maxPosition,
  showPosition,
  hidden,
  disabled,
  onSavingChange,
  onDirtyChange,
  onSaved,
}: Props) {
  const [draft, setDraft] = useState<PredictionDraft>(() => ({
    ...prediction,
    firstPlaceLeadPrediction: String(prediction.firstPlaceLeadPrediction),
  }));
  const [savedDraft, setSavedDraft] = useState(draft);
  const [isSaving, setIsSaving] = useState(false);
  const [status, setStatus] = useState<"saved" | "error" | null>(null);
  const userName = member?.serverName || member?.userName || prediction.userId;
  const dirty = JSON.stringify(draft) !== JSON.stringify(savedDraft);
  const dnfValues = draft.dnfPrediction.noDnfPredicted
    ? [NOBODY]
    : draft.dnfPrediction.dnfPickedDrivers ?? [];
  const validDnf =
    draft.dnfPrediction.noDnfPredicted ||
    (dnfValues.length === 5 && new Set(dnfValues).size === 5);
  const validLead =
    draft.firstPlaceLeadPrediction.trim() !== "" &&
    Number.isFinite(Number(draft.firstPlaceLeadPrediction)) &&
    Number(draft.firstPlaceLeadPrediction) >= 0;
  const validPosition =
    !showPosition ||
    (Number.isInteger(draft.driverPositionPrediction) &&
      draft.driverPositionPrediction >= 1 &&
      draft.driverPositionPrediction <= maxPosition);
  const valid =
    !!draft.tenthPlacePickedDriver && validDnf && validLead && validPosition;

  function update(patch: Partial<PredictionDraft>) {
    const next = { ...draft, ...patch };
    setDraft(next);
    setStatus(null);
    onDirtyChange(
      prediction.userId,
      JSON.stringify(next) !== JSON.stringify(savedDraft),
    );
  }

  function changeDnf(values: string[]) {
    const choseNobody =
      values.includes(NOBODY) && !draft.dnfPrediction.noDnfPredicted;
    update({
      dnfPrediction: choseNobody
        ? {
            noDnfPredicted: true,
            dnfPickedDrivers: null,
          }
        : {
            noDnfPredicted: false,
            dnfPickedDrivers: values
              .filter((value) => value !== NOBODY)
              .slice(0, 5),
          },
    });
  }

  async function save() {
    if (!dirty || !valid || disabled) return;
    setIsSaving(true);
    onSavingChange(true);
    setStatus(null);
    try {
      const updated = {
        ...draft,
        firstPlaceLeadPrediction: Number(draft.firstPlaceLeadPrediction),
      };
      const result = await F1PredictionsApi.addPrediction(
        prediction.raceId,
        updated,
        true,
      );
      if (result !== AddPredictionResultDto.Success)
        throw new Error("Prediction was not saved");
      setSavedDraft(draft);
      onDirtyChange(prediction.userId, false);
      onSaved(updated);
      setStatus("saved");
    } catch {
      setStatus("error");
    } finally {
      setIsSaving(false);
      onSavingChange(false);
    }
  }

  return (
    <TableRow
      sx={{
        display: hidden ? "none" : undefined,
        bgcolor: dirty ? "rgba(144,202,249,0.035)" : "transparent",
        "& .MuiTableCell-root": { py: 1.5, verticalAlign: "top" },
      }}
    >
      <TableCell>
        <Stack
          direction="row"
          spacing={1}
          alignItems="center"
          sx={{ minHeight: 40 }}
        >
          <Avatar
            src={member?.avatarUrl}
            alt={userName}
            sx={{ width: 28, height: 28, fontSize: "0.75rem" }}
          >
            {userName.slice(0, 1)}
          </Avatar>
          <Typography
            variant="body2"
            title={prediction.userId}
            sx={{ overflowWrap: "anywhere", maxWidth: 180 }}
          >
            {userName}
          </Typography>
        </Stack>
      </TableCell>
      <TableCell>
        <FormControl size="small" fullWidth disabled={disabled}>
          <Select
            value={draft.tenthPlacePickedDriver}
            inputProps={{ "aria-label": `10-е место — ${userName}` }}
            onChange={(event) =>
              update({ tenthPlacePickedDriver: event.target.value })
            }
          >
            {drivers.map((driver) => (
              <MenuItem key={driver} value={driver}>
                <Box sx={{ display: "flex", gap: 1, alignItems: "center" }}>
                  <F1TeamBadge driver={driver} teams={teams} />
                  {driver}
                </Box>
              </MenuItem>
            ))}
          </Select>
        </FormControl>
      </TableCell>
      <TableCell sx={{ maxWidth: 320 }}>
        <FormControl
          size="small"
          fullWidth
          disabled={disabled}
          error={!validDnf}
        >
          <Select
            multiple
            displayEmpty
            value={dnfValues}
            inputProps={{ "aria-label": `DNF — ${userName}` }}
            onChange={(event) =>
              changeDnf(
                typeof event.target.value === "string"
                  ? event.target.value.split(",")
                  : event.target.value,
              )
            }
            renderValue={(values) => {
              const label = values.includes(NOBODY)
                ? "Никто"
                : values.length
                  ? `${values.length}/5 · ${values.join(", ")}`
                  : "Выберите 5 пилотов";
              return (
                <Box
                  title={label}
                  sx={{
                    overflow: "hidden",
                    textOverflow: "ellipsis",
                    whiteSpace: "nowrap",
                    maxWidth: 260,
                  }}
                >
                  {label}
                </Box>
              );
            }}
            MenuProps={{ PaperProps: { sx: { maxHeight: 440 } } }}
          >
            <MenuItem value={NOBODY}>
              <Checkbox
                size="small"
                checked={draft.dnfPrediction.noDnfPredicted}
                sx={{ mr: 1, p: 0.5 }}
              />
              Никто
            </MenuItem>
            {drivers.map((driver) => (
              <MenuItem
                key={driver}
                value={driver}
                disabled={
                  !draft.dnfPrediction.noDnfPredicted &&
                  dnfValues.length >= 5 &&
                  !dnfValues.includes(driver)
                }
              >
                <Checkbox
                  size="small"
                  checked={dnfValues.includes(driver)}
                  sx={{ mr: 1, p: 0.5 }}
                />
                <Box sx={{ display: "flex", gap: 1, alignItems: "center" }}>
                  <F1TeamBadge driver={driver} teams={teams} />
                  {driver}
                </Box>
              </MenuItem>
            ))}
          </Select>
          {!validDnf && (
            <FormHelperText>
              Выберите ровно 5 пилотов или «Никто»
            </FormHelperText>
          )}
        </FormControl>
      </TableCell>
      <TableCell>
        <FormControl size="small" fullWidth disabled={disabled}>
          <Select
            value={draft.safetyCarsPrediction}
            inputProps={{ "aria-label": `Инциденты — ${userName}` }}
            onChange={(event) =>
              update({
                safetyCarsPrediction: event.target
                  .value as F1SafetyCarPredictionDto,
              })
            }
          >
            {F1SafetyCarsPredictionDto.map((value) => (
              <MenuItem key={value} value={value}>
                {INCIDENT_LABELS[value]}
              </MenuItem>
            ))}
          </Select>
        </FormControl>
      </TableCell>
      <TableCell>
        <TextField
          size="small"
          fullWidth
          value={draft.firstPlaceLeadPrediction}
          disabled={disabled}
          error={!validLead}
          helperText={!validLead ? "Число ≥ 0" : undefined}
          inputProps={{
            inputMode: "decimal",
            "aria-label": `Отрыв победителя в секундах — ${userName}`,
          }}
          onChange={(event) => {
            const value = event.target.value.replace(",", ".");
            if (/^\d*(\.\d*)?$/.test(value))
              update({ firstPlaceLeadPrediction: value });
          }}
        />
      </TableCell>
      {showPosition && (
        <TableCell>
          <FormControl
            size="small"
            fullWidth
            disabled={disabled}
            error={!validPosition}
          >
            <Select
              value={validPosition ? draft.driverPositionPrediction : ""}
              displayEmpty
              inputProps={{
                "aria-label": `Позиция выбранного пилота — ${userName}`,
              }}
              onChange={(event) =>
                update({ driverPositionPrediction: Number(event.target.value) })
              }
            >
              <MenuItem value="" disabled>
                —
              </MenuItem>
              {Array.from({ length: maxPosition }, (_, index) => index + 1).map(
                (position) => (
                  <MenuItem key={position} value={position}>
                    {position}
                  </MenuItem>
                ),
              )}
            </Select>
          </FormControl>
        </TableCell>
      )}
      <TableCell>
        <Stack spacing={0.5}>
          <LoadingButton
            size="small"
            variant="outlined"
            loading={isSaving}
            disabled={disabled || !dirty || !valid}
            startIcon={<Save fontSize="small" />}
            onClick={save}
            sx={{ textTransform: "none", minHeight: 40 }}
          >
            Сохранить
          </LoadingButton>
          {status === "saved" && (
            <Stack direction="row" spacing={0.5} alignItems="center">
              <Check sx={{ fontSize: 14, color: "success.main" }} />
              <Typography variant="caption" color="success.main">
                Сохранено
              </Typography>
            </Stack>
          )}
          {status === "error" && (
            <Typography variant="caption" color="error.main">
              Не удалось сохранить. Повторите попытку.
            </Typography>
          )}
          {dirty && status !== "error" && (
            <Typography variant="caption" color="text.secondary">
              Есть изменения
            </Typography>
          )}
        </Stack>
      </TableCell>
    </TableRow>
  );
}
