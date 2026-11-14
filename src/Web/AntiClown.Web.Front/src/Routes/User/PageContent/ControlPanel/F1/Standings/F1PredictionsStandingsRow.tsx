import { DiscordMemberDto } from "../../../../../../Dto/Users/DiscordMemberDto";
import { F1RacePointsView as F1PredictionUserResultDto, F1RaceSummary as F1RaceDto, F1StandingsRowView as F1StandingsRowDto } from "../../../../../../Dto/F1Predictions/F1InternalDto";
import {
  Avatar,
  Box,
  Collapse,
  IconButton,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  Tooltip,
  Typography,
} from "@mui/material";
import { lighten } from "@mui/material/styles";
import React from "react";
import KeyboardArrowDownIcon from "@mui/icons-material/KeyboardArrowDown";
import KeyboardArrowUpIcon from "@mui/icons-material/KeyboardArrowUp";
import EmojiEventsIcon from "@mui/icons-material/EmojiEvents";
import DiscordMember from "../../../../../../Components/Users/DiscordMember";
import { MAIN_COLOR } from "../../../../../../Helpers/Colors";

const STICKY_BG_DEFAULT = lighten(MAIN_COLOR, 0.025);
const STICKY_BG_ME = "#200056";
const PODIUM_COLORS = ["#FFD700", "#C0C0C0", "#CD7F32"];

const POINT_COLUMNS = [
  { key: "tenthPlacePoints", label: "10 место" },
  { key: "dnfsPoints", label: "DNF" },
  { key: "safetyCarsPoints", label: "Инциденты" },
  { key: "firstPlaceLeadPoints", label: "Отрыв 1 места" },
  { key: "teamMatesPoints", label: "Команды" },
  { key: "driverPositionPoints", label: "Позиция гонщика" },
] as const;
type PointColumn = typeof POINT_COLUMNS[number];

interface IProps {
  discordMember: DiscordMemberDto | undefined;
  results: F1StandingsRowDto;
  isMe: boolean;
  races: F1RaceDto[];
  position: number;
  pointsLeft: number;
  leaderPoints: number;
}

