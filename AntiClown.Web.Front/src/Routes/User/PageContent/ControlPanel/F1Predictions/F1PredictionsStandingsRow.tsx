import {DiscordMemberDto} from "../../../../../Dto/Users/DiscordMemberDto";
import {F1PredictionUserResultDto} from "../../../../../Dto/F1Predictions/F1PredictionUserResultDto";
import {
  Avatar,
  Box,
  Collapse,
  IconButton,
  Stack,
  Table, TableBody,
  TableCell,
  TableHead,
  TableRow,
  Typography
} from "@mui/material";
import React from "react";
import {countPointsForRace} from "../../../../../Helpers/F1PredictionUserResultDtoHelpers";
import {countTotalPoints} from "../../../../../Helpers/F1PredictionUserResultDtoHelpers";
import KeyboardArrowDownIcon from '@mui/icons-material/KeyboardArrowDown';
import KeyboardArrowUpIcon from '@mui/icons-material/KeyboardArrowUp';
import {F1RaceDto} from "../../../../../Dto/F1Predictions/F1RaceDto";

interface IProps {
  discordMember: DiscordMemberDto | undefined;
  results: (F1PredictionUserResultDto | null)[];
  isMe: boolean;
  races: F1RaceDto[];
}

export function F1PredictionsStandingsRow({discordMember, results, isMe, races}: IProps) {
  const [isOpen, setIsOpen] = React.useState(false);
  const sumPoints = countTotalPoints(results);
  const userName = discordMember?.serverName ?? discordMember?.userName;
  return (
    <React.Fragment>
      <TableRow sx={isMe ? {backgroundColor: "#120030"} : {}}>
        <TableCell key={"expand"}>
          <IconButton
            aria-label="expand row"
            size="small"
            onClick={() => setIsOpen(!isOpen)}
          >
            {isOpen ? <KeyboardArrowUpIcon/> : <KeyboardArrowDownIcon/>}
          </IconButton>
        </TableCell>
        <TableCell key={"avatar"} align={"left"} sx={{padding: '8px'}}>
          {
            discordMember && <Stack direction={"row"} spacing={1}>
                  <Avatar
                      alt={userName}
                      src={discordMember.avatarUrl}
                      sx={{width: 24, height: 24}}
                  />
                  <Typography>{userName}</Typography>
              </Stack>
          }

        </TableCell>
        <TableCell key={"points_sum"} align={"center"} sx={{padding: '8px'}}>
          <Typography>{sumPoints}</Typography>
        </TableCell>
        {
          results.map(x =>
            <TableCell key={`${x?.raceId}_points`} sx={{padding: '4px'}}>
              <Typography>{!!x ? countPointsForRace(x) : ""}</Typography>
            </TableCell>)
        }
      </TableRow>
      <TableRow>
        <TableCell style={{paddingBottom: 0, paddingTop: 0}} colSpan={64}>
          <Collapse in={isOpen} timeout="auto" unmountOnExit>
            <Box sx={{margin: 1}}>
              <Typography variant="h6" component="div">
                История предсказаний {userName}
              </Typography>
              <Table size="small">
                <TableHead>
                  <TableRow key={"mainRow"}>
                    <TableCell key={"head_name"}>Гонка</TableCell>
                    <TableCell key={"head_10th"} align="center">10 место</TableCell>
                    <TableCell key={"head_dnf"} align="center">DNF</TableCell>
                    <TableCell key={"head_incidents"} align="center">Инциденты</TableCell>
                    <TableCell key={"head_1st"} align="center">Отрыв 1 места</TableCell>
                    <TableCell key={"head_teams"} align="center">Команда</TableCell>
                    <TableCell key={"head_sum"} align="center">Сумма</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {results.filter(race => race !== null).map(race => (
                    <TableRow key={race!.raceId}>
                      <TableCell key={`${race!.raceId}_name`}>{races.find(x => x.id === race!.raceId)!.name}</TableCell>
                      <TableCell key={`${race!.raceId}_10th`} align="center">{race!.tenthPlacePoints}</TableCell>
                      <TableCell key={`${race!.raceId}_dnf`} align="center">{race!.dnfsPoints}</TableCell>
                      <TableCell key={`${race!.raceId}_incidents`} align="center">{race!.safetyCarsPoints}</TableCell>
                      <TableCell key={`${race!.raceId}_1st`} align="center">{race!.firstPlaceLeadPoints}</TableCell>
                      <TableCell key={`${race!.raceId}_teams`} align="center">{race!.teamMatesPoints}</TableCell>
                      <TableCell key={`${race!.raceId}_sum`} align="center">{countPointsForRace(race!)}</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </Box>
          </Collapse>
        </TableCell>
      </TableRow>
    </React.Fragment>
  )
}