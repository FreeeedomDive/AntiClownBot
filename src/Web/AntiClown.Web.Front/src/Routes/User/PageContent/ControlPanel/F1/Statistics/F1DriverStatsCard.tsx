import { Box, Card, CardContent, LinearProgress, Typography } from "@mui/material";
import { DriverStatisticsDto } from "../../../../../../Dto/F1Predictions/DriverStatisticsDto";

interface Props {
  title: string;
  data: DriverStatisticsDto[];
  scoreLabel?: string;
}

export default function F1DriverStatsCard({ title, data, scoreLabel }: Props) {
  const maxScore = Math.max(...data.map((x) => x.score), 1);

  return (
    <Card sx={{ height: "100%", backgroundColor: "transparent" }}>
      <CardContent>
        <Typography
          variant="subtitle2"
          color="text.secondary"
          gutterBottom
          sx={{ fontWeight: 600 }}
        >
          {title}
        </Typography>
        {data.length === 0 ? (
          <Typography variant="body2" color="text.disabled">
            Нет данных
          </Typography>
        ) : (
          <Box sx={{ display: "flex", flexDirection: "column", gap: 0.75 }}>
            {data.map((entry, i) => (
              <Box key={entry.driver}>
                <Box
                  sx={{
                    display: "flex",
                    alignItems: "center",
                    mb: 0.25,
                    gap: 0.5,
                  }}
                >
                  <Typography
                    variant="caption"
                    color="text.disabled"
                    sx={{ width: 18, flexShrink: 0 }}
                  >
                    {i + 1}.
                  </Typography>
                  <Typography
                    variant="body2"
                    sx={{
                      flex: 1,
                      overflow: "hidden",
                      textOverflow: "ellipsis",
                      whiteSpace: "nowrap",
                    }}
                  >
                    {entry.driver}
                  </Typography>
                  <Typography variant="caption" fontWeight="bold">
                    {entry.score}
                    {scoreLabel && (
                      <Typography
                        component="span"
                        variant="caption"
                        color="text.disabled"
                      >
                        {" "}
                        {scoreLabel}
                      </Typography>
                    )}
                  </Typography>
                </Box>
                <LinearProgress
                  variant="determinate"
                  value={(entry.score / maxScore) * 100}
                  sx={{ height: 4, borderRadius: 2 }}
                />
              </Box>
            ))}
          </Box>
        )}
      </CardContent>
    </Card>
  );
}
