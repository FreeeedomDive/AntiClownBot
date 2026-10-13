import { useMemo, useState } from "react";
import {
  Alert,
  Avatar,
  Box,
  Button,
  Checkbox,
  Divider,
  FormControlLabel,
  Paper,
  Stack,
  Switch,
  TextField,
  ToggleButton,
  ToggleButtonGroup,
  Typography,
} from "@mui/material";
import { LineChart } from "@mui/x-charts/LineChart";
import { ChartsAxisContentProps } from "@mui/x-charts/ChartsTooltip";
import { LineSeriesType } from "@mui/x-charts/models";
import { useParams } from "react-router-dom";
import { DiscordMemberDto } from "../../../../../../Dto/Users/DiscordMemberDto";
import { F1ChartsDto } from "../../../../../../Dto/F1Predictions/F1ChartsDto";
import { F1RaceDto } from "../../../../../../Dto/F1Predictions/F1RaceDto";
import { F1StandingsDto } from "../../../../../../Dto/F1Predictions/F1StandingsDto";
import SortedAxisTooltip from "./SortedAxisTooltip";

interface Props {
  members: DiscordMemberDto[];
  charts: F1ChartsDto;
  races: F1RaceDto[];
  standings: F1StandingsDto;
}

interface Participant {
  userId: string;
  seriesId: string;
  isMe: boolean;
  userName: string;
  searchText: string;
  avatarUrl?: string;
  color: string;
  points: number[];
  racePoints: (number | null)[];
}

const THRESHOLD_ID = "prediction_title_threshold";
const THRESHOLD_COLOR = "#90a4ae";
const COLORS = [
  "#ffd700",
  "#c0c0c0",
  "#cd7f32",
  "#f44336",
  "#2196f3",
  "#4caf50",
  "#ff9800",
  "#9c27b0",
  "#00bcd4",
  "#e91e63",
  "#3f51b5",
  "#009688",
  "#8bc34a",
  "#ff5722",
  "#795548",
  "#607d8b",
  "#cddc39",
  "#40c4ff",
  "#69f0ae",
  "#ea80fc",
];
const PANEL_SX = {
  borderRadius: 2,
  bgcolor: "rgba(255,255,255,0.025)",
  backgroundImage: "none",
  minWidth: 0,
} as const;

function pointsAt(points: number[], index: number): number {
  return points[index] ?? points.at(-1) ?? 0;
}

