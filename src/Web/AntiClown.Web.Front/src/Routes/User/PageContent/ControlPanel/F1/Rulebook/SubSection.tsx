import React from "react";
import { Card, CardContent, Chip, Stack, Typography } from "@mui/material";

interface Props {
  title: string;
  badge?: string;
  badgeColor?:
    | "default"
    | "primary"
    | "secondary"
    | "error"
    | "info"
    | "success"
    | "warning";
  seasons?: string;
  isActive?: boolean;
  children: React.ReactNode;
}

export function SubSection({
  title,
  badge,
  badgeColor = "primary",
  seasons,
  isActive = true,
  children,
}: Props) {
  return (
    <Card
      sx={{
        backgroundColor: isActive
          ? "rgba(255,255,255,0.05)"
          : "rgba(255,255,255,0.02)",
        borderRadius: 2,
        opacity: isActive ? 1 : 0.72,
      }}
    >
      <CardContent>
        <Stack direction="row" spacing={1} alignItems="center" sx={{ mb: 1.5 }}>
          <Typography variant="h6" sx={{ fontWeight: "bold", flexGrow: 1 }}>
            {title}
          </Typography>
          {badge && <Chip label={badge} color={badgeColor} size="small" />}
          {seasons && (
            <Chip
              label={seasons}
              color={isActive ? "success" : "error"}
              size="small"
              variant="outlined"
            />
          )}
        </Stack>
        <Stack spacing={1}>{children}</Stack>
      </CardContent>
    </Card>
  );
}
