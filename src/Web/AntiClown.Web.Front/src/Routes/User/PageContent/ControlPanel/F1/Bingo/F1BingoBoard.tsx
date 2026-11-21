import { useParams, useSearchParams } from "react-router-dom";
import React, { useEffect, useState } from "react";
import F1BingoV2Api from "../../../../../../Api/F1BingoV2Api";
import { getF1ErrorMessage } from "../../../../../../Api/F1PredictionsApi";
import { F1BingoDetailsDto } from "../../../../../../Dto/F1Bingo/F1BingoDetailsDto";
import { RightsDto } from "../../../../../../Dto/Rights/RightsDto";
import { RightsWrapper } from "../../../../../../Components/UserRights/RightsWrapper";
import { Alert, Button, Grid, Stack } from "@mui/material";
import { Loader } from "../../../../../../Components/Loader/Loader";
import F1BingoCard from "./F1BingoCard";

export default function F1BingoBoard() {
  const { userId } = useParams<"userId">();
  const [searchParams] = useSearchParams();
  const currentYear = new Date().getFullYear();
  const requestedSeason = Number(searchParams.get("season") ?? currentYear);
  const season =
    Number.isInteger(requestedSeason) &&
    requestedSeason >= 2023 &&
    requestedSeason <= currentYear
      ? requestedSeason
      : currentYear;

  return (
    <RightsWrapper requiredRights={[RightsDto.F1Predictions]}>
      <BingoBoard key={`${userId}:${season}`} season={season} />
    </RightsWrapper>
  );
}

function BingoBoard({ season }: { season: number }) {
  const [details, setDetails] = useState<F1BingoDetailsDto>();
  const [error, setError] = useState<string>();
  const [revision, setRevision] = useState(0);

  useEffect(() => {
    const controller = new AbortController();
    F1BingoV2Api.getOwnBoard(season, controller.signal)
      .then((data) => {
        if (!controller.signal.aborted) setDetails(data);
      })
      .catch((error) => {
        if (!controller.signal.aborted) setError(getF1ErrorMessage(error));
      });
    return () => controller.abort();
  }, [season, revision]);

  if (error) {
    return (
      <Alert
        severity="error"
        action={
          <Button
            color="inherit"
            onClick={() => {
              setError(undefined);
              setRevision((value) => value + 1);
            }}
          >
            Повторить
          </Button>
        }
      >
        {error}
      </Alert>
    );
  }
  if (!details) return <Loader />;
  if (!details.board) {
    return (
      <Alert severity="info">
        Доска бинго за сезон {details.season} недоступна.
      </Alert>
    );
  }

  return (
    <Stack direction="column" spacing={2}>
      {details.board.bingoAchieved && <Alert severity="success">Бинго!</Alert>}
      <Grid
        container
        spacing={1}
        sx={{ width: "100%", height: "100%", margin: "auto" }}
      >
        {details.board.cards.map((card) => (
          <Grid
            item
            key={card.id}
            xs={12}
            sm={6}
            md={4}
            lg={2.4}
            sx={{
              display: "flex",
              justifyContent: "center",
              alignItems: "center",
            }}
          >
            <F1BingoCard card={card} />
          </Grid>
        ))}
      </Grid>
    </Stack>
  );
}
