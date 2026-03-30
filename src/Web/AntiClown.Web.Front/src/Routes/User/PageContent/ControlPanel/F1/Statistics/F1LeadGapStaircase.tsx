import { Box, Card, CardContent, Tooltip, Typography } from "@mui/material";
import { LeadGapPredictionStatsDto } from "../../../../../../Dto/F1Predictions/LeadGapPredictionStatsDto";
import { DiscordMemberDto } from "../../../../../../Dto/Users/DiscordMemberDto";

interface Props {
  data: LeadGapPredictionStatsDto[];
  members: DiscordMemberDto[];
}

export default function F1LeadGapStaircase({ data, members }: Props) {
  if (data.length === 0) {
    return (
      <Typography variant="body2" color="text.disabled">
        Нет данных
      </Typography>
    );
  }

  const maxDiff = Math.max(...data.map((x) => x.difference), 0.01);

  const getMemberName = (userId: string) => {
    const m = members.find((x) => x.userId === userId);
    return m?.serverName ?? m?.userName ?? "—";
  };

  return (
    <Card sx={{ backgroundColor: "transparent" }}>
      <CardContent>
        <Typography
          variant="subtitle2"
          color="text.secondary"
          gutterBottom
          sx={{ fontWeight: 600 }}
        >
          Самые точные предсказания отрыва
        </Typography>
        <Box
          sx={{
            mt: 1,
            display: "grid",
            gridTemplateColumns: "175px 1fr",
            alignItems: "center",
            rowGap: 1,
          }}
        >
          {data.map((entry, i) => {
            const name = getMemberName(entry.userId);
            const label = `${name} (${entry.raceName})`;
            const widthPercent = (entry.difference / maxDiff) * 80;

            return [
              <Typography
                variant="caption"
                color="text.secondary"
                sx={{
                  textAlign: "right",
                  pr: 1.5,
                  overflow: "hidden",
                  textOverflow: "ellipsis",
                  whiteSpace: "nowrap",
                  cursor: "default",
                }}
              >
                {label}
              </Typography>,

              <Box key={`bar-${i}`} sx={{ display: "flex", alignItems: "center" }}>
                <Tooltip
                  title={
                    <Box>
                      <Typography variant="caption" display="block">
                        Предсказание: {entry.predictedGap.toFixed(3)} сек
                      </Typography>
                      <Typography variant="caption" display="block">
                        Реальный отрыв: {entry.actualGap.toFixed(3)} сек
                      </Typography>
                    </Box>
                  }
                  arrow
                  placement="top"
                >
                  <Box
                    sx={{
                      width: `${widthPercent}%`,
                      minWidth: 8,
                      height: 28,
                      bgcolor:
                        entry.difference === 0 ? "success.main" : "primary.main",
                      borderRadius: "0 4px 4px 0",
                      cursor: "default",
                      flexShrink: 0,
                    }}
                  />
                </Tooltip>
                <Typography
                  variant="caption"
                  fontWeight="bold"
                  sx={{ pl: 1, whiteSpace: "nowrap" }}
                >
                  +{entry.difference.toFixed(3)}
                </Typography>
              </Box>,
            ];
          })}
        </Box>
      </CardContent>
    </Card>
  );
}
