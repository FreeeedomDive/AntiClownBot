import {
  Box,
  FormControl,
  MenuItem,
  Select,
  Stack,
  Typography,
} from "@mui/material";
import { F1PredictionsStandingsDto } from "../../../../../../Dto/F1Predictions/F1PredictionsStandingsDto";
import { DiscordMemberDto } from "../../../../../../Dto/Users/DiscordMemberDto";
import { LineChart } from "@mui/x-charts/LineChart";
import { useState } from "react";
import { useParams } from "react-router-dom";

interface Props {
  season: number;
  standings: F1PredictionsStandingsDto;
  members: DiscordMemberDto[];
}

interface PredictionsSeries {
  isMe: boolean;
  userName: string;
  points: number[];
}

const colors: string[] = [
  "#ffd700",
  "#c0c0c0",
  "#cd7f32",
  "#930000",
  "#2300d6",
  "#005a0f",
  "#b600ad",
  "#00cf29",
  "#ff7c7c",
  "#37c2ff",
];

export const ShowLastNumberOfRaces = {
  All: "Показывать все гонки",
  Last5: "Показывать последние 5 гонок",
  Last10: "Показывать последние 10 гонок",
} as const;
export type ShowLastNumberOfRacesType =
  (typeof ShowLastNumberOfRaces)[keyof typeof ShowLastNumberOfRaces];

export default function F1PredictionsStandingsChart({
  season,
  standings,
  members,
}: Props) {
  const [showLastRacesCount, setShowLastRacesCount] =
    useState<ShowLastNumberOfRacesType>(ShowLastNumberOfRaces.All);

  const { userId } = useParams<"userId">();
  let series = Array.of<PredictionsSeries>();
  for (let standingsUserId in standings) {
    const userResults = standings[standingsUserId];
    let currentSum = 0;
    const points = [currentSum];
    for (const raceResult of userResults) {
      if (raceResult === null || raceResult === undefined) {
        points.push(currentSum);
        continue;
      }

      currentSum += raceResult.totalPoints;
      points.push(currentSum);
    }

    const user = members.find((x) => x.userId === standingsUserId);
    series.push({
      isMe: standingsUserId === userId,
      userName: user?.serverName ?? user?.userName ?? "",
      points: points,
    });
  }
  series.sort(
    (a, b) => b.points[b.points.length - 1] - a.points[a.points.length - 1],
  );

  const totalRaces = season === 2023 ? 20 : 30;
  const maxPointsPerRace = season === 2023 ? 30 : 55;
  const possibleChampionPoints = series[0].points.map(
    (currentLeaderPoints, raceNumber) =>
      Math.max(
        0,
        currentLeaderPoints - (totalRaces - raceNumber) * maxPointsPerRace,
      ),
  );
  if (possibleChampionPoints.filter((x) => x > 0).length > 0) {
    series.push({
      isMe: false,
      userName: "Необходимое количество очков для чемпионства",
      points: possibleChampionPoints,
    });
  }

  const sliceCount =
    showLastRacesCount === ShowLastNumberOfRaces.All
      ? series[0].points.length
      : showLastRacesCount === ShowLastNumberOfRaces.Last10 &&
          series[0].points.length > 10
        ? 10
        : showLastRacesCount === ShowLastNumberOfRaces.Last5 &&
            series[0].points.length > 5
          ? 5
          : series[0].points.length;

  if (
    (showLastRacesCount === ShowLastNumberOfRaces.Last5 &&
      series[0].points.length > 5) ||
    (showLastRacesCount === ShowLastNumberOfRaces.Last10 &&
      series[0].points.length > 10)
  ) {
    series = series.filter((x) => {
      return (
        x.points[x.points.length - sliceCount - 1] >=
        possibleChampionPoints[possibleChampionPoints.length - sliceCount - 1]
      );
    });
  }

  const getMinY = () => {
    const pts = series.map((x) => x.points.slice(-sliceCount)[0]);
    return Math.min(...pts);
  };

  const getMaxY = () => {
    const pts = series.map((x) => {
      const slice = x.points.slice(-sliceCount);
      return slice[slice.length - 1];
    });
    return Math.max(...pts);
  };

  return (
    <Stack direction={"column"}>
      <Box display="flex" justifyContent="space-between" alignItems="center">
        <Typography variant="h6">График чемпионата</Typography>
        <FormControl>
          <Select
            labelId="last-races-count-select"
            id="last-races-count-select"
            value={showLastRacesCount}
            onChange={async (event) => {
              const value = event.target.value as ShowLastNumberOfRacesType;
              setShowLastRacesCount(value);
            }}
          >
            {Object.values(ShowLastNumberOfRaces).map((numberOfRaces) => (
              <MenuItem key={numberOfRaces} value={numberOfRaces}>
                {numberOfRaces}
              </MenuItem>
            ))}
          </Select>
        </FormControl>
      </Box>
      <LineChart
        series={series.map((x, i) => {
          return {
            color: x.isMe ? "#ffffff" : colors[i % colors.length],
            curve: "linear",
            id: `prediction_points_${x.userName}`,
            label: x.userName,
            area: false,
            data: x.points.slice(-sliceCount),
          };
        })}
        xAxis={[
          {
            data: series[0].points.map((_, i) => i).slice(-sliceCount),
            scaleType: "linear",
          },
        ]}
        yAxis={[
          {
            min: getMinY(),
            max: getMaxY(),
            scaleType: "linear",
          },
        ]}
        height={600}
        grid={{ vertical: true, horizontal: true }}
      />
    </Stack>
  );
}
