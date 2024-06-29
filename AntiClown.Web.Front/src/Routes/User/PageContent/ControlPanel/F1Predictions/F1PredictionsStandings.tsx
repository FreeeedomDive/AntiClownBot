import {useParams} from "react-router-dom";
import {useEffect, useState} from "react";
import F1PredictionsApi from "../../../../../Api/F1PredictionsApi";
import DiscordMembersApi from "../../../../../Api/DiscordMembersApi";
import {F1RaceDto} from "../../../../../Dto/F1Predictions/F1RaceDto";
import {F1PredictionsStandingsDto} from "../../../../../Dto/F1Predictions/F1PredictionsStandingsDto";
import {DiscordMemberDto} from "../../../../../Dto/Users/DiscordMemberDto";
import {F1PredictionsStandingsRow} from "./F1PredictionsStandingsRow";
import {countTotalPoints} from "../../../../../Helpers/F1PredictionUserResultDtoHelpers";
import {
  Avatar, Badge,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow
} from "@mui/material";
import {convertRaceNameToFlag} from "../../../../../Helpers/RaceNameToFlagHelper";

export default function F1PredictionsStandings() {
  const {userId} = useParams<"userId">();
  const [finishedRaces, setFinishedRaces] = useState<F1RaceDto[]>([]);
  const [standings, setStandings] = useState<F1PredictionsStandingsDto>({});
  const [members, setMembers] = useState<DiscordMemberDto[]>([]);

  useEffect(() => {
    async function load() {
      const season = new Date().getFullYear();
      const allFinishedRaces = await F1PredictionsApi.find({
        isActive: false,
        season
      });
      const standings = await F1PredictionsApi.getStandings(season);
      const members = await DiscordMembersApi.getMembers(Object.keys(standings));

      setFinishedRaces(allFinishedRaces);
      setStandings(standings);
      setMembers(members);
    }

    load();
  }, []);

  const sortedStandings = Object.values(standings).sort((a, b) => countTotalPoints(b) - countTotalPoints(a));
  return (
    <>
      <TableContainer>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell align={"center"} sx={{padding: '8px'}}/>
              <TableCell align={"left"} sx={{padding: '8px'}}/>
              <TableCell align={"center"} sx={{padding: '8px'}}>
                Очки за сезон
              </TableCell>
              {
                finishedRaces.map(
                  race => {
                    const avatar = <Avatar
                      variant={"rounded"}
                      alt={race.name}
                      src={convertRaceNameToFlag(race.name)}
                      sx={{width: 24, height: 24}}
                    />;
                    return (<TableCell sx={{padding: '4px'}}>
                      {
                        race.name.indexOf("спринт") > 0
                          ?
                          <Badge variant={"dot"} color="info">
                            {avatar}
                          </Badge>
                          : avatar
                      }
                    </TableCell>)
                  }
                )
              }
            </TableRow>
          </TableHead>
          <TableBody>
            {sortedStandings
              .map(results => {
                const resultsUserId = results.find(x => x.userId !== null)!.userId;
                return (<F1PredictionsStandingsRow
                  key={resultsUserId}
                  discordMember={members.find(member => member.userId === resultsUserId)}
                  results={standings[resultsUserId]}
                  isMe={resultsUserId === userId}
                  races={finishedRaces}
                />);
              })
            }
          </TableBody>
        </Table>
      </TableContainer>
    </>
  )
}