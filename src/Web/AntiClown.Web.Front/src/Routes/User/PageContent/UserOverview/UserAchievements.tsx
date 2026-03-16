import React, { useEffect, useState } from "react";
import { UserAchievementWithDetailsDto } from "../../../../Dto/Achievements/UserAchievementWithDetailsDto";
import { Box, Skeleton, Stack, Tooltip, Typography } from "@mui/material";
import AchievementsApi from "../../../../Api/AchievementsApi";
import { formatDate } from "../../../../Helpers/DateHelpers";

interface Props {
  userId: string;
}

const SIZE = 64;
export default function UserAchievements({userId}: Props) {
  const [loading, setLoading] = useState(true);
  const [achievements, setAchievements] = useState<
    UserAchievementWithDetailsDto[]
  >([]);

  useEffect(() => {
    async function updateAchievements(): Promise<void> {
      const data = await AchievementsApi.getByUser(userId);
      setAchievements(data);
    }

    updateAchievements()
      .catch(console.error)
      .finally(() => setLoading(false));
  }, [userId]);

  return (
    <Stack direction={"row"} spacing={1} flexWrap="wrap">
      {loading && (
        <>
          <Skeleton variant="rounded" sx={{ width: SIZE, height: SIZE }} />
          <Skeleton variant="rounded" sx={{ width: SIZE, height: SIZE }} />
          <Skeleton variant="rounded" sx={{ width: SIZE, height: SIZE }} />
        </>
      )}
      {!loading &&
        achievements.length > 0 &&
        achievements.map((achievement) => (
          <Tooltip
            key={achievement.id}
            title={
              <Stack direction={"column"} spacing="2px">
                <Typography variant={"body2"}>{achievement.name}</Typography>
                <Typography variant={"caption"}>
                  Выдано {formatDate(achievement.grantedAt)}
                </Typography>
              </Stack>
            }
            arrow
          >
            <Box
              component="img"
              src={`data:image/png;base64,${achievement.logo}`}
              alt={achievement.name}
              sx={{
                width: SIZE,
                height: SIZE,
                objectFit: "contain",
                cursor: "pointer",
                borderRadius: "4px",
              }}
            />
          </Tooltip>
        ))}
    </Stack>
  );
}
