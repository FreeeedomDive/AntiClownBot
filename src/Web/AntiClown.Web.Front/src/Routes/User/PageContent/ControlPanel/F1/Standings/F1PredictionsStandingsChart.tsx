import { DiscordMemberDto } from "../../../../../../Dto/Users/DiscordMemberDto";
import { LineChart } from "@mui/x-charts/LineChart";
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

const CHAMPION_COLOR = "#90a4ae";

const colors: string[] = [
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

export default function F1PredictionsStandingsChart({
  members,
  charts,
}: Props) {
  const { userId } = useParams<"userId">();
  const series = Array.of<PredictionsSeries>();
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

  const allPoints = series[0].points;
  const minY = Math.min(...series.map((x) => x.points[0]));
  const maxY = Math.max(...series.map((x) => x.points[x.points.length - 1]));

  return (
    <LineChart
      series={series.map((x, i) => ({
        color: x.isMe
          ? "#ffffff"
          : x.userName === "Чемпион"
            ? CHAMPION_COLOR
            : colors[i % colors.length],
        curve: "linear",
        id: `prediction_points_${x.userName}`,
        label: x.userName,
        area: false,
        data: x.points,
      }))}
      xAxis={[
        {
          data: allPoints.map((_, i) => i),
          scaleType: "linear",
        },
      ]}
      yAxis={[
        {
          min: minY,
          max: maxY,
          scaleType: "linear",
        },
      ]}
      height={600}
      grid={{ vertical: true, horizontal: true }}
    />
  );
}
