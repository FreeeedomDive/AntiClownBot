import {
  Box,
  FormControl,
  MenuItem,
  Select,
  Stack,
  Typography,
} from "@mui/material";
import { DiscordMemberDto } from "../../../../../../Dto/Users/DiscordMemberDto";
import { LineChart } from "@mui/x-charts/LineChart";
import { useState } from "react";
import { useParams } from "react-router-dom";
import { F1ChartsDto } from "../../../../../../Dto/F1Predictions/F1ChartsDto";

interface Props {
  members: DiscordMemberDto[];
  charts: F1ChartsDto;
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
  members,
  charts,
}: Props) {
  const [showLastRacesCount, setShowLastRacesCount] =
    useState<ShowLastNumberOfRacesType>(ShowLastNumberOfRaces.All);

  const { userId } = useParams<"userId">();
  let series = Array.of<PredictionsSeries>();
  for (const chart of charts.usersCharts) {
    const user = members.find((x) => x.userId === chart.userId);
    series.push({
      isMe: chart.userId === userId,
      userName: user?.serverName ?? user?.userName ?? "",
      points: chart.points,
    });
  }
  if (charts.championChart.points.filter((x) => x > 0).length > 0) {
    series.push({
      isMe: false,
      userName: "Чемпион",
      points: charts.championChart.points,
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
        charts.championChart.points[
          charts.championChart.points.length - sliceCount - 1
        ]
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
