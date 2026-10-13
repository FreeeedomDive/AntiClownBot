import React, { useEffect, useState } from "react";
import {
  Alert,
  InputAdornment,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
} from "@mui/material";
import { Search } from "@mui/icons-material";
import DiscordMembersApi from "../../../../../../Api/DiscordMembersApi";
import { F1PredictionDto } from "../../../../../../Dto/F1Predictions/F1PredictionDto";
import { getPredictionMaxPosition } from "../../../../../../Dto/F1Predictions/F1PredictionHelpers";
import { F1RaceDto } from "../../../../../../Dto/F1Predictions/F1RaceDto";
import { F1TeamDto } from "../../../../../../Dto/F1Predictions/F1TeamDto";
import { DiscordMemberDto } from "../../../../../../Dto/Users/DiscordMemberDto";
import F1UserPredictionRow from "./F1UserPredictionRow";

interface Props {
  race: F1RaceDto;
  teams: F1TeamDto[];
  drivers: string[];
  disabled: boolean;
  onSavingChange: (saving: boolean) => void;
  onDirtyChange: (userId: string, dirty: boolean) => void;
  onSaved: (prediction: F1PredictionDto) => void;
}

export default function F1UserPredictionsTable({
  race,
  teams,
  drivers,
  disabled,
  onSavingChange,
  onDirtyChange,
  onSaved,
}: Props) {
  const [search, setSearch] = useState("");
  const [members, setMembers] = useState<DiscordMemberDto[]>([]);
  const [membersError, setMembersError] = useState(false);
  const userIdsKey = race.predictions
    .map((prediction) => prediction.userId)
    .join(",");

  useEffect(() => {
    if (!userIdsKey) return;
    let cancelled = false;
    DiscordMembersApi.getMembers(userIdsKey.split(","))
      .then((result) => {
        if (!Array.isArray(result))
          throw new Error("Invalid Discord members response");
        if (!cancelled) setMembers(result);
      })
      .catch(() => {
        if (!cancelled) setMembersError(true);
      });
    return () => {
      cancelled = true;
    };
  }, [userIdsKey]);

  const membersById = new Map(members.map((member) => [member.userId, member]));
  const nameFor = (userId: string) => {
    const member = membersById.get(userId);
    return member?.serverName || member?.userName || userId;
  };
  const predictions = [...race.predictions].sort((a, b) =>
    nameFor(a.userId).localeCompare(nameFor(b.userId), "ru"),
  );
  const matches = (userId: string) => {
    const member = membersById.get(userId);
    return [userId, member?.serverName, member?.userName].some(
      (name) =>
        name?.toLocaleLowerCase().includes(search.trim().toLocaleLowerCase()),
    );
  };
  const visibleCount = predictions.filter((prediction) =>
    matches(prediction.userId),
  ).length;
  const positionDriver = race.conditions?.positionPredictionDriver;
  const maxPosition = getPredictionMaxPosition(race, teams);

  if (predictions.length === 0) {
    return (
      <Alert severity="info">
        На эту гонку пока нет пользовательских предсказаний.
      </Alert>
    );
  }

  return (
    <Stack spacing={2}>
      <Stack
        direction={{ xs: "column", sm: "row" }}
        spacing={2}
        justifyContent="space-between"
        alignItems={{ sm: "center" }}
      >
        <TextField
          size="small"
          placeholder="Поиск по нику"
          value={search}
          onChange={(event) => setSearch(event.target.value)}
          sx={{ width: { xs: "100%", sm: 260 } }}
          inputProps={{ "aria-label": "Поиск предсказаний по пользователю" }}
          InputProps={{
            startAdornment: (
              <InputAdornment position="start">
                <Search fontSize="small" />
              </InputAdornment>
            ),
          }}
        />
      </Stack>
      {membersError && (
        <Alert severity="warning">
          Не удалось загрузить ники. Предсказания доступны по ID пользователей.
        </Alert>
      )}
      <Paper
        variant="outlined"
        sx={{
          borderRadius: 2,
          overflow: "hidden",
          bgcolor: "rgba(255,255,255,0.025)",
          backgroundImage: "none",
          minWidth: 0,
        }}
      >
        <TableContainer>
          <Table
            size="small"
            sx={{
              minWidth: positionDriver ? 1120 : 1000,
              "& .MuiTableCell-root": { borderColor: "rgba(255,255,255,0.08)" },
              "& .MuiTableHead-root .MuiTableCell-root": {
                color: "text.secondary",
                fontSize: "0.75rem",
                fontWeight: 600,
                py: 1.5,
                whiteSpace: "nowrap",
              },
            }}
          >
            <TableHead>
              <TableRow>
                <TableCell sx={{ minWidth: 180 }}>Пользователь</TableCell>
                <TableCell sx={{ minWidth: 175 }}>10-е место</TableCell>
                <TableCell sx={{ minWidth: 280 }}>
                  DNF · Никто / 5 пилотов
                </TableCell>
                <TableCell sx={{ minWidth: 95 }}>Инциденты</TableCell>
                <TableCell sx={{ minWidth: 130 }}>Отрыв, сек</TableCell>
                {positionDriver && (
                  <TableCell sx={{ minWidth: 110 }}>
                    Позиция {positionDriver}
                  </TableCell>
                )}
                <TableCell sx={{ minWidth: 150 }}>Сохранение</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {predictions.map((prediction) => (
                <F1UserPredictionRow
                  key={prediction.userId}
                  prediction={prediction}
                  member={membersById.get(prediction.userId)}
                  teams={teams}
                  drivers={drivers}
                  qualifyingGrid={race.qualifyingGrid}
                  maxPosition={maxPosition}
                  showPosition={!!positionDriver}
                  hidden={!matches(prediction.userId)}
                  disabled={disabled}
                  onSavingChange={onSavingChange}
                  onDirtyChange={onDirtyChange}
                  onSaved={onSaved}
                />
              ))}
              {visibleCount === 0 && (
                <TableRow>
                  <TableCell
                    colSpan={positionDriver ? 7 : 6}
                    sx={{ py: 4, textAlign: "center", color: "text.secondary" }}
                  >
                    Пользователи не найдены
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </TableContainer>
      </Paper>
    </Stack>
  );
}