export default function F1PredictionsStandingsChart({
  members,
  charts,
  races,
  standings,
}: Props) {
  const { userId } = useParams<"userId">();
  const [mode, setMode] = useState<"points" | "gap" | "race">("points");
  const [selectedUsers, setSelectedUsers] = useState<Set<string> | null>(null);
  const [showThreshold, setShowThreshold] = useState(true);
  const [search, setSearch] = useState("");

  const standingsByUserId = new Map(
    standings.standings.map((row) => [row.userId, row]),
  );
  const participants: Participant[] = charts.usersCharts.map((chart, index) => {
    const member = members.find((item) => item.userId === chart.userId);
    const resultsByRaceId = new Map(
      (standingsByUserId.get(chart.userId)?.results ?? [])
        .filter((result) => result !== null)
        .map((result) => [result.raceId, result.totalPoints]),
    );
    return {
      userId: chart.userId,
      seriesId: `prediction_points_${chart.userId}`,
      isMe: chart.userId === userId,
      userName: member?.serverName || member?.userName || chart.userId,
      searchText: [member?.serverName, member?.userName, chart.userId]
        .filter(Boolean)
        .join(" ")
        .toLocaleLowerCase(),
      avatarUrl: member?.avatarUrl,
      color:
        chart.userId === userId ? "#ffffff" : COLORS[index % COLORS.length],
      points: chart.points,
      racePoints: races.map((race) => resultsByRaceId.get(race.id) ?? null),
    };
  });
  const visibleParticipants = participants.filter(
    (participant) =>
      selectedUsers === null || selectedUsers.has(participant.userId),
  );
  const filteredParticipants = participants.filter((participant) =>
    participant.searchText.includes(search.trim().toLocaleLowerCase()),
  );
  const allSelected = visibleParticipants.length === participants.length;
  const partlySelected = visibleParticipants.length > 0 && !allSelected;
  const stageCount = Math.max(
    0,
    ...participants.map((participant) => participant.points.length),
  );
  const stages = Array.from({ length: stageCount }, (_, index) => index);
  const axisStages =
    mode === "race" ? races.map((_, index) => index + 1) : stages;
  // The leader comes from all participants, even when some lines are hidden.
  const leaderPoints = stages.map((index) =>
    Math.max(
      0,
      ...participants.map((participant) => pointsAt(participant.points, index)),
    ),
  );
  const firstThresholdStage = charts.championChart.points.findIndex(
    (value) => value > 0,
  );
  const hasThreshold = firstThresholdStage >= 0;
  const ownParticipant = participants.find((participant) => participant.isMe);
  const ownSeriesId = ownParticipant?.seriesId;

  const chartSeries: Omit<LineSeriesType, "type">[] = visibleParticipants.map(
    (participant) => ({
      id: participant.seriesId,
      label: participant.userName,
      color: participant.color,
      curve: "linear",
      area: false,
      showMark: mode === "race",
      connectNulls: false,
      data:
        mode === "race"
          ? participant.racePoints
          : stages.map((index) => {
              const points = pointsAt(participant.points, index);
              return mode === "gap" ? points - leaderPoints[index] : points;
            }),
    }),
  );

  if (mode !== "race" && showThreshold && hasThreshold) {
    chartSeries.push({
      id: THRESHOLD_ID,
      label: "Порог борьбы за титул",
      color: THRESHOLD_COLOR,
      curve: "linear",
      area: false,
      showMark: false,
      data: stages.map((index) => {
        const threshold = charts.championChart.points[index] ?? 0;
        // Keep the preceding zero so a newly appeared threshold has a visible segment.
        if (threshold <= 0 && index !== firstThresholdStage - 1) return null;
        return mode === "gap" ? threshold - leaderPoints[index] : threshold;
      }),
    });
  }

  const values = chartSeries.flatMap((series) =>
    (series.data ?? []).filter((value): value is number => value !== null),
  );
  const minY = mode === "gap" ? Math.min(-1, ...values) : 0;
  const maxY = mode === "gap" ? 0 : Math.max(1, ...values);
  const axisContent = useMemo(
    () => (props: ChartsAxisContentProps) => (
      <SortedAxisTooltip
        dataIndex={props.dataIndex ?? null}
        series={props.series
          .filter((series) => series.type === "line")
          .map((series) => ({
            id: series.id,
            label:
              typeof series.label === "function"
                ? series.label("tooltip")
                : series.label ?? String(series.id),
            data: series.data,
            color: series.color,
          }))}
        races={races}
        mode={mode}
        referenceSeriesId={THRESHOLD_ID}
      />
    ),
    [races, mode],
  );

  function toggleParticipant(id: string) {
    setSelectedUsers((previous) => {
      const next = new Set(
        previous ?? participants.map((participant) => participant.userId),
      );
      if (next.has(id)) next.delete(id);
      else next.add(id);
      return next;
    });
  }

  function selectMe() {
    if (!ownParticipant) return;
    setSelectedUsers(new Set([ownParticipant.userId]));
    setSearch("");
  }

  if (participants.length === 0 || axisStages.length === 0) {
    return <Alert severity="info">Данных для графика пока нет.</Alert>;
  }

  return (
    <Box
      sx={{
        display: "grid",
        gridTemplateColumns: {
          xs: "minmax(0, 1fr)",
          lg: "minmax(0, 1fr) 280px",
        },
        gap: 2,
        alignItems: "stretch",
      }}
    >
      <Paper
        variant="outlined"
        sx={{ ...PANEL_SX, p: 2, display: "flex", flexDirection: "column" }}
      >
        <Stack spacing={0.5}>
          <Typography fontWeight={600}>Динамика сезона</Typography>
          <Typography variant="caption" color="text.secondary">
            {mode === "points"
              ? "Накопленные очки после каждого этапа"
              : mode === "gap"
                ? "Отставание от лидера на каждом этапе · 0 — лидер"
                : "Общая сумма очков за каждую гонку · Пропуск — нет предсказания"}
          </Typography>
        </Stack>
        {chartSeries.length > 0 ? (
          <Box
            sx={{ flex: 1, minHeight: 420, minWidth: 0, position: "relative" }}
          >
            <Box sx={{ position: "absolute", inset: 0 }}>
              <LineChart
                series={chartSeries}
                xAxis={[
                  {
                    data: axisStages,
                    scaleType: "linear",
                    min: axisStages[0],
                    max: axisStages[axisStages.length - 1],
                    domainLimit: "strict",
                    tickMinStep: 1,
                    tickInterval: axisStages,
                    label: "Этап сезона",
                  },
                ]}
                yAxis={[
                  {
                    min: minY,
                    max: maxY,
                    scaleType: "linear",
                    tickMinStep: 1,
                    label:
                      mode === "gap"
                        ? "Отставание, очки"
                        : mode === "race"
                          ? "Очки за гонку"
                          : "Очки",
                  },
                ]}
                margin={{ top: 20, bottom: 55, left: 65, right: 20 }}
                grid={{ horizontal: true }}
                tooltip={{ trigger: "axis" }}
                slots={{ axisContent }}
                slotProps={{
                  legend: { hidden: true },
                  line: ({ id }) => ({
                    style: {
                      strokeWidth:
                        id === THRESHOLD_ID
                          ? 1.5
                          : id === ownSeriesId
                            ? 3
                            : 1.75,
                      strokeDasharray: id === THRESHOLD_ID ? "6 4" : undefined,
                    },
                  }),
                }}
                sx={{
                  "& .MuiChartsGrid-line": { stroke: "rgba(255,255,255,0.08)" },
                  "& .MuiChartsAxis-line, & .MuiChartsAxis-tick": {
                    stroke: "rgba(255,255,255,0.2)",
                  },
                }}
              />
            </Box>
          </Box>
        ) : (
          <Box
            sx={{
              flex: 1,
              minHeight: 420,
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
            }}
          >
            <Typography variant="body2" color="text.secondary">
              Выберите участников в настройках графика
            </Typography>
          </Box>
        )}
      </Paper>
      <Paper
        variant="outlined"
        sx={{ ...PANEL_SX, p: 2, display: "flex", flexDirection: "column" }}
      >
        <Stack spacing={2} sx={{ flex: 1, minHeight: 0 }}>
          <Typography fontWeight={600}>Настройки графика</Typography>
          <ToggleButtonGroup
            value={mode}
            exclusive
            fullWidth
            size="small"
            color="primary"
            aria-label="Режим графика"
            sx={{
              "& .MuiToggleButton-root": {
                px: 0.75,
                fontSize: "0.75rem",
                whiteSpace: "nowrap",
              },
            }}
            onChange={(_, value: "points" | "gap" | "race" | null) => {
              if (value !== null) setMode(value);
            }}
          >
            <ToggleButton
              value="points"
              sx={{ flex: 1, textTransform: "none" }}
            >
              Очки
            </ToggleButton>
            <ToggleButton value="gap" sx={{ flex: 1, textTransform: "none" }}>
              Отставание
            </ToggleButton>
            <ToggleButton
              value="race"
              sx={{ flex: 1, textTransform: "none" }}
              disabled={races.length === 0}
            >
              За гонку
            </ToggleButton>
          </ToggleButtonGroup>
          <Divider />
          <Stack spacing={1} sx={{ flex: 1, minHeight: 0 }}>
            <Stack
              direction="row"
              alignItems="center"
              justifyContent="space-between"
            >
              <Typography variant="body2" fontWeight={600}>
                Участники
              </Typography>
              <Typography variant="caption" color="text.secondary">
                {visibleParticipants.length} / {participants.length}
              </Typography>
            </Stack>
            <Stack direction="row" spacing={0.5} alignItems="center">
              <Checkbox
                size="small"
                checked={allSelected}
                indeterminate={partlySelected}
                onChange={(_, checked) =>
                  setSelectedUsers(checked ? null : new Set<string>())
                }
                inputProps={{
                  "aria-label": allSelected
                    ? "Снять выбор со всех участников"
                    : "Выбрать всех участников",
                  "aria-checked": partlySelected ? "mixed" : allSelected,
                }}
                sx={{ p: 0.5, flexShrink: 0 }}
              />
              <TextField
                size="small"
                type="search"
                placeholder="Поиск по нику"
                value={search}
                onChange={(event) => setSearch(event.target.value)}
                inputProps={{ "aria-label": "Поиск участников по нику" }}
                sx={{
                  flex: 1,
                  minWidth: 0,
                  "& .MuiInputBase-input": { px: 1, fontSize: "0.8rem" },
                  "& .MuiInputBase-root": { height: 36 },
                }}
              />
              <Button
                size="small"
                variant="outlined"
                onClick={selectMe}
                disabled={!ownParticipant}
                title="Показать только мои результаты"
                sx={{
                  textTransform: "none",
                  minWidth: 32,
                  height: 36,
                  px: 1,
                  flexShrink: 0,
                }}
              >
                Я
              </Button>
            </Stack>
            <Stack
              sx={{
                maxHeight: 280,
                overflowY: "auto",
                minHeight: 0,
                minWidth: 0,
                pr: 0.5,
                scrollbarWidth: "thin",
                scrollbarColor: "rgba(255,255,255,0.2) transparent",
                "&::-webkit-scrollbar": { width: 5 },
                "&::-webkit-scrollbar-track": {
                  backgroundColor: "transparent",
                },
                "&::-webkit-scrollbar-thumb": {
                  backgroundColor: "rgba(255,255,255,0.2)",
                  borderRadius: 3,
                },
                "&::-webkit-scrollbar-thumb:hover": {
                  backgroundColor: "rgba(255,255,255,0.35)",
                },
              }}
            >
              {filteredParticipants.map((participant) => (
                <FormControlLabel
                  key={participant.userId}
                  sx={{
                    m: 0,
                    py: 0.5,
                    minWidth: 0,
                    borderRadius: 1,
                    bgcolor: participant.isMe
                      ? "rgba(255,255,255,0.04)"
                      : "transparent",
                    "& .MuiFormControlLabel-label": { flex: 1, minWidth: 0 },
                  }}
                  control={
                    <Checkbox
                      size="small"
                      checked={
                        selectedUsers === null ||
                        selectedUsers.has(participant.userId)
                      }
                      onChange={() => toggleParticipant(participant.userId)}
                      inputProps={{
                        "aria-label": `Показать ${participant.userName}`,
                      }}
                      sx={{
                        p: 0.5,
                        mr: 0.5,
                        color: participant.color,
                        "&.Mui-checked": { color: participant.color },
                      }}
                    />
                  }
                  label={
                    <Stack direction="row" alignItems="center" spacing={0.75}>
                      <Avatar
                        src={participant.avatarUrl}
                        alt={participant.userName}
                        sx={{ width: 22, height: 22, fontSize: "0.7rem" }}
                      >
                        {participant.userName.slice(0, 1)}
                      </Avatar>
                      <Typography
                        variant="body2"
                        title={participant.userName}
                        sx={{
                          flex: 1,
                          minWidth: 0,
                          overflow: "hidden",
                          textOverflow: "ellipsis",
                          whiteSpace: "nowrap",
                          fontWeight: participant.isMe ? 600 : 400,
                        }}
                      >
                        {participant.userName}
                      </Typography>
                      <Typography
                        variant="caption"
                        color="text.secondary"
                        title="Сумма очков за сезон"
                        sx={{
                          fontVariantNumeric: "tabular-nums",
                          flexShrink: 0,
                        }}
                      >
                        {pointsAt(participant.points, stageCount - 1)}
                      </Typography>
                    </Stack>
                  }
                />
              ))}
              {filteredParticipants.length === 0 && (
                <Typography
                  variant="body2"
                  color="text.secondary"
                  sx={{ py: 2, textAlign: "center" }}
                >
                  Участники не найдены
                </Typography>
              )}
            </Stack>
          </Stack>
          {mode !== "race" && (
            <>
              <Divider />
              <Stack spacing={0.5}>
                <FormControlLabel
                  sx={{ m: 0, alignItems: "center" }}
                  control={
                    <Switch
                      size="small"
                      checked={hasThreshold && showThreshold}
                      disabled={!hasThreshold}
                      onChange={(_, checked) => setShowThreshold(checked)}
                    />
                  }
                  label={
                    <Typography variant="body2">
                      Порог борьбы за титул
                    </Typography>
                  }
                />
                <Stack direction="row" alignItems="center" spacing={1}>
                  <Box
                    sx={{
                      width: 24,
                      borderTop: `2px dashed ${THRESHOLD_COLOR}`,
                      flexShrink: 0,
                    }}
                  />
                  <Typography variant="caption" color="text.secondary">
                    {hasThreshold
                      ? "Минимум очков для сохранения шансов на титул"
                      : "Порог пока не появился"}
                  </Typography>
                </Stack>
              </Stack>
            </>
          )}
        </Stack>
      </Paper>
    </Box>
  );
}
