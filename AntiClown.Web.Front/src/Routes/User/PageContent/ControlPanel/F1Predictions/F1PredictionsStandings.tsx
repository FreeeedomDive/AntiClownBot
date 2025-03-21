import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import F1PredictionsApi from "../../../../../Api/F1PredictionsApi";
import DiscordMembersApi from "../../../../../Api/DiscordMembersApi";
import { F1RaceDto } from "../../../../../Dto/F1Predictions/F1RaceDto";
import { F1PredictionsStandingsDto } from "../../../../../Dto/F1Predictions/F1PredictionsStandingsDto";
import { DiscordMemberDto } from "../../../../../Dto/Users/DiscordMemberDto";
import { F1PredictionsStandingsRow } from "./F1PredictionsStandingsRow";
import { countTotalPoints } from "../../../../../Helpers/F1PredictionUserResultDtoHelpers";
import {
  Avatar,
  Badge,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
} from "@mui/material";
import { convertRaceNameToFlag } from "../../../../../Helpers/RaceNameToFlagHelper";
import { Loader } from "../../../../../Components/Loader/Loader";
import F1PredictionsStandingsChart from "./F1PredictionsStandingsChart";

export default function F1PredictionsStandings() {
  const [isLoading, setIsLoading] = useState(true);
  const { userId } = useParams<"userId">();
  const [finishedRaces, setFinishedRaces] = useState<F1RaceDto[]>([]);
  const [standings, setStandings] = useState<F1PredictionsStandingsDto>({});
  const [members, setMembers] = useState<DiscordMemberDto[]>([]);

  const season = new Date().getFullYear();
  useEffect(() => {
    async function load() {
      const allFinishedRaces = await F1PredictionsApi.find({
        isActive: false,
        season,
      });
      const standings = await F1PredictionsApi.getStandings(season);
      const members = await DiscordMembersApi.getMembers(
        Object.keys(standings),
      );

      setFinishedRaces(allFinishedRaces);
      setStandings(standings);
      setMembers(members);
      setIsLoading(false);
    }

    load();
  }, []);

  const sortedStandings = Object.values(standings).sort(
    (a, b) => countTotalPoints(b) - countTotalPoints(a),
  );
  return (
    <>
      {isLoading && <Loader />}
      {!isLoading && sortedStandings && sortedStandings.length === 0 && (
        <Typography variant={"h5"}>Сезон {season} еще не стартовал</Typography>
      )}
      {!isLoading && sortedStandings && sortedStandings.length > 0 && (
        <Stack direction={"column"} spacing={4}>
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell align={"center"} sx={{ padding: "8px" }} />
                  <TableCell align={"left"} sx={{ padding: "8px" }} />
                  <TableCell align={"center"} sx={{ padding: "8px" }}>
                    Очки за сезон
                  </TableCell>
                  {finishedRaces.map((race) => {
                    const avatar = (
                      <Avatar
                        variant={"rounded"}
                        alt={race.name}
                        src={convertRaceNameToFlag(race.name)}
                        sx={{ width: 24, height: 24 }}
                      />
                    );
                    return (
                      <TableCell sx={{ padding: "4px" }}>
                        {race.isSprint ? (
                          <Badge variant={"dot"} color="info">
                            {avatar}
                          </Badge>
                        ) : (
                          avatar
                        )}
                      </TableCell>
                    );
                  })}
                </TableRow>
              </TableHead>
              <TableBody>
                {sortedStandings.map((results) => {
                  const resultsUserId = results.find(
                    (x) => x.userId !== null,
                  )!.userId;
                  return (
                    <F1PredictionsStandingsRow
                      key={resultsUserId}
                      discordMember={members.find(
                        (member) => member.userId === resultsUserId,
                      )}
                      results={standings[resultsUserId]}
                      isMe={resultsUserId === userId}
                      races={finishedRaces}
                    />
                  );
                })}
              </TableBody>
            </Table>
          </TableContainer>
          <F1PredictionsStandingsChart
            standings={standings}
            members={members}
          />
        </Stack>
      )}
    </>
  );
}
