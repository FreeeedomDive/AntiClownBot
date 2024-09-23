import { F1PredictionsStandingsDto } from "../../../../../Dto/F1Predictions/F1PredictionsStandingsDto";
import { DiscordMemberDto } from "../../../../../Dto/Users/DiscordMemberDto";
import {countPointsForRace} from "../../../../../Helpers/F1PredictionUserResultDtoHelpers";
import { LineChart } from "@mui/x-charts/LineChart";
import React from "react";
import {useParams} from "react-router-dom";

interface Props {
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
]

export default function F1PredictionsStandingsChart({
  standings,
  members,
}: Props) {
  const {userId} = useParams<"userId">();
  const series = Array.of<PredictionsSeries>();
  for (let standingsUserId in standings) {
    const userResults = standings[standingsUserId];
    let currentSum = 0;
    const points = [currentSum];
    for (const raceResult of userResults) {
      if (raceResult === null || raceResult === undefined) {
        points.push(currentSum);
        continue;
      }

      currentSum += countPointsForRace(raceResult);
      points.push(currentSum);
    }

    const user = members.find((x) => x.userId === standingsUserId);
    series.push({
      isMe: standingsUserId === userId,
      userName: user?.serverName ?? user?.userName ?? "",
      points: points,
    });
  }
  series.sort((a, b) => b.points[b.points.length - 1] - a.points[a.points.length - 1]);

  const totalRaces = 30;
  const maxPointsPerRace = 55;
  const possibleChampionPoints = series[0].points.map((currentLeaderPoints, raceNumber) => Math.max(0, currentLeaderPoints - ((totalRaces - raceNumber) * maxPointsPerRace)));
  if (possibleChampionPoints.filter(x => x > 0).length > 0){
    series.push({
      isMe: false,
      userName: "Необходимое количество очков для чемпионства",
      points: possibleChampionPoints
    })
  }

  return (
    <LineChart
      series={series.map((x, i) => {
        return {
          color: x.isMe ? "#ffffff" : colors[i % colors.length],
          curve: "linear",
          id: `prediction_points_${x.userName}`,
          label: x.userName,
          area: false,
          data: x.points,
        };
      })}
      height={600}
      grid={{ vertical: true, horizontal: true }}
    />
  );
}
