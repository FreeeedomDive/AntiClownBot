import { useParams } from "react-router-dom";
import React, { useEffect, useState } from "react";
import { UserAchievementWithDetailsDto } from "../../../../Dto/Achievements/UserAchievementWithDetailsDto";
import { Skeleton } from "@mui/material";
import { Stack, Tooltip, Typography } from "@mui/material";
import AchievementsApi from "../../../../Api/AchievementsApi";
import { formatDate } from "../../../../Helpers/DateHelpers";

const SIZE = 64;
export default function UserAchievements() {
  const { userId = "" } = useParams<"userId">();
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
    <Stack direction={"row"} spacing="8px">
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
            <img
              src={`data:image/png;base64,${achievement.logo}`}
              alt={achievement.name}
              style={{
                width: SIZE,
                height: SIZE,
                objectFit: "contain",
                cursor: "pointer",
                borderRadius: 4,
              }}
            />
          </Tooltip>
        ))}
    </Stack>
  );
}
