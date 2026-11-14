import { Skeleton, Stack } from "@mui/material";

export default function F1PredictionFormSkeleton({
  season,
}: {
  season: number;
}) {
  return (
    <Stack spacing={2} aria-hidden="true" sx={{ opacity: 0.35 }}>
      <Skeleton variant="rounded" animation={false} height={40} />
      <Skeleton variant="rounded" animation={false} height={40} />
      {season >= 2024 && (
        <>
          <Stack direction="row" spacing={0.5}>
            {[0, 1, 2, 3].map((index) => (
              <Skeleton
                key={index}
                variant="rounded"
                animation={false}
                height={32}
                sx={{ flex: 1 }}
              />
            ))}
          </Stack>
          <Skeleton variant="rounded" animation={false} height={40} />
        </>
      )}
      {season >= 2026 && (
        <Skeleton variant="rounded" animation={false} height={40} />
      )}
    </Stack>
  );
}
