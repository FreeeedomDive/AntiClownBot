import { Box, Skeleton, Stack, Typography } from "@mui/material";

export default function F1DriverListSkeleton({ count }: { count: number }) {
  return (
    <Box aria-hidden="true" sx={{ opacity: 0.35 }}>
      {Array.from({ length: count || 22 }, (_, index) => (
        <Stack
          key={index}
          direction="row"
          spacing={1}
          sx={{ height: 27, alignItems: "center" }}
        >
          <Typography
            variant="caption"
            color="text.secondary"
            sx={{ minWidth: 18 }}
          >
            {index + 1}
          </Typography>
          <Skeleton
            variant="rounded"
            animation={false}
            width={40}
            height={17}
          />
          <Skeleton
            animation={false}
            width={index % 2 === 0 ? "50%" : "65%"}
            height={22}
          />
        </Stack>
      ))}
    </Box>
  );
}