export function F1PredictionsStandingsRow({
  discordMember,
  results,
  isMe,
  races,
  position,
  pointsLeft,
  leaderPoints,
}: IProps) {
  const [isOpen, setIsOpen] = React.useState(false);

  const userName = discordMember?.serverName ?? discordMember?.userName ?? results.participantName;

  const getRace = (id: string) => {
    return races.find((x) => x.id === id)!;
  };

  const columns = POINT_COLUMNS.filter((column) => results.results.some((result) => result?.[column.key] != null));
  const stillInChampionship = leaderPoints - results.totalPoints < pointsLeft;
  const stickyBg = isMe ? STICKY_BG_ME : STICKY_BG_DEFAULT;
  const resultsByRaceId = new Map(
    results.results
      .filter((result) => result !== null)
      .map((result) => [result.raceId, result]),
  );

  return (
    <React.Fragment>
      <TableRow sx={isMe ? { backgroundColor: STICKY_BG_ME } : {}}>
        <TableCell
          sx={{
            position: { xs: "static", md: "sticky" },
            left: { xs: "auto", md: 0 },
            zIndex: { xs: 0, md: 2 },
            bgcolor: stickyBg,
            borderRight: "1px solid rgba(255,255,255,0.12)",
            p: "4px 8px",
          }}
        >
          <Stack direction="row" alignItems="center" spacing={0.5}>
            <IconButton
              aria-label="expand row"
              size="small"
              onClick={() => setIsOpen(!isOpen)}
              sx={{ flexShrink: 0 }}
            >
              {isOpen ? <KeyboardArrowUpIcon /> : <KeyboardArrowDownIcon />}
            </IconButton>
            {(
              <Box
                sx={{
                  display: "flex",
                  alignItems: "center",
                  gap: 0.5,
                  flex: 1,
                  overflow: "hidden",
                  minWidth: 0,
                }}
              >
                <Avatar
                  alt={userName}
                   src={discordMember?.avatarUrl}
                  sx={{ width: 24, height: 24, flexShrink: 0 }}
                />
                 {discordMember ? <DiscordMember member={discordMember} /> : <Typography variant="body2">{userName}</Typography>}
                <Stack direction="row" spacing={0.25} sx={{ flexShrink: 0 }}>
                  {results.previousPodiums.map((podium) => {
                    const label = `${podium.season}: ${podium.place} место`;
                    return (
                      <Tooltip key={podium.season} title={label} arrow>
                        <Box
                          component="span"
                          role="img"
                          tabIndex={0}
                          aria-label={label}
                          sx={{ display: "inline-flex" }}
                        >
                          <EmojiEventsIcon
                            sx={{
                              color: PODIUM_COLORS[podium.place - 1],
                              fontSize: 19,
                            }}
                          />
                        </Box>
                      </Tooltip>
                    );
                  })}
                </Stack>
              </Box>
            )}
          </Stack>
        </TableCell>
        {races.map((race) => {
          const raceResult = resultsByRaceId.get(race.id);
          return (
            <TableCell
              key={`${results.userId}_${race.id}_points`}
              align="center"
              sx={{ p: "4px", width: 40 }}
            >
              {raceResult && (
                <Tooltip
                  title={
                    <Typography
                      sx={{ whiteSpace: "pre-line", fontSize: "0.75rem" }}
                    >
                      {getPointsDetails(raceResult)}
                    </Typography>
                  }
                  arrow
                  placement="top"
                >
                  <Typography sx={{ cursor: "default" }}>
                    {raceResult.totalPoints}
                  </Typography>
                </Tooltip>
              )}
            </TableCell>
          );
        })}
        <TableCell
          align="center"
          sx={{
            position: "sticky",
            right: 0,
            zIndex: 2,
            bgcolor: stickyBg,
            borderLeft: "1px solid rgba(255,255,255,0.12)",
            p: "4px 8px",
          }}
        >
          <Box
            sx={{
              display: "flex",
              alignItems: "baseline",
              justifyContent: "center",
              gap: 0.5,
            }}
          >
            <Typography
              fontWeight="bold"
              color={position === 1 && pointsLeft === 0 ? "gold" : "inherit"}
            >
              {results.totalPoints}
            </Typography>
            {position > 1 && pointsLeft > 0 && (
              <Tooltip
                title={
                  stillInChampionship
                    ? "В борьбе за титул"
                    : "Больше нет шансов на титул"
                }
                arrow
              >
                <Typography
                  variant="caption"
                  color={stillInChampionship ? "error" : "text.disabled"}
                >
                  {results.totalPoints - leaderPoints}
                </Typography>
              </Tooltip>
            )}
            {position > 1 && pointsLeft === 0 && (
              <Typography variant="caption" color="text.disabled">
                {results.totalPoints - leaderPoints}
              </Typography>
            )}
          </Box>
        </TableCell>
      </TableRow>
      <TableRow>
        <TableCell
          style={{ paddingBottom: 0, paddingTop: 0 }}
          colSpan={2 + races.length}
        >
          <Collapse in={isOpen} timeout="auto" unmountOnExit>
            <Box sx={{ margin: 1 }}>
              <Typography variant="h6" component="div">
                История предсказаний {userName}
              </Typography>
              <Table size="small">
                <TableHead>{getTableHeaderRows(columns)}</TableHead>
                <TableBody>
                  {results.results
                    .filter((raceResult) => raceResult !== null)
                    .map((raceResult) =>
                      getTableRow(
                        raceResult,
                        getRace(raceResult.raceId),
                        columns,
                      ),
                    )}
                </TableBody>
              </Table>
            </Box>
          </Collapse>
        </TableCell>
      </TableRow>
    </React.Fragment>
  );
}

function getTableHeaderRows(columns: readonly PointColumn[]) {
  return (
    <TableRow key={"mainRow"}>
      <TableCell key={"head_name"}>Гонка</TableCell>
      {columns.map((column) => <TableCell key={column.key} align="center">{column.label}</TableCell>)}
      <TableCell key={"head_sum"} align="center">
        Сумма
      </TableCell>
    </TableRow>
  );
}

function getTableRow(
  raceResult: F1PredictionUserResultDto,
  race: F1RaceDto,
  columns: readonly PointColumn[],
) {
  const raceName = race.name + (race.isSprint ? " (спринт)" : "");
  return (
    <TableRow key={`${raceResult.userId}_${raceResult.raceId}`}>
      <TableCell key={`${raceResult.userId}_${raceResult.raceId}_name`}>
        {raceName}
      </TableCell>
      {columns.map((column) => <TableCell key={column.key} align="center">{raceResult[column.key] ?? "—"}</TableCell>)}
      <TableCell
        key={`${raceResult.userId}_${raceResult.raceId}_sum`}
        align="center"
      >
        {raceResult.totalPoints}
      </TableCell>
    </TableRow>
  );
}

function getPointsDetails(
  raceResult: F1PredictionUserResultDto,
) {
  const parts = POINT_COLUMNS.filter((column) => raceResult[column.key] !== null)
    .map((column) => `${column.label}: ${raceResult[column.key]}`);
  parts.push(`Сумма: ${raceResult.totalPoints}`, raceResult.rulesVersion);
  return parts.join("\n");
}
