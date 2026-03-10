import { DiscordMemberDto } from "../../../../../../Dto/Users/DiscordMemberDto";
import { F1PredictionUserResultDto } from "../../../../../../Dto/F1Predictions/F1PredictionUserResultDto";
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
import React from "react";
import KeyboardArrowDownIcon from "@mui/icons-material/KeyboardArrowDown";
import KeyboardArrowUpIcon from "@mui/icons-material/KeyboardArrowUp";
import { F1RaceDto } from "../../../../../../Dto/F1Predictions/F1RaceDto";
import DiscordMember from "../../../../../../Components/Users/DiscordMember";
import { InfoOutlined } from "@mui/icons-material";
import { F1StandingsRowDto } from "../../../../../../Dto/F1Predictions/F1StandingsRowDto";

const STICKY_BG_DEFAULT = "#000019";
const STICKY_BG_ME = "#120030";

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

  const userName = discordMember?.serverName ?? discordMember?.userName;

  const getRace = (id: string) => {
    return races.find((x) => x.id === id)!;
  };

  const season =
    races.length === 0 ? new Date().getFullYear() : races[0].season;
  const stillInChampionship = leaderPoints - results.totalPoints < pointsLeft;
  const stickyBg = isMe ? STICKY_BG_ME : STICKY_BG_DEFAULT;

  return (
    <React.Fragment>
      <TableRow sx={isMe ? { backgroundColor: STICKY_BG_ME } : {}}>
        <TableCell
          sx={{
            position: "sticky",
            left: 0,
            zIndex: 2,
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
            {discordMember && (
              <>
                <Avatar
                  alt={userName}
                  src={discordMember.avatarUrl}
                  sx={{ width: 24, height: 24, flexShrink: 0 }}
                />
                <Box sx={{ flex: 1, overflow: "hidden", minWidth: 0 }}>
                  <DiscordMember member={discordMember} />
                </Box>
              </>
            )}
          </Stack>
        </TableCell>
        {results.results.map((x) => (
          <TableCell
            key={`${discordMember?.userId}_${x?.raceId}_points`}
            align="center"
            sx={{ p: "4px", width: 40 }}
          >
            {!!x && (
              <Tooltip
                title={
                  <Typography
                    sx={{ whiteSpace: "pre-line", fontSize: "0.75rem" }}
                  >
                    {getPointsDetails(x, getRace(x.raceId), season)}
                  </Typography>
                }
                arrow
                placement="top"
              >
                <Typography sx={{ cursor: "default" }}>
                  {x.totalPoints}
                </Typography>
              </Tooltip>
            )}
          </TableCell>
        ))}
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
          <Typography fontWeight="bold">{results.totalPoints}</Typography>
          {position === 1 && pointsLeft === 0 && (
            <Typography variant="caption" color="gold" display="block">
              Чемпион
            </Typography>
          )}
          {position === 1 && pointsLeft > 0 && (
            <Typography variant="caption" color="success.main" display="block">
              Лидер
            </Typography>
          )}
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
                display="block"
                color={stillInChampionship ? "error" : "text.disabled"}
              >
                {results.totalPoints - leaderPoints}
              </Typography>
            </Tooltip>
          )}
          {position > 1 && pointsLeft === 0 && (
            <Typography variant="caption" display="block" color="text.disabled">
              {results.totalPoints - leaderPoints}
            </Typography>
          )}
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
                <TableHead>{getTableHeaderRows(season)}</TableHead>
                <TableBody>
                  {results.results
                    .filter((raceResult) => raceResult !== null)
                    .map((raceResult) =>
                      getTableRow(
                        raceResult,
                        getRace(raceResult.raceId),
                        season,
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

function getTableHeaderRows(season: number) {
  return (
    <TableRow key={"mainRow"}>
      <TableCell key={"head_name"}>Гонка</TableCell>
      {/*За 10 место голосование есть всегда*/}
      <TableCell key={"head_10th"} align="center">
        10 место
      </TableCell>
      {/*В 2023 голосовали за первый DNF в гонке, затем выбирали 5 гонщиков*/}
      <TableCell key={"head_dnf"} align="center">
        {season === 2023 ? "Первый DNF" : "DNF"}
      </TableCell>
      {/*Голосование за количество инцидентов началось с 2024 года*/}
      {season > 2023 && (
        <TableCell key={"head_incidents"} align="center">
          Инциденты
        </TableCell>
      )}
      {/*Голосование за отрыв 1 места началось с 2024 года*/}
      {season > 2023 && (
        <TableCell key={"head_1st"} align="center">
          Отрыв 1 места
        </TableCell>
      )}
      {/*Голосование за победителей внутри команд было с 2024 по 2025 год*/}
      {(season === 2024 || season === 2025) && (
        <TableCell key={"head_teams"} align="center">
          Команды
        </TableCell>
      )}
      {/*Голосование за итоговую позицию гонщика началось в 2026 году*/}
      {season >= 2026 && (
        <TableCell key={"head_driver_position"} align="center">
          Позиция гонщика
        </TableCell>
      )}
      {/*
      В 2023 году не было предсказаний спринтов
      В 2024 году предсказания спринтов были и начисляли полные очки
      В 2025 году предсказания спринтов приносили 30% очков
      */}
      <TableCell key={"head_sum"} align="center">
        Сумма
        {season === 2025 && (
          <Tooltip title="За спринт начисляется 30% очков" arrow>
            <IconButton
              sx={{
                minWidth: "auto",
                borderRadius: "50%",
                backgroundColor: "transparent",
              }}
            >
              <InfoOutlined sx={{ fontSize: 18 }} />
            </IconButton>
          </Tooltip>
        )}
      </TableCell>
    </TableRow>
  );
}

function getTableRow(
  raceResult: F1PredictionUserResultDto,
  race: F1RaceDto,
  season: number,
) {
  const raceName = race.name + (race.isSprint ? " (спринт)" : "");
  return (
    <TableRow key={`${raceResult.userId}_${raceResult.raceId}`}>
      <TableCell key={`${raceResult.userId}_${raceResult.raceId}_name`}>
        {raceName}
      </TableCell>
      {/*За 10 место голосование есть всегда*/}
      <TableCell
        key={`${raceResult.userId}_${raceResult.raceId}_10th`}
        align="center"
      >
        {raceResult.tenthPlacePoints}
      </TableCell>
      {/*В 2023 голосовали за первый DNF в гонке, затем выбирали 5 гонщиков*/}
      <TableCell
        key={`${raceResult.userId}_${raceResult.raceId}_dnf`}
        align="center"
      >
        {raceResult.dnfsPoints}
      </TableCell>
      {/*Голосование за количество инцидентов началось с 2024 года*/}
      {season > 2023 && (
        <TableCell
          key={`${raceResult.userId}_${raceResult.raceId}_incidents`}
          align="center"
        >
          {raceResult.safetyCarsPoints}
        </TableCell>
      )}
      {/*Голосование за отрыв 1 места началось с 2024 года*/}
      {season > 2023 && (
        <TableCell
          key={`${raceResult.userId}_${raceResult.raceId}_1st`}
          align="center"
        >
          {raceResult.firstPlaceLeadPoints}
        </TableCell>
      )}
      {/*Голосование за победителей внутри команд было с 2024 по 2025 год*/}
      {(season === 2024 || season === 2025) && (
        <TableCell
          key={`${raceResult.userId}_${raceResult.raceId}_teams`}
          align="center"
        >
          {raceResult.teamMatesPoints}
        </TableCell>
      )}
      {/*Голосование за итоговую позицию гонщика началось в 2026 году*/}
      {season >= 2026 && (
        <TableCell
          key={`${raceResult.userId}_${raceResult.raceId}_driver_position`}
          align="center"
        >
          {raceResult.driverPositionPoints}
        </TableCell>
      )}
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
  race: F1RaceDto,
  season: number,
) {
  const parts: string[] = [];
  parts.push(`10 место: ${raceResult.tenthPlacePoints}`);
  parts.push(
    (season === 2023 ? "Первый DNF" : "DNF") + `: ${raceResult.dnfsPoints}`,
  );
  if (season > 2023) {
    parts.push(`Инциденты: ${raceResult.safetyCarsPoints}`);
    parts.push(`Отрыв 1 места: ${raceResult.firstPlaceLeadPoints}`);
    if (season === 2024 || season === 2025) {
      parts.push(`Команды: ${raceResult.teamMatesPoints}`);
    }
  }
  if (season >= 2026 && !!race.conditions) {
    parts.push(
      `Позиция ${race.conditions.positionPredictionDriver}: ${raceResult.driverPositionPoints}`,
    );
  }
  parts.push(
    `Сумма: ${raceResult.totalPoints}` +
      (race.isSprint ? " (30% за спринт)" : ""),
  );

  return parts.join("\n");
}
