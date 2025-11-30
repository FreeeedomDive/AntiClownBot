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
import { countTotalPoints } from "../../../../../../Helpers/F1PredictionUserResultDtoHelpers";
import KeyboardArrowDownIcon from "@mui/icons-material/KeyboardArrowDown";
import KeyboardArrowUpIcon from "@mui/icons-material/KeyboardArrowUp";
import { F1RaceDto } from "../../../../../../Dto/F1Predictions/F1RaceDto";
import DiscordMember from "../../../../../../Components/Users/DiscordMember";
import { InfoOutlined } from "@mui/icons-material";

interface IProps {
  discordMember: DiscordMemberDto | undefined;
  results: (F1PredictionUserResultDto | null)[];
  isMe: boolean;
  races: F1RaceDto[];
}

export function F1PredictionsStandingsRow({
  discordMember,
  results,
  isMe,
  races,
}: IProps) {
  const [isOpen, setIsOpen] = React.useState(false);
  const sumPoints = countTotalPoints(results);
  const userName = discordMember?.serverName ?? discordMember?.userName;

  const getRaceName = (id: string) => {
    const race = races.find((x) => x.id === id)!;
    return race.name + (race.isSprint ? " (спринт)" : "");
  };
  const season =
    races.length === 0 ? new Date().getFullYear() : races[0].season;

  return (
    <React.Fragment>
      <TableRow sx={isMe ? { backgroundColor: "#120030" } : {}}>
        <TableCell key={"expand"}>
          <IconButton
            aria-label="expand row"
            size="small"
            onClick={() => setIsOpen(!isOpen)}
          >
            {isOpen ? <KeyboardArrowUpIcon /> : <KeyboardArrowDownIcon />}
          </IconButton>
        </TableCell>
        <TableCell key={"avatar"} align={"left"} sx={{ padding: "8px" }}>
          {discordMember && (
            <Stack direction={"row"} spacing={1}>
              <Avatar
                alt={userName}
                src={discordMember.avatarUrl}
                sx={{ width: 24, height: 24 }}
              />
              <DiscordMember member={discordMember} />
            </Stack>
          )}
        </TableCell>
        <TableCell key={"points_sum"} align={"center"} sx={{ padding: "8px" }}>
          <Typography>{sumPoints}</Typography>
        </TableCell>
        {results.map((x) => (
          <TableCell
            key={`${discordMember?.userId}_${x?.raceId}_points`}
            sx={{ padding: "4px" }}
          >
            <Typography>{!!x ? x.totalPoints : ""}</Typography>
          </TableCell>
        ))}
      </TableRow>
      <TableRow>
        <TableCell style={{ paddingBottom: 0, paddingTop: 0 }} colSpan={64}>
          <Collapse in={isOpen} timeout="auto" unmountOnExit>
            <Box sx={{ margin: 1 }}>
              <Typography variant="h6" component="div">
                История предсказаний {userName}
              </Typography>
              <Table size="small">
                <TableHead>{getTableHeaderRows(season)}</TableHead>
                <TableBody>
                  {results
                    .filter((race) => race !== null)
                    .map((race) => getTableRow(race, getRaceName, season))}
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
  race: F1PredictionUserResultDto | null,
  getRaceName: (id: string) => string,
  season: number,
) {
  return (
    <TableRow key={race!.raceId}>
      <TableCell key={`${race!.raceId}_name`}>
        {getRaceName(race!.raceId)}
      </TableCell>
      {/*За 10 место голосование есть всегда*/}
      <TableCell key={`${race!.raceId}_10th`} align="center">
        {race!.tenthPlacePoints}
      </TableCell>
      {/*В 2023 голосовали за первый DNF в гонке, затем выбирали 5 гонщиков*/}
      <TableCell key={`${race!.raceId}_dnf`} align="center">
        {race!.dnfsPoints}
      </TableCell>
      {/*Голосование за количество инцидентов началось с 2024 года*/}
      {season > 2023 && (
        <TableCell key={`${race!.raceId}_incidents`} align="center">
          {race!.safetyCarsPoints}
        </TableCell>
      )}
      {/*Голосование за отрыв 1 места началось с 2024 года*/}
      {season > 2023 && (
        <TableCell key={`${race!.raceId}_1st`} align="center">
          {race!.firstPlaceLeadPoints}
        </TableCell>
      )}
      {/*Голосование за победителей внутри команд было с 2024 по 2025 год*/}
      {(season === 2024 || season === 2025) && (
        <TableCell key={`${race!.raceId}_teams`} align="center">
          {race!.teamMatesPoints}
        </TableCell>
      )}
      <TableCell key={`${race!.raceId}_sum`} align="center">
        {race!.totalPoints}
      </TableCell>
    </TableRow>
  );
}